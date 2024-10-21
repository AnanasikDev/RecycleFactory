using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;
using RecycleFactory.Buildings;
using System.Linq;
using System;

namespace RecycleFactory.Player
{
    public class PlayerBuilder : MonoBehaviour
    {
        [Header("Building Settings")]
        [SerializeField] private Building[] buildingsPrefabs;

        [SerializeField] private bool showPreview = true;
        [SerializeField][ShowIf("showPreview")][ReadOnly] private BuildingPreview[] previews;
        [SerializeField][ShowIf("showPreview")] private Transform previewsHandler;
        [SerializeField][ShowIf("showPreview")] private Color preview_freeColor;
        [SerializeField][ShowIf("showPreview")] private Color preview_takenColor;

        [ShowNativeProperty] public int selectedRotation { get; private set; }

        [SerializeField][ReadOnly] private Building selectedBuilding;
        [SerializeField][ReadOnly] private int selectedBuildingIndex;
        [SerializeField][ReadOnly] private bool isSelectedSpotAvailable = false;
        [SerializeField][ReadOnly] private Vector2Int selectedCell;

        private event Action onCellSelectedEvent;

        public void Init()
        {
            selectedBuilding = buildingsPrefabs[0];
            InitPreview();

            onCellSelectedEvent += UpdatePreview;
        }

        private void InitPreview()
        {
            previews = new BuildingPreview[buildingsPrefabs.Length];
            for (int i = 0; i < previews.Length; i++)
            {
                Building preview_building = Instantiate(buildingsPrefabs[i], Vector3.zero, Quaternion.identity, previewsHandler);
                previews[i] = preview_building.gameObject.AddComponent<BuildingPreview>();

                previews[i].meshFilter = preview_building.meshFilter;
                previews[i].meshRenderer = preview_building.meshRenderer;
                previews[i].gameObject.SetActive(true);
                Destroy(preview_building); // remove Building component from the preview
            }
        }

        private void Update()
        {
            if (showPreview)
            {
                HandleCellSelection();
            }

            HandleRotation();
            HandleSelection();
            HandlePlacement();
        }

        private void HandleCellSelection()
        {
            Vector2Int prevCell = selectedCell;
            Vector3 position = GetMouseWorldPosition();
            Vector2Int mapPos = new Vector2(position.x, position.z).FloorToInt();
            mapPos.Clamp(Vector2Int.zero, Map.mapSize - Vector2Int.one);
            selectedCell = mapPos;
            if (prevCell != selectedCell)
            {
                isSelectedSpotAvailable = Map.isSpaceFree(mapPos, Utils.RotateXY(selectedBuilding.size, selectedRotation));
                onCellSelectedEvent?.Invoke();
            }
        }

        private void HandleRotation()
        {
            if (Input.GetKey(KeyCode.R))
            {
                int r = Hexath.Ternarsign(Input.mouseScrollDelta.y);
                if (r != 0)
                {
                    selectedRotation = (int)Mathf.Repeat(selectedRotation + r, 4);
                    if (showPreview) UpdatePreview();
                }
            }
        }

        private void HandleSelection()
        {
            for (int i = 0; i < buildingsPrefabs.Length; i++)
            {
                if (Input.GetKeyDown((i + 1).ToString()))
                {
                    selectedBuildingIndex = i;
                    SelectBuilding(i);
                }
            }
        }

        private void SelectBuilding(int index)
        {
            if (index >= 0 && index < buildingsPrefabs.Length)
            {
                selectedBuilding = buildingsPrefabs[index];
                selectedRotation = 0;
                Debug.Log($"Selected building: {selectedBuilding.name}");
                if (showPreview) UpdatePreview();
            }
        }

        private void HandlePlacement()
        {
            if (Input.GetMouseButtonDown(0) && selectedBuilding != null)
            {
                // if preview is rendered then take the calculated values (used for the preview)
                if (showPreview)
                {
                    if (isSelectedSpotAvailable)
                    {
                        Building building = Instantiate(selectedBuilding, selectedCell.ConvertTo2D().ProjectTo3D().WithY(Map.floorHeight), Quaternion.identity);
                        building.Rotate(selectedRotation);
                        building.Init(selectedCell);
                    }
                }
                else
                {
                    Vector3 position = GetMouseWorldPosition();
                    Vector2Int mapPos = new Vector2(position.x, position.z).FloorToInt();

                    if (Map.isSpaceFree(mapPos, Utils.RotateXY(selectedBuilding.size, selectedRotation)))
                    {
                        Building building = Instantiate(selectedBuilding, position, Quaternion.identity);
                        building.Rotate(selectedRotation);
                        building.Init(mapPos);
                    }
                    else
                    {
                        Debug.Log("Building canceled: space is already taken.");
                    }
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
                    Hexath.SnapNumberToStep(hit.point.x, 1),
                    Map.floorHeight,
                    Hexath.SnapNumberToStep(hit.point.z, 1)
                );

                return snappedPosition;
            }

            return Vector3.zero;
        }

        private void UpdatePreview()
        {
            // TODO: add toggle on/off

            for (int i = 0; i < previews.Length; i++)
            {
                // activate only the preview of the selected building
                if (i == selectedBuildingIndex)
                {
                    previews[i].gameObject.SetActive(true);
                    previews[i].meshRenderer.materials.ToList().ForEach(mat => mat.color = isSelectedSpotAvailable ? preview_freeColor : preview_takenColor);
                    previews[i].transform.position = selectedCell.ConvertTo2D().ProjectTo3D(Map.floorHeight);
                    previews[i].transform.eulerAngles = Vector3.up * selectedRotation * 90;
                }
                else
                {
                    previews[i].gameObject.SetActive(false);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Vector3 position = GetMouseWorldPosition();

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, 0.5f);
        }
    }
}