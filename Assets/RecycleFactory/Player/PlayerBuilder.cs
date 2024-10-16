using UnityEngine;
using System.Collections.Generic;

public class PlayerBuilder : MonoBehaviour
{
    [Header("Building Settings")]
    [SerializeField] private List<GameObject> buildings;
    [SerializeField] private float gridScale = 1f;

    private GameObject currentBuilding;
    public void Init()
    {
        currentBuilding = buildings[0];
    }

    private void Update()
    {
        HandleBuildingSelection();
        HandleBuildingPlacement();
    }

    private void HandleBuildingSelection()
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                SelectBuilding(i);
            }
        }
    }

    private void SelectBuilding(int index)
    {
        if (index >= 0 && index < buildings.Count)
        {
            currentBuilding = buildings[index];
            Debug.Log($"Selected building: {currentBuilding.name}");
        }
    }

    private void HandleBuildingPlacement()
    {
        if (Input.GetMouseButtonDown(0) && currentBuilding != null)
        {
            Vector3 position = GetMouseWorldPosition();
            if (position != Vector3.zero)
            {
                Instantiate(currentBuilding, position, Quaternion.identity);
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 snappedPosition = new Vector3(
                Mathf.Round(hit.point.x / gridScale) * gridScale,
                0f, // ground
                Mathf.Round(hit.point.z / gridScale) * gridScale
            );

            return snappedPosition;
        }

        return Vector3.zero;
    }
}
