using DG.Tweening;
using NaughtyAttributes;
using RecycleFactory.Buildings;
using RecycleFactory.UI;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.LightTransport;
using static UnityEditor.PlayerSettings;

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

        [SerializeField][ReadOnly] private Building selectedBuildingPrefab;
        [SerializeField][ReadOnly] private int selectedBuildingIndex;
        [SerializeField][ReadOnly] private bool isSelectedSpotAvailable = false;
        [SerializeField][ReadOnly] private Vector2Int selectedCell;

        private Func<bool> placementTrigger = () => Input.GetMouseButtonDown(0) && !UIInputMask.isPointerOverUI;

        private event Action onCellSelectedEvent;
        public event Action<Building> onBuildEvent;
        public event Action onAnyBuildEvent;

        public void Init()
        {
            selectedBuildingPrefab = buildingsPrefabs[0];
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

        internal void _Update()
        {
            if (showPreview)
            {
                HandleCellSelection();
            }

            HandleRotation();
            HandleSelection();

            if (HandlePlacement())
            {
                // if built successfully, check new cell selection
                isSelectedSpotAvailable = false;
                UpdatePreview();
            }
        }

        private bool CheckSelectedSpot()
        {
            isSelectedSpotAvailable = Map.isSpaceFree(selectedCell, Utils.RotateXY(selectedBuildingPrefab.shift, selectedRotation), Utils.RotateXY(selectedBuildingPrefab.size, selectedRotation));
            return isSelectedSpotAvailable;
        }

        private void HandleCellSelection()
        {
            Vector2Int mapPos = Scripts.PlayerController.GetSelectedCell();

            if (mapPos != selectedCell)
            {
                selectedCell = mapPos;
                CheckSelectedSpot();

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
            selectedBuildingPrefab = buildingPrefab;
            selectedBuildingIndex = buildingsPrefabs.ToList().IndexOf(buildingPrefab);
            selectedRotation = 0;
            if (showPreview)
            {
                CheckSelectedSpot();
                UpdatePreview();
            }
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

        /// <summary>
        /// Places building at position with rotation; Building cost is subtracted from budget. No checks on budget sufficiency or placement eligibility is done
        /// </summary>
        private Building ForceBuild(Building selectedBuilding, Vector3 position, int selectedRotation, Vector2Int mapPos)
        {
            Building building = Instantiate(selectedBuilding, position, Quaternion.identity);
            building.Init(mapPos, selectedRotation);
            Scripts.Budget.Add(-selectedBuilding.cost);
            onBuildEvent?.Invoke(building);
            onAnyBuildEvent?.Invoke();
            return building;
        }

        private bool HandlePlacement()
        {
            if (placementTrigger() && selectedBuildingPrefab != null)
            {
                if (Scripts.Budget.amount < selectedBuildingPrefab.cost)
                {
                    Debug.LogWarning($"Cannot afford building {selectedBuildingPrefab.name} as it costs {selectedBuildingPrefab.cost} while there is only {Scripts.Budget.amount} left on account.");
                    return false;
                }

                // if preview is rendered then take the calculated values (used for the preview)
                if (showPreview)
                {
                    if (isSelectedSpotAvailable)
                    {
                        StartCoroutine(AnimateAndBuild(selectedBuildingPrefab, selectedCell.ConvertTo2D().ProjectTo3D().WithY(Map.floorHeight), selectedRotation, selectedCell));
                        return true;
                    }
                }
                else
                {
                    Vector3 position = Scripts.PlayerController.GetMouseWorldPosition();
                    selectedCell = new Vector2(position.x, position.z).FloorToInt();

                    if (CheckSelectedSpot())
                    {
                        StartCoroutine(AnimateAndBuild(selectedBuildingPrefab, position, selectedRotation, selectedCell));
                        return true;
                    }
                    else
                    {
                        Debug.Log("Building canceled: space is already taken.");
                        return false;
                    }
                }
            }

            return false;
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

                    UpdateArrowPreviews(previews[i]);
                }
                else
                {
                    previews[i].gameObject.SetActive(false);
                }
            }
        }

        private void UpdateArrowPreviews(BuildingPreview preview)
        {
            Scripts.BuildingArrowPreviewController.Display(preview.transform, selectedBuildingPrefab, selectedRotation);
        }


        private IEnumerator AnimateAndBuild(Building selectedBuilding, Vector3 position, int selectedRotation, Vector2Int mapPos)
        {
            Building building = ForceBuild(selectedBuilding, position, selectedRotation, mapPos);

            YieldInstruction animate(Building _building)
            {
                return DOTween.Sequence().Append(_building.buildingRenderer.meshFilter.transform.DOMoveY(0, 0.45f).From(0.3f).SetEase(Ease.OutBounce)).Join(Scripts.PlayerCamera.cameraHandler.transform.DOShakePosition(0.2f, 0.05f)).Play().WaitForCompletion();
            }
            yield return animate(building);
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;

            Vector3 position = Scripts.PlayerController.GetMouseWorldPosition();

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, 0.5f);
        }
    }
}