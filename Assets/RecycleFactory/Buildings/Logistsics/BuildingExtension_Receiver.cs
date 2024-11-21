using RecycleFactory.Buildings.Logistics;
using System.Collections.Generic;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    public class BuildingExtension_Receiver : MonoBehaviour
    {
        private Building building;
        public float height;
        [Tooltip("Must be set in inspector")] public List<ConveyorBelt_Anchor> inAnchors;
        [SerializeField] private float maxReceiveDistance = 0.35f;

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
            Building.onAnyBuiltEvent += UpdateAnchorsConnections;
            Building.onAnyDemolishedEvent += UpdateAnchorsConnections;
        }

        private void OnDestroy()
        {
            Building.onAnyBuiltEvent -= UpdateAnchorsConnections;
            Building.onAnyDemolishedEvent -= UpdateAnchorsConnections;
        }

        private ConveyorBelt_Item GetLastFromAnyLane(ConveyorBelt_Driver driver)
        {
            for (int l = 0; l < ConveyorBelt_Driver.LANES_NUMBER; l++)
            {
                var lastNode = driver.lanes[l].Last;
                if (lastNode != null && driver.GetSignedDistanceToEnd(lastNode.Value) <= maxReceiveDistance)
                    return lastNode.Value;
            }
            return null;
        }

        public bool CanReceive(int anchorIndex, out ConveyorBelt_Item item)
        {
            var anchor = inAnchors[anchorIndex];
            item = null;
            if (anchor.conveyor == null) return false;

            if (anchor.onlyDirectConnections)
            {
                // direct connection

                item = GetLastFromAnyLane(anchor.conveyor);
                if (item == null) return false;
                return true;
            }
            else
            {
                // indirect connection

                // try item which is close enough
            }

            return false;
        }

        public void ForceReceive(ConveyorBelt_Item item)
        {
            item.Disable();
        }

        public bool TryReceive(int anchorIndex, out ConveyorBelt_ItemInfo itemInfo)
        {
            if (CanReceive(anchorIndex, out ConveyorBelt_Item item))
            {
                itemInfo = item.info;
                ForceReceive(item);
                return true;
            }

            itemInfo = null;
            return false;
        }

        private void UpdateAnchorsConnections()
        {
            for (int i = 0; i < inAnchors.Count; i++)
            {
                Building otherBuilding = Map.getBuildingAt(building.mapPosition + inAnchors[i].localTilePosition - inAnchors[i].direction);

                if (otherBuilding == null)
                {
                    inAnchors[i].conveyor = null;
                    continue;
                }

                if (otherBuilding.TryGetComponent(out ConveyorBelt_Building otherConveyor))
                {
                    if ((inAnchors[i].onlyDirectConnections && otherConveyor.moveDirectionClamped == inAnchors[i].direction) ||
                        (!inAnchors[i].onlyDirectConnections && otherConveyor.moveDirectionClamped != -inAnchors[i].direction))
                    {
                        inAnchors[i].conveyor = otherConveyor.driver;
                        continue;
                    }
                }

                inAnchors[i].conveyor = null;
            }
        }
    }
}