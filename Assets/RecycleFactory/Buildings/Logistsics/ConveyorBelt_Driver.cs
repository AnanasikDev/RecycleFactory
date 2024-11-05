using UnityEngine;
using Lane = System.Collections.Generic.LinkedList<RecycleFactory.Buildings.Logistics.ConveyorBelt_Item>;
using ItemNode = System.Collections.Generic.LinkedListNode<RecycleFactory.Buildings.Logistics.ConveyorBelt_Item>;

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
        public Vector3 velocity { get { return direction * conveyorBuilding.lengthTiles / conveyorBuilding.transportTimeSeconds * Time.deltaTime; } }

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

        private void MoveAllItems()
        {
            // for each item check distance to the next one, translate if possible, halt movement if not
            for (int l = 0; l < LANES_NUMBER; l++)
            {
                Lane lane = lanes[l];
                if (lanes[l].Count == 0) continue;
                for (ItemNode itemNode = lane.First; itemNode != null; itemNode = itemNode.Next)
                {
                    ConveyorBelt_Item item = itemNode.Value;

                    // close to the end of the current conveyor
                    if (GetSignedDistanceToEnd(item) < minEndDistance)
                    {
                        // no way to go, halt movement
                        if (nextDriver == null) continue;

                        // first item of the next conveyor
                        var nextItemNode = nextDriver.lanes[item.currentLaneIndex].First;

                        // there is next conveyor and there is space in it
                        if (nextItemNode == null || 
                            GetStraightDistance(item, nextItemNode.Value) > minEndDistance)
                        {

                            nextDriver.TakeOwnership(item);
                            continue;
                        }
                        // no next conveyor or it's full - stop here
                        continue;
                    }
                    if (lane == null || GetStraightDistance(item, lane.Value) > minItemDistance)
                    {
                        item.transform.Translate(direction * conveyorBuilding.speed * Time.deltaTime);
                    }
                }
            }

            // if merging to another conveyor straightly reparent
            // if merging to another conveyor at 90 degrees choose a random lane, calculate if there is space in according to travel time, translate or halt
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
        /// Knowing info about the curretn and next drivers, this function returns index of a lane where the item is to go at the moment, or -1 if no lane is available.
        /// </summary>
        /// <param name="targetItem"></param>
        /// <returns></returns>
        private int ChooseLane(ConveyorBelt_Item targetItem)
        {
            // if straight merge, do not change lange
            if (this.direction == nextDriver.direction) return targetItem.currentLaneIndex;
            
            // if conveyors converge, no cross movement is allowed, halt
            if (this.direction == -nextDriver.direction) return -1;

            for (int l = 0; l < LANES_NUMBER; l++)
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
                nextDriver = conveyorBuilding.driver;
            }
            else
            {
                nextDriver = null;
            }
        }

        /// <summary>
        /// Returns true if item can be added to the start of the driver as the first item
        /// </summary>
        /// <returns></returns>
        public bool CanEnqueueItem()
        {
            foreach (var lane in lanes)
                if (lane.Count == 0 || GetSignedDistanceFromStart(lane.First.Value) > minItemDistance) return true;
            return false;
        }

        /// <summary>
        /// Returns true if item can be added to the start of the driver as the first item
        /// </summary>
        /// <returns></returns>
        public bool CanEnqueueItem(Lane lane)
        {
            return lane.Count == 0 || GetSignedDistanceFromStart(lane.First.Value) > minItemDistance;
        }


        /// <summary>
        /// As from a releaser, this function adds an item to an arbitrary lane
        /// </summary>
        public bool AddItem(ConveyorBelt_ItemInfo itemInfo)
        {
            var item = ConveyorBelt_Item.Create(itemInfo);
            return AddItem(item);
        }

        /// <summary>
        /// As from a releaser, this function adds an item to an arbitrary lane
        /// </summary>
        public bool AddItem(ConveyorBelt_Item item)
        {
            for (int i = 0; i < LANES_NUMBER; i++)
            {
                if (CanEnqueueItem(lanes[i]))
                {
                    lanes[i].AddFirst(item);
                    item.currentLaneIndex = i;
                    AlignToLane(item, i);
                    return true;
                }
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

        public void TakeOwnership(ConveyorBelt_Item item)
        {
            item.transform.SetParent(conveyorBuilding.transform);
            lanes[item.currentLaneIndex].Remove(item);
            nextDriver.lanes[item.currentLaneIndex].AddFirst(item);
        }
    }
}
