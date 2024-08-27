using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

[ExecuteInEditMode]
public class GridDataVisual : MonoBehaviour
{
    public Grid grid; // Tham chiếu tới đối tượng Grid
    public Vector2Int gridSize = new Vector2Int(3, 3); // Kích thước lưới, số ô xung quanh tâm
    public Color emptyColor = Color.green; // Màu sắc của ô trống
    public Color placedColor = Color.red; // Màu sắc của ô đã đặt
    public Vector3 center = Vector3.zero; // Tâm của lưới
    public GridData _gridData; // Tham chiếu tới đối tượng GridData để lưu trữ dữ liệu

    // [HideInInspector]
    public bool isEnabled = true; // Thuộc tính để bật/tắt script

    public void UpdateGridDataWithNavMesh()
    {
        if (!isEnabled)
            return;
        if (grid == null || _gridData == null)
        {
            Debug.LogError("Grid or GridData is not assigned.");
            return;
        }

        Debug.Log("Updating Grid Data with NavMesh");

        // Tạo dữ liệu grid dựa trên gridSize và center
        _gridData.GetGridData.Clear();

        for (int x = -gridSize.x; x <= gridSize.x; x++)
        {
            for (int y = -gridSize.y; y <= gridSize.y; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, 0, y);
                Vector3 cellWorldPosition = grid.CellToWorld(cellPosition) + center;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(cellWorldPosition, out hit, 1.0f, NavMesh.AllAreas))
                {
                    _gridData.UpdateCell(cellPosition, CellState.Placed);
                }
                else
                {
                    _gridData.UpdateCell(cellPosition, CellState.Empty);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (grid == null || _gridData == null)
            return;

        Color originalColor = Gizmos.color;

        // Tính toán nửa kích thước của lưới
        float halfGridWidth = gridSize.x * grid.cellSize.x / 2f;
        float halfGridHeight = gridSize.y * grid.cellSize.y / 2f;

        // Duyệt qua tất cả các ô của Grid, bắt đầu từ tâm
        for (int x = -gridSize.x; x <= gridSize.x; x++)
        {
            for (int y = -gridSize.y; y <= gridSize.y; y++)
            {
                Vector3 cellOffset = new Vector3(x * grid.cellSize.x, 0, y * grid.cellSize.y);
                Vector3 cellWorldPosition = center + cellOffset;

                Vector3Int cellPosition = grid.WorldToCell(cellWorldPosition);

                if (_gridData.GetGridData.ContainsKey(cellPosition))
                {
                    CellData cell = _gridData.GetCell(cellPosition);
                    Gizmos.color = cell.cellState == CellState.Empty ? emptyColor : placedColor;
                }
                else
                {
                    Gizmos.color = emptyColor;
                }

                Vector3 cellSize = new Vector3(grid.cellSize.x, 0.1f, grid.cellSize.y);
                Gizmos.DrawCube(cellWorldPosition, cellSize);
            }
        }

        Gizmos.color = originalColor;
    }
}
