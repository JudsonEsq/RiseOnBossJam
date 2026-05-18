using System;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "GridSystemSO", menuName = "Scriptable Objects/GridSystemSO")]
public class GridSystemSO : ScriptableObject
{
    [NonSerialized] public Grid grid;
    public int columns;
    public int rows;
    public float gridSizeX;
    public float gridSizeY;
    public Vector2Int cellPosMin;
    public Vector2Int cellPosMax;
    public GameObject gridCanvasPrefab;
    [NonSerialized] public Canvas canvas;

    public void SetGrid() {
        if (grid == null) return;
        
        grid.cellSize = new Vector2(gridSizeX, gridSizeY);

        if (canvas == null) {
            if (gridCanvasPrefab == null) return;

            GameObject canvasGO = Instantiate(gridCanvasPrefab);
            
            if(canvasGO.TryGetComponent(out Canvas c)) {
                canvas = c;
                c.worldCamera = Camera.main;
            }
        }

        if(canvas.TryGetComponent(out GridLayoutGroup gLG)) gLG.cellSize = new Vector2(gridSizeX, gridSizeY);

        if (canvas.TryGetComponent(out RectTransform canvasRTransform)) {
            canvasRTransform
                .SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, columns * gridSizeX);
            canvasRTransform
                .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rows * gridSizeY);
        }
    }

    public bool CheckForCursor(Vector3 pos) {
        Vector3Int cellPos = grid.WorldToCell(pos);

        if (cellPos.x > cellPosMax.x || cellPos.x < cellPosMin.x || cellPos.y > cellPosMax.y || cellPos.y < cellPosMin.y) return false;

        else return true;
    }
}
