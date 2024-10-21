using System.Collections.Generic;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    public class BuildingExtension_Releaser : MonoBehaviour
    {
        private Building building;
        [Tooltip("Must be set in inspector")] public List<ConveyorAnchor> outAnchors;

        public void Init(Building building)
        {
            this.building = building;
            foreach (var outAnchor in outAnchors)
            {
                outAnchor.conveyor = null;
                outAnchor.machine = building;
            }
            Building.onAnyBuiltEvent += TryConnect;
        }

        private void OnDestroy()
        {
            Building.onAnyBuiltEvent -= TryConnect;
        }

        public void Rotate(int delta)
        {
            foreach (var outAnchor in outAnchors)
            {
                outAnchor.Revolve(delta);
            }
        }

        public bool IsFreeToRelease(int outAnchorIndex)
        {
            return outAnchors[outAnchorIndex].conveyor != null && outAnchors[outAnchorIndex].conveyor.isEmpty;
        }

        public void ForceRelease(ConveyorBelt_Item item, int outAnchorIndex)
        {
            outAnchors[outAnchorIndex].conveyor.SetItem(item);
        }

        public bool TryReleaseAt(ConveyorBelt_Item item, int outAnchorIndex)
        {
            if (!IsFreeToRelease(outAnchorIndex)) return false;

            ForceRelease(item, outAnchorIndex);

            return true;
        }

        /// <summary>
        /// Updates connections on all outAnchors
        /// </summary>
        private void TryConnect()
        {
            for (int i = 0; i < outAnchors.Count; i++)
            {
                Building otherBuilding = Map.getBuildingAt(building.mapPosition + outAnchors[i].localTilePosition + outAnchors[i].direction);
                if (otherBuilding == null) return;
                if (otherBuilding.TryGetComponent(out ConveyorBelt_Building otherConveyor))
                {
                    // TODO: calculate the closest element of otherConveyor (when merging it is not first)
                    outAnchors[i].conveyor = otherConveyor.first;
                }
                else
                {
                    outAnchors[i].conveyor = null;
                }
            }
        }
    }
}