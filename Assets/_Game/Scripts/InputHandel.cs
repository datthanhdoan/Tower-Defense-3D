using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandel : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 currentPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action OnStartPlacing, OnExitPlacing;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key was pressed - Start placing");
            OnStartPlacing?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key was pressed - Exit placing");
            OnExitPlacing?.Invoke();
        }
    }

    // public bool IsPointerOverUI()
    //     => EventSystem.current.IsPointerOverGameObject();

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, placementLayermask))
        {
            currentPosition = hit.point;
        }

        return currentPosition;
    }
}