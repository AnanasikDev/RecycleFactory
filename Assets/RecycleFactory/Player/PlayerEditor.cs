using RecycleFactory.Buildings;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RecycleFactory.Player
{
    public class PlayerEditor : MonoBehaviour
    {
        /// <summary>
        /// Whether player is moving or rotating an existing building
        /// </summary>
        private bool isEditing = false;

        private Func<bool> actionTrigger = () => Input.GetMouseButtonDown(0) && !UI.UIInputMask.isPointerOverUI;
        private Building activeBuilding = null;

        public void Init()
        {
            Scripts.PlayerController.onAfterModeChangedEvent += OnAfterModeChanged;
        }

        private void OnAfterModeChanged(Mode previousMode, Mode newMode)
        {
            if (newMode == Mode.Edit)
            {
                activeBuilding = null;
                isEditing = false;
            }
            else if (previousMode == Mode.Edit)
            {
                
            }
        }

        internal void Build()
        {
            // Place
            activeBuilding.Demolish();
            Scripts.PlayerBuilder.ForceBuild(AllBuildings.name2prefab[activeBuilding.name], Map.map2world(Scripts.PlayerBuilder.selectedCell), Scripts.PlayerBuilder.selectedRotation, Scripts.PlayerBuilder.selectedCell, updateBudget: false);
            isEditing = false;
            activeBuilding = null;
            Scripts.PlayerBuilder.isSelectedSpotAvailable = false;
            Scripts.PlayerBuilder.ResetSelection();
        }

        internal void Select()
        {
            activeBuilding = Map.getBuildingAt(Scripts.PlayerBuilder.selectedCell);
            if (activeBuilding != null)
            {
                Debug.Log(activeBuilding);
                activeBuilding.isAlive = false;
                activeBuilding.gameObject.SetActive(false);
                Map.RemoveBuilding(activeBuilding);
                Building.SimulateOnAnyDemolishedEvent();
                Scripts.PlayerBuilder.ForceSelectBuilding(AllBuildings.name2prefab[activeBuilding.name]);
                Scripts.PlayerBuilder.selectedRotation = activeBuilding.rotation;
                Scripts.PlayerBuilder.CheckSelectedSpot();
                Scripts.PlayerBuilder.UpdatePreview();

                isEditing = true;
            }
            else
                isEditing = false;
        }

        internal void Cancel()
        {
            isEditing = false;
            activeBuilding.isAlive = true;
            activeBuilding.gameObject.SetActive(true);
            Scripts.PlayerBuilder.ResetSelection();
            Map.RegisterNewBuilding(activeBuilding);
            activeBuilding = null;
        }

        internal void UpdateEditingMode()
        {
            if (actionTrigger())
            {
                Scripts.PlayerBuilder.HandleCellSelection();

                if (isEditing && activeBuilding != null)
                {
                    if (Scripts.PlayerBuilder.isSelectedSpotAvailable)
                    {
                        Build();
                    }
                }

                else
                {
                    Select();
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cancel();
            }

            if (isEditing && activeBuilding != null)
            {
                if (Scripts.PlayerBuilder.showPreview)
                {
                    Scripts.PlayerBuilder.HandleCellSelection();
                }

                Scripts.PlayerBuilder.HandleRotation();
                Scripts.PlayerBuilder.CheckSelectedSpot();
            }
        }
    }
}
