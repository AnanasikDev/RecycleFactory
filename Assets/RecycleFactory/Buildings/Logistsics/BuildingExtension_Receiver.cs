using UnityEngine;
using System.Collections.Generic;
using RecycleFactory.Buildings.Logistics;

namespace RecycleFactory.Buildings
{
    public class BuildingExtension_Receiver : MonoBehaviour
    {
        private Building building;
        public float height;
        [Tooltip("Must be set in inspector")] public List<ConveyorBelt_Anchor> inAnchors;

        public void Init(Building building, int rotation)
        {
            this.building = building;
            foreach (var inAnchor in inAnchors)
            {
                inAnchor.conveyor = null;
                inAnchor.machine = building;
                inAnchor.height = height;
                inAnchor.Revolve(rotation);
            }
            Building.onAnyBuiltEvent += TryConnect;
            Building.onAnyDemolishedEvent += TryConnect;
        }

        private void OnDestroy()
        {
            Building.onAnyBuiltEvent -= TryConnect;
            Building.onAnyDemolishedEvent -= TryConnect;
        }

        public ConveyorBelt_ItemInfo Receive(ConveyorBelt_Item item, int anchorIndex)
        {
            item.Disable();
            return item.info;
        }

        public ConveyorBelt_ItemInfo ReceiveLast(int anchorIndex, int lane = -1)
        {
            if (lane != -1)
            {
                return inAnchors[anchorIndex].conveyor.lanes[lane].Last?.Value.info;
            }
            return null;
        }

        /// <summary>
        /// Updates connections on all outAnchors
        /// </summary>
        private void TryConnect()
        {
            for (int i = 0; i < inAnchors.Count; i++)
            {
                Building otherBuilding = Map.getBuildingAt(building.mapPosition + inAnchors[i].localTilePosition + inAnchors[i].direction);

                if (otherBuilding == null || !otherBuilding.TryGetComponent(out ConveyorBelt_Building otherConveyor))
                {
                    inAnchors[i].conveyor = null;
                }
                else
                {
                    inAnchors[i].conveyor = otherConveyor.driver;
                }
            }
        }
    }
}