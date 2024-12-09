using EasyDebug.Prompts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ItemNode = System.Collections.Generic.LinkedListNode<RecycleFactory.Buildings.Logistics.ConveyorBelt_Item>;
using Lane = System.Collections.Generic.LinkedList<RecycleFactory.Buildings.Logistics.ConveyorBelt_Item>;

namespace RecycleFactory.Buildings.Logistics
{
    public class ConveyorBelt_Driver
    {
        internal static readonly int INF = 100000;
        public static readonly int LANES_NUMBER = 3;
        internal readonly Lane[] lanes = new Lane[LANES_NUMBER];
        internal List<ConveyorBelt_Item> allItemsReadonly { get { return lanes.SelectMany(lane => lane).ToList(); } }

        internal ConveyorBelt_Building conveyorBuilding { get; init; }
        internal ConveyorBelt_Driver nextDriver;
        public readonly float minItemDistance = 0.4f;
        public readonly float minEndDistance = 0.3f;
        public readonly float transferEndDistance = 0.0f;

        internal bool isAlive = true;

        public Vector3 direction { get; init; }
        public Vector3 velocity { get { return direction * conveyorBuilding.lengthTiles / conveyorBuilding.transportTimeSeconds; } }

        public ConveyorBelt_Driver(ConveyorBelt_Building conveyorBuilding)
        {
            isAlive = true;
            this.conveyorBuilding = conveyorBuilding;
            direction = conveyorBuilding.moveDirectionClamped.ConvertTo2D().ProjectTo3D();
            Debug.Log("New Driver created with direction = " + direction);

            // init empty lanes
            for (int laneIndex = 0; laneIndex < LANES_NUMBER; laneIndex++)
            {
                lanes[laneIndex] = new Lane();
            }
        }

        public void Destroy()
        {
            if (lanes == null) return;
            for (int l = 0; l < LANES_NUMBER; l++)
            {
                if (lanes[l] == null || lanes[l].Count == 0) continue;
                for (ItemNode itemNode = lanes[l].First; itemNode != null; itemNode = itemNode.Next)
                {
                    // if item hasn't been destroyed
                    if (itemNode.Value != null)
                        // disable item in pool for later use
                        itemNode.Value.gameObject.SetActive(false);
                }
                lanes[l].Clear();
            }
            isAlive = false;
        }

        public void Update()
        {
            MoveAllItems();
        }

        private void MoveAllItems()
        {
            if (!isAlive) return;

            // for each item check distance to the next one, translate if possible, halt movement if not
            for (int laneIndex = 0; laneIndex < LANES_NUMBER; laneIndex++)
            {
                Lane lane = lanes[laneIndex];
                if (lane.Count == 0) continue;

                for (ItemNode itemNode = lane.First; itemNode != null; itemNode = itemNode.Next)
                {
                    ConveyorBelt_Item item = itemNode.Value;

                    float distToEnd = GetSignedDistanceToEnd(item);
                    TextPromptManager.UpdateText(item.gameObject, "distToEnd", "e " + distToEnd.ToString("n2"));
                    TextPromptManager.UpdateText(item.gameObject, "distToNext", "n " + (itemNode.Next == null ? INF : GetStraightDistance(item, itemNode.Next.Value)).ToString("n2"));
                    TextPromptManager.UpdateText(item.gameObject, "name", item.name);
                    //TextPromptManager.UpdateText(item.gameObject, "next", itemNode.Next?.Value?.name ?? "null");

                    // there is next driver connected

                    if (distToEnd <= transferEndDistance)
                    {
                        if (nextDriver == null || !nextDriver.isAlive)
                        {
                            // halt
                            continue;
                        }

                        // next driver has same direction, check if its first item is far enough
                        if (nextDriver.lanes[item.currentLaneIndex].First == null || GetStraightDistance(item, nextDriver.lanes[item.currentLaneIndex].First.Value) > minItemDistance)
                        {
                            // there is either no items in the next conveyor or its first item is far enough
                            nextDriver.TakeOwnershipStraight(this, itemNode);
                        }
                        continue;
                    }

                    if (distToEnd <= minEndDistance)
                    {
                        if (nextDriver == null || !nextDriver.isAlive)
                        {
                            // halt
                            continue;
                        }

                        if (IsOrthogonalTo(nextDriver))
                        {
                            // too far

                            int targetLaneIndex = ChooseOrthogonalLane(item); // find a lane where the item can go right now
                            if (targetLaneIndex == -1)
                            {
                                // halt
                            }
                            else
                            {
                                nextDriver.TakeOwnershipOrthogonal(this, itemNode, targetLaneIndex);
                            }
                        }
                        else
                        {
                            if (nextDriver.lanes[item.currentLaneIndex].First == null || GetStraightDistance(item, nextDriver.lanes[item.currentLaneIndex].First.Value) > minItemDistance)
                            {
                                item.transform.Translate(velocity * Time.deltaTime); // move item
                            }
                        }
                        continue;
                    }

                    if (itemNode.Next == null || GetStraightDistance(item, itemNode.Next.Value) > minItemDistance)
                    {
                        item.transform.Translate(velocity * Time.deltaTime); // move item
                    }
                }
            }
        }

        /// <summary> 
        /// Takes ownership of the item, becoming responsible of its visibility, transition, deletion and processing.
        /// </summary>
        public void TakeOwnershipOrthogonal(ConveyorBelt_Driver oldOwner, ItemNode item, int targetLaneIndex = -1)
        {
            targetLaneIndex = targetLaneIndex == -1 ? item.Value.currentLaneIndex : targetLaneIndex;

            oldOwner.lanes[item.Value.currentLaneIndex].Remove(item);

            ItemNode nextItem = null;
            float minDistance = INF;
            for (ItemNode itemNode = lanes[targetLaneIndex].First; itemNode != null; itemNode = itemNode.Next)
            {
                float distance = oldOwner.GetOrthogonalDistance(item.Value, itemNode.Value);
                if (distance < minDistance && oldOwner.IsOrthogonalItemAhead(item.Value, itemNode.Value))
                {
                    minDistance = distance;
                    nextItem = itemNode;
                }
            }
            //nextItem = lanes[targetLaneIndex].First;

            TextPromptManager.UpdateText(item.Value.gameObject, "next", nextItem?.Value?.name ?? "null");


            this.AddToLaneBeforeNext(targetLaneIndex, item, nextItem);
        }

        public void TakeOwnershipStraight(ConveyorBelt_Driver oldOwner, ItemNode item)
        {
            int lane = item.Value.currentLaneIndex;
            oldOwner.lanes[lane].Remove(item.Value);
            this.AddToLaneBeforeNext(lane, item, lanes[lane].First);
        }

        internal float GetStraightDistance(ConveyorBelt_Item item1, ConveyorBelt_Item item2)
        {
            return item1 == null || item2 == null ? INF : (item1.transform.position - item2.transform.position).Multiply(this.direction).magnitude;
        }
        
        internal float GetOrthogonalDistance(ConveyorBelt_Item thisItem, ConveyorBelt_Item nextItem)
        {
            return thisItem == null || nextItem == null ? INF : 
                (thisItem.transform.position - nextItem.transform.position).Multiply(nextDriver.direction).magnitude;
        }
        
        internal float GetSignedDistanceFromStart(ConveyorBelt_Item item)
        {
            if (item == null) return INF;
            if (direction.x < 0)
                return conveyorBuilding.startPivot.transform.position.x - item.transform.position.x;
            if (direction.x > 0)
                return item.transform.position.x - conveyorBuilding.startPivot.transform.position.x;
            if (direction.z < 0)
                return conveyorBuilding.startPivot.transform.position.z - item.transform.position.z;
            if (direction.z > 0)
                return item.transform.position.z - conveyorBuilding.startPivot.transform.position.z;
            return INF;
        }
        
        internal float GetSignedDistanceToEnd(ConveyorBelt_Item item)
        {
            if (item == null) return INF;
            if (direction.x > 0)
                return conveyorBuilding.endPivot.transform.position.x - item.transform.position.x;
            if (direction.x < 0)
                return item.transform.position.x - conveyorBuilding.endPivot.transform.position.x;
            if (direction.z > 0)
                return conveyorBuilding.endPivot.transform.position.z - item.transform.position.z;
            if (direction.z < 0)
                return item.transform.position.z - conveyorBuilding.endPivot.transform.position.z;
            return INF;
        }

        /// <summary>
        /// Checks if directions of two drivers are perpendicular or parallel. Returns true of they are perpendicular
        /// </summary>
        public bool IsOrthogonalTo(ConveyorBelt_Driver other)
        {
            return other.direction != this.direction && other.direction != -this.direction;
        }

        private bool IsOrthogonalItemAhead(ConveyorBelt_Item thisItem, ConveyorBelt_Item nextItem)
        {
            return (nextItem.transform.position - thisItem.transform.position).SignedMask().Multiply(nextDriver.direction.UnsignedMask()) == nextDriver.direction;
        }

        /// <summary>
        /// Calculates lane index of the nextDriver where targetItem can travel right now. Returns -1 if there is no available lane.
        /// </summary>
        private int ChooseOrthogonalLane(ConveyorBelt_Item targetItem)
        {
            bool laneAvailable(int laneIndex)
            {
                foreach (var item in nextDriver.lanes[laneIndex])
                {
                    // check if there is space for the new item
                    if (GetOrthogonalDistance(item, targetItem) < minItemDistance)
                    {
                        return false;
                    }
                }
                return true;
            }

            if (laneAvailable(targetItem.currentLaneIndex)) return targetItem.currentLaneIndex;

            for (int laneIndex = 0; laneIndex < LANES_NUMBER; laneIndex++)
            {
                if (targetItem.currentLaneIndex == laneIndex) continue;
                
                if (laneAvailable(laneIndex))
                {
                    return laneIndex;
                }
            }

            return -1; // no lane available, wait
        }

        public void TryFindNext()
        {
            Building otherBuilding = Map.getBuildingAt(conveyorBuilding.mapPosition + conveyorBuilding.moveDirectionClamped * conveyorBuilding.lengthTiles);
            if (otherBuilding == null || otherBuilding == conveyorBuilding) return;
            if (otherBuilding.TryGetComponent(out ConveyorBelt_Building otherConveyor) && 
                otherConveyor.driver.direction != -direction) // check if next conveyor is not pointing in the opposite direction
            {
                nextDriver = otherConveyor.driver;
            }
            else
            {
                nextDriver = null;
            }
        }

        /// <summary>
        /// Returns true if item can be added to the start of the driver as the first item. Assumes that item is to be added at the start anchor (without offset)
        /// </summary>
        /// <returns></returns>
        public bool CanEnqueueAnyItem()
        {
            foreach (var lane in lanes)
                if (lane.Count == 0 || GetSignedDistanceFromStart(lane.First.Value) > minItemDistance) return true;
            return false;
        }

        /// <summary>
        /// Returns true if item can be added to the start of the driver as the first item
        /// </summary>
        /// <returns></returns>
        public bool CanAddItemStraight(Lane lane, ConveyorBelt_Item item)
        {
            return lane.Count == 0 || GetStraightDistance(lane.First.Value, item) > minItemDistance;
        }

        /// <summary>
        /// As from a releaser, this function adds an item to an arbitrary lane
        /// </summary>
        public bool TakeOwnershipAddToStart(ConveyorBelt_Item item)
        {
            for (int i = 0; i < LANES_NUMBER; i++)
            {
                if (CanAddItemStraight(lanes[i], item))
                {
                    lanes[i].AddFirst(item);
                    item.transform.SetParent(conveyorBuilding.transform);
                    item.currentLaneIndex = i;
                    item.currentDriver = this;
                    item.holder = conveyorBuilding;
                    item.transform.position = GetPositionAlignedToLane(item.transform.position, i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Try add the item to a specific lane
        /// </summary>
        public void AddToLaneBeforeNext(int laneIndex, ItemNode itemNode, ItemNode nextItem = null)
        {
            AddToLaneBeforeNext(laneIndex, itemNode.Value, nextItem);
        }

        public void AddToLaneBeforeNext(int laneIndex, ConveyorBelt_Item item, ItemNode nextItem = null)
        {
            if (nextItem == null)
            {
                lanes[laneIndex].AddLast(item);
            }
            else
            {
                lanes[laneIndex].AddBefore(nextItem, item);
            }
            item.transform.SetParent(conveyorBuilding.transform);
            item.currentDriver = this;
            item.holder = conveyorBuilding;
            item.currentLaneIndex = laneIndex;
            item.transform.position = GetPositionAlignedToLane(item.transform.position, laneIndex); // align with regard to index and conveyor driver positioning
        }

        /// <summary>
        /// Removes the item from its lane on the conveyor, does not perform checks
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(ConveyorBelt_Item item)
        {
            lanes[item.currentLaneIndex].Remove(item);
        }

        public Vector3 GetPositionAlignedToLane(Vector3 position, int laneIndex)
        {
            float delta = conveyorBuilding.beltWidth / 2f - conveyorBuilding.beltWidth / (LANES_NUMBER-1) * laneIndex;
            if (direction.x != 0)
                return position.WithZ(conveyorBuilding.startPivot.position.z - delta);
            if (direction.z != 0)
                return position.WithX(conveyorBuilding.startPivot.position.x - delta);
            return position;
        }
    }
}
