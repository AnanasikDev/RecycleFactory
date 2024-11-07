using NaughtyAttributes;
using RecycleFactory.Buildings;
using System;
using System.Linq;
using UnityEngine;

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

        [ShowNativeProperty] public PlayerBuilderMode mode { get; private set; }

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
                BuildingPreview preview = new GameObject("Building Preview " + i).AddComponent<BuildingPreview>();

                // mesh filter & renderer
                var renderer = new GameObject("mesh");
                renderer.transform.SetParent(preview.transform);
                renderer.transform.localPosition = buildingsPrefabs[i].buildingRenderer.meshFilter.transform.localPosition;
                renderer.transform.localScale = buildingsPrefabs[i].buildingRenderer.meshFilter.transform.localScale;
                var meshFilter = renderer.gameObject.AddComponent<MeshFilter>();
                var meshRenderer = renderer.gameObject.AddComponent<MeshRenderer>();

                // mesh copy
                Mesh newMesh = UnityEngine.Object.Instantiate(buildingsPrefabs[i].buildingRenderer.meshFilter.sharedMesh);
                meshFilter.mesh = newMesh;
                meshRenderer.materials = buildingsPrefabs[i].buildingRenderer.meshRenderer.sharedMaterials;
                preview.meshFilter = meshFilter;
                preview.meshRenderer = meshRenderer;

                preview.gameObject.SetActive(true);
                previews[i] = preview;
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

        public void ForceSelectBuilding(Building buildingPrefab)
        {
            selectedBuilding = buildingPrefab;
            selectedBuildingIndex = buildingsPrefabs.ToList().IndexOf(buildingPrefab);
            selectedRotation = 0;
            Debug.Log($"Selected building: {selectedBuilding.name}");
            if (showPreview) UpdatePreview();
        }

        public void ForceSelectBuilding(int index)
        {
            ForceSelectBuilding(buildingsPrefabs[index]);
        }

        private void SelectBuilding(int index)
        {
            if (index >= 0 && index < buildingsPrefabs.Length)
            {
                ForceSelectBuilding(index);
            }
        }

        private void ForceBuild(Building selectedBuilding, Vector3 position, int selectedRotation, Vector2Int mapPos)
        {
            Building building = Instantiate(selectedBuilding, position, Quaternion.identity);
            building.Rotate(selectedRotation);
            building.Init(mapPos);
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
                        ForceBuild(selectedBuilding, selectedCell.ConvertTo2D().ProjectTo3D().WithY(Map.floorHeight), selectedRotation, selectedCell);
                    }
                }
                else
                {
                    Vector3 position = GetMouseWorldPosition();
                    Vector2Int mapPos = new Vector2(position.x, position.z).FloorToInt();

                    if (Map.isSpaceFree(mapPos, Utils.RotateXY(selectedBuilding.size, selectedRotation)))
                    {
                        ForceBuild(selectedBuilding, position, selectedRotation, mapPos);
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

        public void SetBuildingMode(PlayerBuilderMode mode)
        {
            this.mode = mode;
        }

        private void OnDrawGizmos()
        {
            Vector3 position = GetMouseWorldPosition();

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, 0.5f);
        }
    }

    public enum PlayerBuilderMode
    {
        Build,
        Destroy
    }
}