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
    [Tooltip("Mouse Pointer or Grid Pointer to show where the tower will be placed")]
    [SerializeField] private GameObject _mousePointer;
    [SerializeField] private GameObject _gridVisualization;
    [SerializeField] private GridData _gridData;

    [Header("Enemy")]
    [SerializeField] private List<Transform> _enemyPath;


    [Header("Input")]
    [SerializeField] private InputHandel _inputHandel;

    [Header("Tower Data")]
    [SerializeField] private TowerDataSO _towerDataSO;


    [Header("Materials")]
    [SerializeField] private Material _canPlaceMaterial;
    [SerializeField] private Material _cannotPlaceMaterial;
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
        _mousePointer.SetActive(false);
        SelectTower(TowerName.Archer);
        UpdateEnemyPathGridData();
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
        _mousePointer.SetActive(true);
    }

    private void ExitPlacing()
    {
        _placementStrte = PlacementState.NotInPlacement;
        _gridVisualization.SetActive(false);
        _mousePointer.SetActive(false);
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
                Vector3 pos = new Vector3(_grid.GetCellCenterWorld(_currentCellPosition).x, 0.15f, _grid.GetCellCenterWorld(_currentCellPosition).z);
                Instantiate(_selectedTowerData.Prefab, pos, Quaternion.identity);
                _gridData.AddCell(_currentCellPosition, CellState.Placed);
            }
        }
    }

    private void HandelInputMaterial()
    {
        if (_gridData.IsCellEmpty(_currentCellPosition))
        {
            _mousePointer.GetComponent<MeshRenderer>().material = _canPlaceMaterial;
        }
        else
        {
            _mousePointer.GetComponent<MeshRenderer>().material = _cannotPlaceMaterial;
        }
    }


    private void UpdateEnemyPathGridData()
    {
        foreach (var cell in _enemyPath)
        {
            Vector3Int cellPosition = _grid.WorldToCell(cell.position);
            _gridData.AddCell(cellPosition, CellState.EnemyPath);
        }
    }

    private void Update()
    {
        if (_placementStrte == PlacementState.InPlacement)
        {
            Vector3 currentMousePos = _inputHandel.GetSelectedMapPosition();
            _currentCellPosition = _grid.WorldToCell(currentMousePos);

            _mousePointer.transform.position = new Vector3(_grid.GetCellCenterWorld(_currentCellPosition).x, 0.15f, _grid.GetCellCenterWorld(_currentCellPosition).z);
            HandelInputMaterial();
            DuringPlacement();
        }
    }

}
