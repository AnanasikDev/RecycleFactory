﻿using UnityEngine;
using Lane = System.Collections.Generic.LinkedList<RecycleFactory.Buildings.Logistics.ConveyorBelt_Item>;
using ItemNode = System.Collections.Generic.LinkedListNode<RecycleFactory.Buildings.Logistics.ConveyorBelt_Item>;
using System.Collections.Generic;
using static UnityEditor.Progress;
using System.Data;
using UnityEditorInternal.Profiling.Memory.Experimental;

namespace RecycleFactory.Buildings.Logistics
{
    public class ConveyorBelt_Driver
    {
        internal static readonly int INF = 100000;
        public static readonly int LANES_NUMBER = 3;
        internal readonly Lane[] lanes = new Lane[LANES_NUMBER];

        internal ConveyorBelt_Building conveyorBuilding { get; init; }
        internal ConveyorBelt_Driver nextDriver;
        public readonly float minItemDistance = 0.6f;
        public readonly float minEndDistance = 0.3f;

        public Vector3 direction { get; init; }
        public Vector3 frameVelocity { get { return direction * conveyorBuilding.lengthTiles / conveyorBuilding.transportTimeSeconds * Time.deltaTime; } }

        public ConveyorBelt_Driver(ConveyorBelt_Building conveyorBuilding)
        {
            this.conveyorBuilding = conveyorBuilding;
            direction = conveyorBuilding.moveDirectionClamped.ConvertTo2D().ProjectTo3D();

            // init empty lanes
            for (int l = 0; l < LANES_NUMBER; l++)
            {
                lanes[l] = new Lane();
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
        }

        public void Update()
        {
            MoveAllItems();
        }

        private void TryMoveItem(ItemNode itemNode, ConveyorBelt_Item item)
        {
            // check if there is enough distance to next item
            if (
            // next item on this conveyor is far enough
                (itemNode.Next == null || GetStraightDistance(item, itemNode.Next.Value) > minItemDistance)
                )
            {
                if (nextDriver == null)
                {
                    item.transform.Translate(frameVelocity);
                }

                else if (this.IsOrthogonalTo(nextDriver))
                {

                }
                else
                {
                    // first item on next conveyor is also far enough
                    if (nextDriver.lanes[item.currentLaneIndex].First == null || GetStraightDistance(item, nextDriver.lanes[item.currentLaneIndex].First.Value) > minItemDistance)
                    {
                        item.transform.Translate(frameVelocity);
                    }
                }
            }
        }

        private bool TryTransferItem(ItemNode itemNode, ConveyorBelt_Item item, int laneIndex)
        {
            // close to the end of the current conveyor
            if (GetSignedDistanceToEnd(item) < minEndDistance)
            {
                // no way to go, halt movement
                if (nextDriver == null) return false;

                // first item of the next conveyor
                var nextItemNode = nextDriver.lanes[item.currentLaneIndex].First;

                // check if there is next conveyor and there is space in it
                if (nextItemNode == null ||
                    (GetStraightDistance(item, nextItemNode.Value) > minItemDistance))
                {
                    // reassign item to be controled by next driver
                    lanes[item.currentLaneIndex].Remove(item);
                    nextDriver.AddToLaneStart(laneIndex, item);

                    // last item in the lane processed, exit the loop
                    return true;
                }
                // no next conveyor or it's full - stop here
                return false;
            }

            return false;
        }

        private void MoveAllItems()
        {
            // for each item check distance to the next one, translate if possible, halt movement if not
            for (int laneIndex = 0; laneIndex < LANES_NUMBER; laneIndex++)
            {
                Lane lane = lanes[laneIndex];
                if (lane.Count == 0) continue;

                for (ItemNode itemNode = lane.First; itemNode != null; itemNode = itemNode.Next)
                {
                    ConveyorBelt_Item item = itemNode.Value;

                    // check if close enough for transfer
                    if (GetSignedDistanceToEnd(item) < minEndDistance)
                    {
                        if (nextDriver == null)
                        {
                            // halt
                        }

                        // there is next driver connected

                        else if (IsOrthogonalTo(nextDriver))
                        {
                            int targetLaneIndex = ChooseOrthogonalLane(item); // find a lane where the item can go right now
                            if (targetLaneIndex == -1)
                            {
                                // halt
                            }
                            else
                            {
                                nextDriver.TakeOwnershipWithTransition(this, item, targetLaneIndex); // transfer item with smooth transition
                            }
                        }

                        else
                        {
                            // next driver has same direction, check if its first item is far enough
                            if (nextDriver.lanes[item.currentLaneIndex].First == null || GetStraightDistance(item, nextDriver.lanes[item.currentLaneIndex].First.Value) > minItemDistance)
                            {
                                // there is either no items in the next conveyor or its first item is far enough
                                nextDriver.TakeOwnership(this, item); // transfer item with no transitions, just switch ownership
                            }
                        }
                    }

                    else // not close to edge, try to move
                    {
                        // check if there is room to go
                        if ((itemNode.Next == null || GetStraightDistance(item, itemNode.Next.Value) > minItemDistance) &&
                        (nextDriver == null || (nextDriver.lanes[item.currentLaneIndex].First == null || GetStraightDistance(item, nextDriver.lanes[item.currentLaneIndex].First.Value) > minItemDistance)))
                        {
                            item.transform.Translate(frameVelocity); // move item
                        }
                    }
                }
            }

            // if merging to another conveyor straightly reparent
            // if merging to another conveyor at 90 degrees choose a random lane, calculate if there is space in according to travel time, translate or halt
        }

        /// <summary>
        /// Takes ownership of the item, becoming responsible of its visibility, transition, deletion and processing.
        /// </summary>
        public void TakeOwnership(ConveyorBelt_Driver oldOwner, ConveyorBelt_Item item)
        {
            oldOwner.lanes[item.currentLaneIndex].Remove(item);
            this.AddToLaneStart(item.currentLaneIndex, item);
            Debug.Log("Ownership of " + item.name + " has been taken!");
        }

        public void TakeOwnershipWithTransition(ConveyorBelt_Driver oldOwner, ConveyorBelt_Item item, int targetLaneIndex, float transitionSpeedFactor = 1.0f)
        {
            TakeOwnership(oldOwner, item);
            // TODO: visualize transition without affecting ownership logic. Calculate travel distance according to targetLaneIndex. Use Tween methods
        }

        private float GetStraightDistance(ConveyorBelt_Item item1, ConveyorBelt_Item item2)
        {
            return item1 == null || item2 == null ? INF : (item1.transform.position - item2.transform.position).Multiply(this.direction).magnitude;
        }

        private float GetOrthogonalDistance(ConveyorBelt_Item thisItem, ConveyorBelt_Item nextItem)
        {
            return thisItem == null || nextItem == null ? INF : (thisItem.transform.position - nextItem.transform.position).Multiply(nextDriver.direction).magnitude;
        }

        private float GetSignedDistanceFromStart(ConveyorBelt_Item item)
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

        private float GetSignedDistanceToEnd(ConveyorBelt_Item item)
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

        /// <summary>
        /// Calculates lane index of the nextDriver where targetItem can travel right now. Returns -1 if there is no available lane.
        /// </summary>
        private int ChooseOrthogonalLane(ConveyorBelt_Item targetItem)
        {
            for (int laneIndex = 0; laneIndex < LANES_NUMBER; laneIndex++)
            {
                bool isLaneObscured = false;
                foreach (var item in lanes[laneIndex])
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
                    return laneIndex;
                }
            }

            return -1; // no lane available, wait
        }

        public void TryFindNext()
        {
            Building otherBuilding = Map.getBuildingAt(conveyorBuilding.mapPosition + conveyorBuilding.moveDirectionClamped * conveyorBuilding.lengthTiles);
            if (otherBuilding == null || otherBuilding == conveyorBuilding) return;
            if (otherBuilding.TryGetComponent(out ConveyorBelt_Building otherConveyor))
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
        public bool CanEnqueueItem(Lane lane, ConveyorBelt_Item item)
        {
            return lane.Count == 0 || GetStraightDistance(lane.First.Value, item) > minItemDistance;
        }

        /// <summary>
        /// As from a releaser, this function adds an item to an arbitrary lane
        /// </summary>
        public bool AddToStart(ConveyorBelt_Item item)
        {
            for (int i = 0; i < LANES_NUMBER; i++)
            {
                if (CanEnqueueItem(lanes[i], item))
                {
                    lanes[i].AddFirst(item);
                    item.transform.SetParent(conveyorBuilding.transform);
                    item.currentLaneIndex = i;
                    item.currentDriver = this;
                    item.holder = conveyorBuilding;
                    AlignToLane(item, i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// As from a releaser, this function adds an item to an arbitrary lane
        /// </summary>
        public bool AddToLaneStart(int laneIndex, ConveyorBelt_Item item)
        {
            if (CanEnqueueItem(lanes[laneIndex], item))
            {
                lanes[laneIndex].AddFirst(item);
                item.transform.SetParent(conveyorBuilding.transform);
                item.currentDriver = this;
                item.holder = conveyorBuilding;
                if (laneIndex != item.currentLaneIndex)
                {
                    item.currentLaneIndex = laneIndex;
                    AlignToLane(item, laneIndex);
                }
                return true;
            }
            return false;
        }

        private void AlignToLane(ConveyorBelt_Item item, int laneIndex)
        {
            float delta = conveyorBuilding.beltWidth / 2f - conveyorBuilding.beltWidth / (LANES_NUMBER-1) * laneIndex;
            if (direction.x != 0)
                item.transform.position = item.transform.position.WithZ(conveyorBuilding.startPivot.position.z - delta);
            if (direction.z != 0)
                item.transform.position = item.transform.position.WithX(conveyorBuilding.startPivot.position.x - delta);
        }
    }
}
