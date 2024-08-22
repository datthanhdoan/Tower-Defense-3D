using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlacementState
{
    InPlacement,
    NotInPlacement
}
public class GridPlacement : MonoBehaviour
{
    /// <summary>
    /// Grid Placement
    /// </summary>
    [Header("Grid Placement")]
    [SerializeField] private LayerMask _gridLayerMask;
    [SerializeField] private Grid _grid;
    [SerializeField] private GameObject _mousePointer;
    [SerializeField] private GameObject _gridVisualization;
    private GridData _gridData = new GridData();


    [Header("Input")]
    [SerializeField] private InputHandel _inputHandel;

    [Header("Tower Data")]
    [SerializeField] private TowerDataSO _towerDataSO;


    /// <summary>
    /// In Game Variables
    /// </summary>
    private TowerData _selectedTowerData;
    private Vector3Int _currentCellPosition;
    private Vector3Int _selectedCellPosition;

    private PlacementState _placementStrte = PlacementState.NotInPlacement;

    private void OnEnable()
    {
        _inputHandel.OnStartPlacing += StartPlacing;
        _inputHandel.OnExitPlacing += ExitPlacing;
    }

    private void OnDisable()
    {
        _inputHandel.OnStartPlacing -= StartPlacing;
        _inputHandel.OnExitPlacing -= ExitPlacing;
    }

    private void Start()
    {
        _gridVisualization.SetActive(false);
        SelectTower(TowerName.Archer);
    }

    public void SelectTower(TowerName towerName)
    {
        _selectedTowerData = _towerDataSO.GetTowerData(towerName);
    }

    private void StartPlacing()
    {
        Debug.Log("Start Placement");
        _placementStrte = PlacementState.InPlacement;
        _gridVisualization.SetActive(true);
    }

    private void DuringPlacement()
    {
        // if (_inputHandel.IsPointerOverUI())
        //     return;
        if (_selectedTowerData == null)
        {
            Debug.LogError("No Tower Selected");
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Button Down - Place Tower");
            if (_gridData.IsCellEmpty(_currentCellPosition))
            {
                Debug.Log("Cell is Empty - Place Tower");
                // Instantiate(_selectedTowerData.Prefab, _grid.CellToWorld(_currentCellPosition), Quaternion.identity);
                // Center the tower on the cell
                Instantiate(_selectedTowerData.Prefab, _grid.GetCellCenterWorld(_currentCellPosition), Quaternion.identity);
                _gridData.AddCell(_currentCellPosition, CellState.Placed);
            }
        }

    }


    private void ExitPlacing()
    {
        _placementStrte = PlacementState.NotInPlacement;
        _gridVisualization.SetActive(false);
    }

    private void Update()
    {
        if (_placementStrte == PlacementState.InPlacement)
        {
            Vector3 currentMousePos = _inputHandel.GetSelectedMapPosition();
            _currentCellPosition = _grid.WorldToCell(currentMousePos);

            _mousePointer.transform.position = new Vector3(_grid.GetCellCenterWorld(_currentCellPosition).x, 0.15f, _grid.GetCellCenterWorld(_currentCellPosition).z);
            DuringPlacement();
        }
    }



}
