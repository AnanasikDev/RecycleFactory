using DG.Tweening;
using NaughtyAttributes;
using RecycleFactory.Buildings;
using RecycleFactory.UI;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace RecycleFactory.Player
{
    public class PlayerBuilder : MonoBehaviour
    {
        [SerializeField] internal bool showPreview = true;
        [SerializeField][ShowIf("showPreview")][ReadOnly] private BuildingPreview[] previews;
        [SerializeField][ShowIf("showPreview")] private Transform previewsHandler;
        [SerializeField][ShowIf("showPreview")] private Color preview_freeColor;
        [SerializeField][ShowIf("showPreview")] private Color preview_takenColor;

        [SerializeField] private bool allowChoosingUnavailableBuildings = true;
        [ShowNativeProperty] public int selectedRotation { get; internal set; }

        [SerializeField][ReadOnly] internal Building selectedBuildingPrefab;
        [SerializeField][ReadOnly] internal int selectedBuildingIndex;
        [SerializeField][ReadOnly] internal bool isSelectedSpotAvailable = false;
        [SerializeField][ReadOnly] internal Vector2Int selectedCell;

        [SerializeField] private Transform previewHandler;
        [SerializeField] private Material previewMaterial;

        private Func<bool> placementTrigger = () => Input.GetMouseButtonDown(0) && !UIInputMask.isPointerOverUI;

        private event Action onCellSelectedEvent;
        public event Action<Building> onBuildEvent;
        public event Action onAnyBuildEvent;

        public void Init()
        {
            Scripts.PlayerController.onAfterModeChangedEvent += OnAfterModeChanged;
            selectedBuildingPrefab = AllBuildings.allBuildings[0];
            InitPreview();

            onCellSelectedEvent += UpdatePreview;
        }

        private void InitPreview()
        {
            previews = new BuildingPreview[AllBuildings.allBuildings.Count];
            for (int i = 0; i < previews.Length; i++)
            {
                BuildingPreview preview = new GameObject("Building Preview " + i).AddComponent<BuildingPreview>();

                // mesh filter & renderer
                var renderer = new GameObject("mesh");
                renderer.transform.SetParent(preview.transform);
                renderer.transform.localPosition = AllBuildings.allBuildings[i].buildingRenderer.meshFilter.transform.localPosition;
                renderer.transform.localScale = AllBuildings.allBuildings[i].buildingRenderer.meshFilter.transform.localScale;
                var meshFilter = renderer.gameObject.AddComponent<MeshFilter>();
                var meshRenderer = renderer.gameObject.AddComponent<MeshRenderer>();

                // mesh copy
                Mesh newMesh = UnityEngine.Object.Instantiate(AllBuildings.allBuildings[i].buildingRenderer.meshFilter.sharedMesh);
                meshFilter.mesh = newMesh;
                meshRenderer.materials = AllBuildings.allBuildings[i].buildingRenderer.meshRenderer.sharedMaterials.Select(m => m = previewMaterial).ToArray();
                preview.meshFilter = meshFilter;
                preview.meshRenderer = meshRenderer;

                preview.gameObject.SetActive(false);
                preview.gameObject.transform.SetParent(previewHandler);
                previews[i] = preview;
            }
        }
        
        private void OnAfterModeChanged(Mode prevMode, Mode newMode)
        {
            if (newMode == Mode.Build)
            {
                SelectBuilding(0);
            }
            else
            {
                ResetSelection();
            }
        }

        public void ResetSelection()
        {
            selectedBuildingIndex = -1;
            selectedBuildingPrefab = null;
            UpdatePreview();
            Scripts.BuildingArrowPreviewController.HideAll();
        }

        internal void UpdateBuildingMode()
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

        internal bool CheckSelectedSpot()
        {
            isSelectedSpotAvailable = Map.isSpaceFree(selectedCell, Utils.RotateXY(selectedBuildingPrefab.shift, selectedRotation), Utils.RotateXY(selectedBuildingPrefab.size, selectedRotation));
            return isSelectedSpotAvailable;
        }

        internal void HandleCellSelection()
        {
            Vector2Int mapPos = Scripts.PlayerController.GetSelectedCellMapPos();

            if (mapPos != selectedCell)
            {
                selectedCell = mapPos;
                if (selectedBuildingIndex != -1) CheckSelectedSpot();

                onCellSelectedEvent?.Invoke();
            }
        }

        internal void HandleRotation()
        {
            bool inverse = false;

            if (Input.GetKey(KeyCode.LeftShift))
                inverse = true;

            if (Input.GetKeyDown(KeyCode.R))
            {
                int delta = inverse ? -1 : 1;

                selectedRotation = (int)Mathf.Repeat(selectedRotation + delta, 4);
                if (showPreview)
                {
                    CheckSelectedSpot();
                    UpdatePreview();
                }
                // if no preview needed, no need to check for selected spot availability before building.
            }
        }

        internal void HandleSelection()
        {
            for (int i = 0; i < Mathf.Min(AllBuildings.allBuildings.Count, 9); i++)
            {
                if (Input.GetKeyDown((i + 1).ToString()))
                {
                    if (allowChoosingUnavailableBuildings || Scripts.LevelController.IsUnlocked(AllBuildings.allBuildings[i]))
                    {
                        selectedBuildingIndex = i;
                        SelectBuilding(i);
                    }
                }
            }
        }

        public void ForceSelectBuilding(Building buildingPrefab)
        {
            selectedBuildingPrefab = buildingPrefab;
            selectedBuildingIndex = AllBuildings.allBuildings.ToList().IndexOf(buildingPrefab);
            selectedRotation = 0;
            if (showPreview)
            {
                CheckSelectedSpot();
                UpdatePreview();
            }
        }

        public void ForceSelectBuilding(int index)
        {
            ForceSelectBuilding(AllBuildings.allBuildings[index]);
        }

        internal void SelectBuilding(int index)
        {
            if (index >= 0 && index < AllBuildings.allBuildings.Count)
            {
                ForceSelectBuilding(index);
            }
        }

        /// <summary>
        /// Places building at position with rotation; Building cost is subtracted from budget. No checks on budget sufficiency or placement eligibility is done
        /// </summary>
        internal Building ForceBuild(Building selectedBuilding, Vector3 position, int selectedRotation, Vector2Int mapPos, bool updateBudget = true)
        {
            Building building = Instantiate(selectedBuilding, position, Quaternion.identity);
            building.Init(mapPos, selectedRotation);
            if (updateBudget) Scripts.Budget.Add(-selectedBuilding.cost);
            onBuildEvent?.Invoke(building);
            onAnyBuildEvent?.Invoke();
            return building;
        }

        internal bool HandlePlacement()
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
                    //Vector3 position = Scripts.PlayerController.GetMouseWorldPosition();
                    //selectedCell = new Vector2(position.x, position.z).FloorToInt();

                    Vector3 position = Scripts.PlayerController.GetSelectedCellMapPos().ConvertTo2D().ProjectTo3D();

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

        internal void UpdatePreview()
        {
            if (selectedCell == Map.invalidLocation)
            {
                Scripts.BuildingArrowPreviewController.HideAll();
            }

            for (int i = 0; i < previews.Length; i++)
            {
                // activate only the preview of the selected building
                if (i == selectedBuildingIndex && Map.isMapPosValid(selectedCell))
                {
                    previews[i].gameObject.SetActive(true);
                    previews[i].meshRenderer.materials.ToList().ForEach(mat => mat.color = isSelectedSpotAvailable ? preview_freeColor : preview_takenColor);
                    previews[i].transform.position = Map.map2world(selectedCell) + Vector3.up * Map.floorHeight;
                    previews[i].transform.eulerAngles = Vector3.up * selectedRotation * 90;

                    UpdateArrowPreviews(previews[i]);
                }
                else
                {
                    previews[i].gameObject.SetActive(false);
                }
            }
        }

        internal void UpdateArrowPreviews(BuildingPreview preview)
        {
            Scripts.BuildingArrowPreviewController.Display(preview.transform, selectedBuildingPrefab, selectedRotation);
        }


        internal IEnumerator AnimateAndBuild(Building selectedBuilding, Vector3 position, int selectedRotation, Vector2Int mapPos)
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