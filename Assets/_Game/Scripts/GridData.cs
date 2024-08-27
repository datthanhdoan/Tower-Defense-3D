using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "ScriptableObjects/GridData", order = 1)]
public class GridData : ScriptableObject
{
    private Dictionary<Vector3Int, CellData> _gridData = new Dictionary<Vector3Int, CellData>();
    public Dictionary<Vector3Int, CellData> GetGridData => _gridData;

    public void AddCell(Vector3Int position, CellState cellState)
    {
        if (_gridData.ContainsKey(position))
            return;

        _gridData.Add(position, new CellData(position, cellState));
    }

    public void UpdateCell(Vector3Int position, CellState cellState)
    {
        if (!_gridData.ContainsKey(position))
            return;

        _gridData[position].SetCellState(cellState);
    }

    public void RemoveCell(Vector3Int position)
    {
        if (!_gridData.ContainsKey(position))
            return;

        _gridData.Remove(position);
    }

    public CellData GetCell(Vector3Int position)
    {
        if (!_gridData.ContainsKey(position))
            return null;

        return _gridData[position];
    }

    public bool IsCellEmpty(Vector3Int position)
    {
        if (!_gridData.ContainsKey(position))
            return true;

        return _gridData[position].cellState == CellState.Empty;
    }


}
public enum CellState
{
    Empty = 0,
    Placed = 1,
    EnemyPath = 2
}
public class CellData
{
    public Vector3Int position { get; private set; }
    public CellState cellState { get; private set; }

    public void SetCellState(CellState cellState)
    {
        this.cellState = cellState;
    }
    public CellData(Vector3Int position, CellState cellState)
    {
        this.position = position;
        this.cellState = cellState;
    }
}
