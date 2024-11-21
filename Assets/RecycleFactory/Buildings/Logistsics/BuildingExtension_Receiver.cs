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

        private ConveyorBelt_Item GetLastFromAnyLane(ConveyorBelt_Driver driver, System.Func<ConveyorBelt_Item, bool> itemCheckFunction = null)
        {
            if (itemCheckFunction == null) itemCheckFunction = (ConveyorBelt_Item i) => true;
            for (int l = 0; l < ConveyorBelt_Driver.LANES_NUMBER; l++)
            {
                var lastNode = driver.lanes[l].Last;
                if (lastNode != null && itemCheckFunction(lastNode.Value) && driver.GetSignedDistanceToEnd(lastNode.Value) <= maxReceiveDistance)
                    return lastNode.Value;
            }
            return null;
        }

        private ConveyorBelt_Item GetFromAnyLane(ConveyorBelt_Driver driver, Vector3 targetPosition, float radius, System.Func<ConveyorBelt_Item, bool> itemCheckFunction = null)
        {
            if (itemCheckFunction == null) itemCheckFunction = (ConveyorBelt_Item i) => true;
            for (int l = 0; l < ConveyorBelt_Driver.LANES_NUMBER; l++)
            {
                foreach (var item in driver.lanes[l])
                {
                    if (Vector3.Distance(item.transform.position, targetPosition) < radius && itemCheckFunction(item))
                        return item;
                }
            }
            return null;
        }

        public bool CanReceive(int anchorIndex, out ConveyorBelt_Item item, System.Func<ConveyorBelt_Item, bool> itemCheckFunction = null)
        {
            var anchor = inAnchors[anchorIndex];
            item = null;
            if (anchor.conveyor == null) return false;
            if (itemCheckFunction == null) itemCheckFunction = (ConveyorBelt_Item i) => true;

            if (anchor.onlyDirectConnections)
            {
                // direct connection

                item = GetLastFromAnyLane(anchor.conveyor, itemCheckFunction);
                return item != null;
            }
            else
            {
                // indirect connection
                
                // calculate position of where to search for the item
                var pos = transform.position + anchor.localTilePosition.ConvertTo2D().ProjectTo3D() - anchor.direction.ConvertTo2D().ProjectTo3D();
                float radius = 0.4f;

                // find an item on any lane near that position
                item = GetFromAnyLane(anchor.conveyor, pos, radius, itemCheckFunction);
                return item != null;
            }
        }

        public void ForceReceive(ConveyorBelt_Item item)
        {
            item.Disable();
        }

        public bool TryReceive(int anchorIndex, out ConveyorBelt_ItemInfo itemInfo, System.Func<ConveyorBelt_Item, bool> itemCheckFunction = null)
        {
            if (CanReceive(anchorIndex, out ConveyorBelt_Item item, itemCheckFunction))
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