using UnityEngine;
using UnityEngine.InputSystem;
using static AudioEvents;

public class InventoryManager : MonoBehaviour {
    [SerializeField] private Grid inventoryGrid;
    [SerializeField] private GameObject previewObj;
    [SerializeField] private float xMax = 2.15f;
    [SerializeField] private float xMin = -2.25f;
    [SerializeField] private float yMax = 2.25f;
    [SerializeField] private float yMin = -2.15f;

    void Update() {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        mouseWorldPos.z = 0;

        Vector3Int cellPos = inventoryGrid.WorldToCell(mouseWorldPos);

        Vector3 snappedPos = inventoryGrid.GetCellCenterWorld(cellPos);

        if (snappedPos.x > xMax || snappedPos.x < xMin || snappedPos.y > yMax || snappedPos.y < yMin) {
            Debug.Log("Mouse outside of the grid.");
            return;
        }

        previewObj.transform.position = snappedPos;

        if(Mouse.current.leftButton.wasPressedThisFrame) PickUpTile(snappedPos);
    }

    void PickUpTile(Vector3 pos) {
        RaiseTilePickedUp(); //play audio for picking up tile
        return;
    }
}
