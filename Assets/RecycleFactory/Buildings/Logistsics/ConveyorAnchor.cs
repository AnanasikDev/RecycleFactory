﻿using NaughtyAttributes;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    /// <summary>
    /// Corresponds to an anchor leading towards a conveyor (conveyor-in-anchor) or towards a machine (conveyor-out-anchor)
    /// </summary>
    public class ConveyorAnchor : MonoBehaviour
    {
        public Vector2 localTilePosition;
        public Vector2Int direction;

        /// <summary>
        /// Rotates the direction and revolves the position around (0, 0)
        /// </summary>
        public void Revolve(int delta)
        {
            direction = Utils.Rotate(direction, delta);

            for (int i = 0; i < Mathf.Abs(delta); i++)
            {
                localTilePosition = new Vector2(
                    -localTilePosition.y,
                    localTilePosition.x
                );
            }
        }

        public ConveyorAnchor GetRevolved(int delta)
        {
            ConveyorAnchor anchor = new ConveyorAnchor();
            anchor.direction = direction;
            anchor.localTilePosition = localTilePosition;
            anchor.Revolve(delta);
            return anchor;
        }
    }
}
