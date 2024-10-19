using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using RecycleFactory.Buildings;

namespace RecycleFactory.Player
{
    public class PlayerBuilder : MonoBehaviour
    {
        [Header("Building Settings")]
        [SerializeField] private List<Building> buildingsPrefabs;

        [ShowNativeProperty] public int selectedRotation { get; private set; }

        private Building currentBuilding;
        public void Init()
        {
            currentBuilding = buildingsPrefabs[0];
        }

        private void Update()
        {
            HandleRotation();
            HandleSelection();
            HandlePlacement();
        }

        private void HandleRotation()
        {
            if (Input.GetKey(KeyCode.R))
            {
                int r = Hexath.Ternarsign(Input.mouseScrollDelta.y);
                if (r != 0)
                    selectedRotation = (int)Mathf.Repeat(selectedRotation + r, 4);
            }
        }

        private void HandleSelection()
        {
            for (int i = 0; i < buildingsPrefabs.Count; i++)
            {
                if (Input.GetKeyDown((i + 1).ToString()))
                {
                    SelectBuilding(i);
                }
            }
        }

        private void SelectBuilding(int index)
        {
            if (index >= 0 && index < buildingsPrefabs.Count)
            {
                currentBuilding = buildingsPrefabs[index];
                selectedRotation = 0;
                Debug.Log($"Selected building: {currentBuilding.name}");
            }
        }

        private void HandlePlacement()
        {
            if (Input.GetMouseButtonDown(0) && currentBuilding != null)
            {
                Vector3 position = GetMouseWorldPosition();
                Vector2Int mapPos = new Vector2(position.x, position.z).FloorToInt();

                Debug.Log(mapPos);

                Building building = Instantiate(currentBuilding, position, Quaternion.identity);
                building.Init(mapPos);
                building.Rotate(selectedRotation);
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 snappedPosition = new Vector3(
                    Mathf.Round(hit.point.x / Map.cellScale) * Map.cellScale,
                    0f, // ground
                    Mathf.Round(hit.point.z / Map.cellScale) * Map.cellScale
                );

                return snappedPosition;
            }

            return Vector3.zero;
        }

        private void OnDrawGizmos()
        {
            Vector3 position = GetMouseWorldPosition();

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, 0.5f);
        }
    }
}