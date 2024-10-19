using NaughtyAttributes;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    /// <summary>
    /// Corresponds to an anchor leading towards a conveyor (conveyor-in-anchor) or towards a machine (conveyor-out-anchor)
    /// </summary>
    public class ConveyorAnchor
    {
        public ConveyorBelt_Element conveyor;
        public Building machine;

        public Vector2Int localTilePosition;
        public Vector2Int direction;

        /// <summary>
        /// Rotates the direction and revolves the position around (0, 0).
        /// Inputs delta values [-4; 4].
        /// </summary>
        public void Revolve(int delta)
        {
            direction = Utils.Rotate(direction, delta);
            // TODO: add delta clamping

            localTilePosition = Utils.Rotate(localTilePosition, delta);
        }

        public ConveyorAnchor GetRevolved(int delta)
        {
            ConveyorAnchor anchor = new ConveyorAnchor();
            anchor.direction = direction;
            anchor.localTilePosition = localTilePosition;
            anchor.Revolve(delta);
            return anchor;
        }

        public void ConnectToMachine(Building _machine)
        {
            machine = _machine;
        }

        public void ConnectToConveyor(ConveyorBelt_Element _conveyor)
        {
            conveyor = _conveyor;
        }
    }
}
