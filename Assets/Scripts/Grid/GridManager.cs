using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Grid playAreaGrid;
    [SerializeField] private Grid inventoryGrid;
    [SerializeField] private GameObject previewObj;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject cursor;
    [SerializeField] private GameObject cardPrefab;
    private List<Vector3Int> inventoryPos = new();
    [SerializeField] private int inventoryRows = 3;
    [SerializeField] private int inventoryCols = 3;
    [SerializeField] private int playAreaRows = 5;
    [SerializeField] private int playAreaCols = 5;
    [SerializeField] private Canvas inventoryCanvas;
    [SerializeField] private Canvas playAreaCanvas;
    [SerializeField] private GameObject prefabUI;
    private Dictionary<Vector3Int, GameObject> availableTiles = new();
    private Dictionary<Vector3Int, GameObject> placedTiles = new();
    [SerializeField] private int inventoryXMax = 6;
    [SerializeField] private int inventoryXMin = 4;
    [SerializeField] private int inventoryYMax = 1;
    [SerializeField] private int inventoryYMin = -1;
    [SerializeField] private int playAreaXMax = 2;
    [SerializeField] private int playAreaXMin = -2;
    [SerializeField] private int playAreaYMax = 2;
    [SerializeField] private int playAreaYMin = -2;
    private bool holdingCard;
    private bool holdingTile;
    private Vector3Int cardPos;

    InputAction primaryAction;
    InputAction releaseAction;
    InputAction removeAction;

    void Awake() {
        RectTransform playAreaRT = playAreaCanvas.GetComponent<RectTransform>();
        float playAreaCellSizeX = playAreaCanvas.GetComponent<GridLayoutGroup>().cellSize.x;
        float playAreaCellSizeY = playAreaCanvas.GetComponent<GridLayoutGroup>().cellSize.y;

        playAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playAreaCols * playAreaCellSizeX);
        playAreaRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, playAreaRows * playAreaCellSizeY);

        RectTransform inventoryRT = inventoryCanvas.GetComponent<RectTransform>();
        float inventoryCellSizeX = inventoryCanvas.GetComponent<GridLayoutGroup>().cellSize.x;
        float inventoryCellSizeY = inventoryCanvas.GetComponent<GridLayoutGroup>().cellSize.y;

        inventoryRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryCols * inventoryCellSizeX);
        inventoryRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryRows * inventoryCellSizeY);

        for (int i = 0; i < playAreaCols * playAreaRows; i++) {
            Instantiate(prefabUI, playAreaCanvas.transform);
        }

        for (int i = 0; i < inventoryCols * inventoryRows; i++) {
            Instantiate(prefabUI, inventoryCanvas.transform);
        }
        

        for (int i = 0; i < inventoryRows; i++) {
            for (int j = 0; j < inventoryCols; j++) {
                Vector3Int pos = new Vector3Int(inventoryXMin + j, inventoryYMax - i, 0);
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

        if (CursorOverInventory(mouseWorldPos) && !holdingCard && !holdingTile) {
            cellPos = inventoryGrid.WorldToCell(mouseWorldPos);

            if(primaryAction.triggered) PickUpCard(cellPos);
            return;
        }

        if (CursorOverPlayArea(mouseWorldPos)) {
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
            Vector3 pos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, -1);
            previewObj.transform.position = pos;

            if (releaseAction.triggered) {
                if (holdingCard) ReleaseCard();
                else if (holdingTile) ReleaseTile();
            }

            return;
        }
    }

    bool CursorOverInventory(Vector3 pos) {
        Vector3Int cellPos = inventoryGrid.WorldToCell(pos);

        if (cellPos.x > inventoryXMax || cellPos.x < inventoryXMin || cellPos.y > inventoryYMax || cellPos.y < inventoryYMin) return false;

        else return true;
    }

    bool CursorOverPlayArea(Vector3 pos) {
        Vector3Int cellPos = playAreaGrid.WorldToCell(pos);

        if (cellPos.x > playAreaXMax || cellPos.x < playAreaXMin || cellPos.y > playAreaYMax || cellPos.y < playAreaYMin) return false;

        else return true;
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
    }

    void RemoveTile(Vector3Int pos) {
        if (!placedTiles.ContainsKey(pos)) return;

        Destroy(placedTiles[pos]);
        placedTiles.Remove(pos);

        Vector3Int spawnPos = inventoryPos[availableTiles.Count];
        GameObject newCard = Instantiate(cardPrefab, inventoryGrid.GetCellCenterWorld(spawnPos), Quaternion.identity);
        availableTiles.Add(spawnPos, newCard);
    }

    void ReleaseTile() {
        if (!holdingTile) return;

        previewObj.transform.position = playAreaGrid.GetCellCenterWorld(cardPos);
        previewObj = null;
        holdingTile = false;
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
    }
}
