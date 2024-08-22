using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridData
{
    private Dictionary<Vector3Int, CellData> _gridData = new Dictionary<Vector3Int, CellData>();
    public Dictionary<Vector3Int, CellData> GetGridData => _gridData;

    public void AddCell(Vector3Int position, CellState cellState)
    {
        if (_gridData.ContainsKey(position))
            return;

        _gridData.Add(position, new CellData(position, cellState));
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
    Empty,
    Placed
}
public class CellData
{
    public Vector3Int position { get; private set; }
    public CellState cellState { get; private set; }

    public CellData(Vector3Int position, CellState cellState)
    {
        this.position = position;
        this.cellState = cellState;
    }
}
