using NaughtyAttributes;
using System;
using UnityEngine;

namespace RecycleFactory.Player
{
    [RequireComponent(typeof(PlayerCamera))]
    [RequireComponent(typeof(PlayerBuilder))]
    [RequireComponent(typeof(PlayerDemolisher))]
    public sealed class PlayerController : MonoBehaviour
    {
        public PlayerCamera playerCamera { get; private set; }
        public PlayerBuilder playerBuilder { get; private set; }
        public PlayerDemolisher playerDemolisher { get; private set; }

        [ShowNativeProperty] internal Mode mode { get; private set; }

        private Action updateFunction;

        public void Init()
        {
            playerCamera = GetComponent<PlayerCamera>();
            playerBuilder = GetComponent<PlayerBuilder>();
            playerDemolisher = GetComponent<PlayerDemolisher>();

            playerCamera.Init();
            playerBuilder.Init();
            playerDemolisher.Init();

            SetMode(Mode.None);
        }

        private void Update()
        {
            updateFunction();
        }

        /// <summary>
        /// Update function for Mode.None (selection, machine adjustment etc can be performed here)
        /// </summary>
        private void DefaultUpdate() { }

        internal Vector3 GetMouseWorldPosition()
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

        internal Vector2Int GetSelectedCell()
        {
            Vector3 position = Scripts.PlayerController.GetMouseWorldPosition();
            Vector2Int mapPos = new Vector2(position.x, position.z).FloorToInt();
            mapPos.Clamp(Vector2Int.zero, Map.mapSize - Vector2Int.one);
            return mapPos;
        }

        /// <summary>
        /// Used to loop through modes using GUI button
        /// </summary>
        public void IncrementMode()
        {
            SetMode((Mode)Mathf.Repeat((int)this.mode + 1, Enum.GetValues(typeof(Mode)).Length));
        }

        internal void SetMode(Mode mode)
        {
            this.mode = mode;

            switch (mode)
            {
                case Mode.None:
                    updateFunction = DefaultUpdate;
                    break;
                case Mode.Build:
                    updateFunction = playerBuilder._Update;
                    break;
                case Mode.Demolish:
                    updateFunction = playerDemolisher._Update;
                    break;
            }
        }
    }

    internal enum Mode
    {
        None,
        Build,
        Demolish
    }
}