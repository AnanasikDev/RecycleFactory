using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Lane = System.Collections.Generic.LinkedList<RecycleFactory.Buildings.ConveyorBelt_Item>;

namespace RecycleFactory.Buildings
{
    [Serializable]
    public class ConveyorBelt_Driver
    {
        public static readonly int lanesNumber = 3;
        public readonly Lane[] lanes = new Lane[lanesNumber];

        private ConveyorBelt_Building conveyorBuilding;
        public ConveyorBelt_Driver next;
        public float minItemDistance = 0.3f;
        [ShowNativeProperty] public Vector2 direction { get ; private set; }

        public void Init(ConveyorBelt_Building building)
        {
            this.conveyorBuilding = building;
            direction = building.moveDirectionClamped.ConvertTo2D();
            // init empty lanes
        }

        public void Update()
        {
            MoveAllItems();
        }

        private void MoveAllItems()
        {
            // for each item check distance to the next one, translate if possible, halt movement if not
            for (int l = 0; l < lanesNumber; l++)
            {
                LinkedListNode<ConveyorBelt_Item> currentNode = lanes.ElementAt(l).First;
                while (currentNode != null)
                {
                    ConveyorBelt_Item item = currentNode.Value;

                    currentNode = currentNode.Next;

                    if (GetStraightDistance(item, currentNode.Value) > minItemDistance)
                    {
                        item.transform.Translate(direction * conveyorBuilding.speed);
                    }
                }
            }

            // if merging to another conveyor straightly reparent
            // if merging to another conveyor at 90 degrees choose a random lane, calculate if there is space in according to travel time, translate or halt
        }

        private float GetStraightDistance(ConveyorBelt_Item item1, ConveyorBelt_Item item2)
        {
            return ((item1.transform.position - item2.transform.position) * this.direction).magnitude;
        }

        private float GetOrthogonalDistance(ConveyorBelt_Item item1, ConveyorBelt_Item item2)
        {
            return ((item1.transform.position - item2.transform.position) * next.direction).magnitude;
        }

        private float GetDistanceFromStart(ConveyorBelt_Item item)
        {
            return ((item.transform.position - conveyorBuilding.localStartPivot.transform.position) * next.direction).magnitude;
        }

        private float GetDistanceToEnd(ConveyorBelt_Item item)
        {
            return ((item.transform.position - conveyorBuilding.localEndPivot.transform.position) * next.direction).magnitude;
        }

        private int ChooseLane(ConveyorBelt_Item targetItem)
        {
            for (int l = 0; l < lanesNumber; l++)
            {
                bool isLaneObscured = false;
                foreach (var item in lanes[l])
                {
                    // possible to move to that lane only if there is enough space between
                    if (GetOrthogonalDistance(item, targetItem) < GetStraightDistance(item, targetItem) + minItemDistance)
                    {
                        // at least one item is too close, choose next lane (MIGHT BE AN ITEM IN FRONT THOUGH)
                        isLaneObscured = true;
                        break;
                    }
                }
                if (!isLaneObscured)
                {
                    return l;
                }
            }

            return -1; // no lane available, wait
        }

        public void TryFindNext()
        {
            Building otherBuilding = Map.getBuildingAt(conveyorBuilding.mapPosition + conveyorBuilding.moveDirectionClamped * conveyorBuilding.lengthTiles);
            if (otherBuilding == null) return;
            if (otherBuilding.TryGetComponent(out ConveyorBelt_Building otherConveyor))
            {
                next = conveyorBuilding.driver;
            }
            else
            {
                next = null;
            }
        }

        /// <summary>
        /// Returns true if item can be added to the start of the driver as the first item
        /// </summary>
        /// <returns></returns>
        public bool CanEnqueueItem()
        {
            foreach (var lane in lanes)
                if (GetDistanceFromStart(lane.First.Value) > minItemDistance) return true;
            return false;
        }

        /// <summary>
        /// Returns true if item can be added to the start of the driver as the first item
        /// </summary>
        /// <returns></returns>
        public bool CanEnqueueItem(Lane lane)
        {
            return GetDistanceFromStart(lane.First.Value) > minItemDistance;
        }


        /// <summary>
        /// As from a releaser, this function adds an item to an arbitrary lane
        /// </summary>
        public bool AddItem(ConveyorBelt_ItemInfo itemInfo)
        {
            var item = ConveyorBelt_Item.Create(itemInfo);
            foreach (var lane in lanes)
            {
                if (CanEnqueueItem(lane))
                {
                    lane.AddFirst(item);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// As from a releaser, this function adds an item to an arbitrary lane
        /// </summary>
        public bool AddItem(ConveyorBelt_Item item)
        {
            foreach (var lane in lanes)
            {
                if (CanEnqueueItem(lane))
                {
                    lane.AddFirst(item);
                    return true;
                }
            }
            return false;
        }
    }
}
