using NaughtyAttributes;
using UnityEngine;

namespace RecycleFactory.Buildings.Logistics
{
    /// <summary>
    /// Corresponds to an anchor leading towards a conveyor (conveyor-in-anchor) or towards a machine (conveyor-out-anchor)
    /// </summary>
    [System.Serializable]
    public class ConveyorBelt_Anchor
    {
        [ReadOnly] public ConveyorBelt_Driver conveyor;
        [ReadOnly] public Building machine;

        public float height;

        /// <summary>
        /// On an instantiated releaser/receiver it is automatically rotated according to the building rotation. Represents the tile at which the anchor is located relatively to the pivot point of the building.
        /// </summary>
        public Vector2Int localTilePosition;

        /// <summary>
        /// On an instantiated releaser/receiver it is automatically rotated according to the building rotation. Represents the direction of item flow. Must be inward for input and outward for output.
        /// </summary>
        public Vector2Int direction;

        /// <summary>
        /// Rotates the direction and revolves the position around (0, 0).
        /// Inputs delta values [-4; 4].
        /// </summary>
        public void Revolve(int delta)
        {
            direction = Utils.RotateXY(direction, delta);
            // TODO: add delta clamping

            localTilePosition = Utils.RotateXY(localTilePosition, delta);
        }

        public ConveyorBelt_Anchor GetRevolved(int delta)
        {
            ConveyorBelt_Anchor anchor = new ConveyorBelt_Anchor();
            anchor.direction = direction;
            anchor.localTilePosition = localTilePosition;
            anchor.Revolve(delta);
            return anchor;
        }

        public void ConnectToMachine(Building _machine)
        {
            machine = _machine;
        }

        public void ConnectToConveyor(ConveyorBelt_Driver _conveyor)
        {
            conveyor = _conveyor;
        }
    }
}
