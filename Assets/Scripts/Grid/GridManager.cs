using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static AudioEvents;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Grid inventoryGrid;
    [SerializeField] private Grid playAreaGrid;
    [SerializeField] private GridSystemSO inventoryGridSO;
    [SerializeField] private GridSystemSO playAreaGridSO;
    [SerializeField] private GameObject previewObj;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject cursor;
    [SerializeField] private GameObject cardPrefab;
    private List<Vector3Int> inventoryPos = new();
    [SerializeField] private Canvas inventoryCanvas;
    [SerializeField] private Canvas playAreaCanvas;
    [SerializeField] private GameObject prefabUI;
    private Dictionary<Vector3Int, GameObject> availableTiles = new();
    private Dictionary<Vector3Int, GameObject> placedTiles = new();
    private bool holdingCard;
    private bool holdingTile;
    private Vector3Int cardPos;

    // MOVE THESE TO AN INVENTORY SO RUNTIME SET
    private Dictionary<Vector3Int, Room> inventoryRooms;
    private Dictionary<Vector3Int, Trap> inventoryTraps;

    // MOVE THESE TO AN PLAY AREA SO RUNTIME SET
    private Dictionary<Vector3Int, Room> placedRooms;
    private Dictionary<Vector3Int, Trap> placedTraps;

    InputAction primaryAction;
    InputAction releaseAction;
    InputAction removeAction;

    void Awake() {
        if (inventoryGrid != null) {
            inventoryGridSO.grid = inventoryGrid;
            if (inventoryCanvas != null) inventoryGridSO.canvas = inventoryCanvas;
            inventoryGridSO.SetGrid();
        }

        if (playAreaGrid != null) {
            playAreaGridSO.grid = playAreaGrid;
            if (playAreaCanvas != null) playAreaGridSO.canvas = playAreaCanvas;
            playAreaGridSO.SetGrid();
        }

        for (int i = 0; i < playAreaGridSO.columns * playAreaGridSO.rows; i++) {
            Instantiate(prefabUI, playAreaCanvas.transform);
        }

        for (int i = 0; i < inventoryGridSO.columns * inventoryGridSO.rows; i++) {
            Instantiate(prefabUI, inventoryCanvas.transform);
        }
        

        for (int i = 0; i < inventoryGridSO.rows; i++) {
            for (int j = 0; j < inventoryGridSO.columns; j++) {
                Vector3Int pos = new Vector3Int(inventoryGridSO.cellPosMin.x + j, inventoryGridSO.cellPosMax.y - i, 0);
                inventoryPos.Add(pos);

                GameObject tile = Instantiate(cardPrefab, inventoryGrid.GetCellCenterWorld(pos), Quaternion.identity);
                availableTiles.Add(pos, tile);
            }
        }

        primaryAction = InputSystem.actions.FindAction("Pick Up / Place");
        releaseAction = InputSystem.actions.FindAction("Release");
        removeAction = InputSystem.actions.FindAction("Remove");
    }

    void Update() {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        mouseWorldPos.z = 0;

        if (cursor != null) cursor.transform.position = mouseWorldPos;

        Vector3Int cellPos;
        Vector3 snappedPos;

        if (inventoryGridSO.CheckForCursor(mouseWorldPos) && !holdingCard && !holdingTile) {
            cellPos = inventoryGrid.WorldToCell(mouseWorldPos);

            if(primaryAction.triggered) PickUpCard(cellPos);
            return;
        }

        if (playAreaGridSO.CheckForCursor(mouseWorldPos)) {
            cellPos = playAreaGrid.WorldToCell(mouseWorldPos);
            snappedPos = playAreaGrid.GetCellCenterWorld(cellPos);
            snappedPos.z = 0;

            if (primaryAction.triggered && !holdingCard && !holdingTile) {
                PickUpTile(cellPos);
                return;
            }

            if (holdingCard || holdingTile) {
                previewObj.transform.position = snappedPos;

                if (primaryAction.triggered) PlaceTile(cellPos, snappedPos);
                if (releaseAction.triggered) {
                    if (holdingCard) ReleaseCard();
                    else if (holdingTile) ReleaseTile();
                }

                return;
            }

            if (removeAction.triggered) RemoveTile(cellPos);
            return;
        }

        if (holdingCard || holdingTile) {
            Vector3 pos = new(mouseWorldPos.x, mouseWorldPos.y, -1);
            previewObj.transform.position = pos;

            if (releaseAction.triggered) {
                if (holdingCard) ReleaseCard();
                else if (holdingTile) ReleaseTile();
            }

            return;
        }
    }

    void PickUpCard(Vector3Int pos) {
        if (availableTiles.Count <= 0 || !availableTiles.TryGetValue(pos, out _)) return;

        holdingCard = true;
        previewObj = Instantiate(tilePrefab, pos, Quaternion.identity);
        cardPos = pos;
    }

    void ReleaseCard() {
        if (!holdingCard) return;

        Destroy(previewObj);
        previewObj = null;
        holdingCard = false;
    }

    void PlaceTile(Vector3Int cellPos, Vector3 worldPos) {
        if (cellPos == cardPos) {
            ReleaseTile();
            return;
        }

        if (placedTiles.ContainsKey(cellPos)) return;

        previewObj.transform.position = worldPos;
        placedTiles.Add(cellPos, previewObj);

        previewObj = null;

        if (holdingCard) {
            holdingCard = false;
            Destroy(availableTiles[cardPos]);
            availableTiles.Remove(cardPos);

            if (cardPos != inventoryPos.Last()) OrganizeInventory();
        }

        else if (holdingTile) {
            holdingTile = false;
            placedTiles.Remove(cardPos);
        }
    }

    void PickUpTile(Vector3Int pos) {
        if (placedTiles.TryGetValue(pos, out GameObject tile)) {
            holdingTile = true;
            cardPos = pos;
            previewObj = tile;
        }

        else {
            holdingTile = false;
            return;
        }

        RaiseTilePickedUp(); //play audio for picking up tile
    }

    void RemoveTile(Vector3Int pos) {
        if (!placedTiles.ContainsKey(pos)) return;

        Destroy(placedTiles[pos]);
        placedTiles.Remove(pos);

        RaiseTileDiscarded(); //play audio for destroying tile

        Vector3Int spawnPos = inventoryPos[availableTiles.Count];
        GameObject newCard = Instantiate(cardPrefab, inventoryGrid.GetCellCenterWorld(spawnPos), Quaternion.identity);
        availableTiles.Add(spawnPos, newCard);
    }

    void ReleaseTile() {
        if (!holdingTile) return;

        previewObj.transform.position = playAreaGrid.GetCellCenterWorld(cardPos);
        previewObj = null;
        holdingTile = false;
        RaiseTileReleased(); //play audio for discarding tile, although is that what this Release Tile does?]
    }

    void OrganizeInventory() {
        Dictionary<Vector3Int, GameObject> tempCards = new();
        List<Vector3Int> cardsToRemove = new();
        int i = 0;

        foreach (Vector3Int card in availableTiles.Keys) {
            if (card == inventoryPos[i]) {
                i++;
                continue;
            }

            availableTiles[card].transform.position = inventoryGrid.GetCellCenterWorld(inventoryPos[i]);
           
            tempCards.Add(inventoryPos[i], availableTiles[card]);
            cardsToRemove.Add(card);
            i++;
        }

        foreach (Vector3Int card in cardsToRemove) {
            availableTiles.Remove(card);
        }

        foreach (Vector3Int card in tempCards.Keys) {
            availableTiles.Add(card, tempCards[card]);
        }
        RaiseOrganizeInventory(); //play audio for organizing inventory
    }
}
