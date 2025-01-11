using NaughtyAttributes;
using System;
using UnityEngine;

namespace RecycleFactory.Player
{
    [RequireComponent(typeof(PlayerCamera))]
    [RequireComponent(typeof(PlayerBuilder))]
    [RequireComponent(typeof(PlayerDemolisher))]
    [RequireComponent(typeof(PlayerEditor))]
    public sealed class PlayerController : MonoBehaviour
    {
        public PlayerCamera playerCamera { get; private set; }
        public PlayerBuilder playerBuilder { get; private set; }
        public PlayerDemolisher playerDemolisher { get; private set; }
        public PlayerEditor playerEditor { get; private set; }

        [ShowNativeProperty] internal Mode mode { get; private set; }
        internal event Action<Mode, Mode> onAfterModeChangedEvent;

        private Action updateFunction;

        public void Init()
        {
            playerCamera = GetComponent<PlayerCamera>();
            playerBuilder = GetComponent<PlayerBuilder>();
            playerDemolisher = GetComponent<PlayerDemolisher>();
            playerEditor = GetComponent<PlayerEditor>();

            playerCamera.Init();
            playerBuilder.Init();
            playerDemolisher.Init();
            playerEditor.Init();

            SetMode(Mode.Build);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                IncrementMode();
                return;
            }

            updateFunction();
        }

        internal Vector3 GetMouseWorldPosition()
        {
            Ray ray = playerCamera.cameraHandler.ScreenPointToRay(Input.mousePosition);
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

            return Map.invalidLocation.ConvertTo2D().ProjectTo3D();
        }

        internal Vector2Int GetSelectedCellMapPos()
        {
            Vector3 position = Scripts.PlayerController.GetMouseWorldPosition();
            Vector2Int mapPos = Map.world2map(position);
            if (Map.isMapPosValid(mapPos))
            {
                return mapPos;
            }
            return Map.invalidLocation;
        }

        /// <summary>
        /// Used to loop through modes using GUI button
        /// </summary>
        public void IncrementMode()
        {
            SetMode((Mode)Mathf.Repeat((int)this.mode + 1, Enum.GetValues(typeof(Mode)).Length));
        }

        public void SetMode(Mode mode)
        {
            switch (mode)
            {
                case Mode.Edit:
                    updateFunction = playerEditor.UpdateEditingMode;
                    break;
                case Mode.Build:
                    updateFunction = playerBuilder.UpdateBuildingMode;
                    break;
                case Mode.Demolish:
                    updateFunction = playerDemolisher._Update;
                    break;
            }
            Mode before = this.mode;
            this.mode = mode;

            if (before != mode)
            {
                Scripts.PlayerModeSwitch.UpdateModeIcon();
                onAfterModeChangedEvent?.Invoke(before, mode);
            }
        }
    }

    public enum Mode
    {
        Edit,
        Build,
        Demolish
    }
}