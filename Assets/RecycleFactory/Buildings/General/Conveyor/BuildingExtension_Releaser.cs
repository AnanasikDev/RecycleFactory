using System.Collections.Generic;
using UnityEngine;
using RecycleFactory.Buildings.Logistics;
using Node = System.Collections.Generic.LinkedListNode<RecycleFactory.Buildings.Logistics.ConveyorBelt_Item>;

namespace RecycleFactory.Buildings
{
    public class BuildingExtension_Releaser : MonoBehaviour
    {
        private Building building;
        public float height;

        [Tooltip("Must be set in inspector")]
        public List<ConveyorBelt_Anchor> outAnchors;

        public void Init(Building building, int rotation)
        {
            this.building = building;
            foreach (var outAnchor in outAnchors)
            {
                outAnchor.conveyor = null;
                outAnchor.machine = building;
                outAnchor.height = height;
                outAnchor.Revolve(rotation);
            }
            Building.onAnyBuiltEvent += UpdateAnchorsConnections;
            Building.onAnyDemolishedEvent += UpdateAnchorsConnections;
        }

        private void OnDestroy()
        {
            Building.onAnyBuiltEvent -= UpdateAnchorsConnections;
            Building.onAnyDemolishedEvent -= UpdateAnchorsConnections;
        }

        private Vector3 GetReleasePosition(int anchorIndex, int laneIndex)
        {
            return outAnchors[anchorIndex].conveyor.GetPositionAlignedToLane
                (
                ((building.worldPosition2DInt + outAnchors[anchorIndex].localTilePosition).ConvertTo2D() + outAnchors[anchorIndex].direction.ConvertTo2D() / 2f).ProjectTo3D().WithY(outAnchors[anchorIndex].height), 
                laneIndex
                );
        }

        public int ChooseLane(int anchorIndex, out Node nextNode)
        {
            var anchor = outAnchors[anchorIndex];
            nextNode = null;

            // no conveyor connected OR next conveyor is flipped
            if (outAnchors[anchorIndex].conveyor == null || anchor.direction == -anchor.conveyor.direction.ProjectTo2D())
            {
                return -1;
            }

            // direct connection
            if (anchor.direction == anchor.conveyor.direction.ProjectTo2D())
            {
                var con = ChooseLaneStraight(anchorIndex);
                nextNode = con.nextItem;
                return con.laneIndex;
            }

            // orthogonal connection
            else if (!anchor.onlyDirectConnections)
            {
                var con = ChooseLaneOrthogonal(anchorIndex);
                EasyDebug.PipeConsole.Print(con.nextItem, con.laneIndex);
                nextNode = con.nextItem;
                return con.laneIndex;
            }

            return -1;
        }

        public void ForceRelease(int anchorIndex, int laneIndex, ConveyorBelt_Item item, Node nextNode)
        {
            item.transform.position = GetReleasePosition(anchorIndex, laneIndex);
            outAnchors[anchorIndex].conveyor.AddToLaneBeforeNext(laneIndex, item, nextNode);
        }

        /// <summary>
        /// Returns index of lane of the next STRAIGHT conveyor driver where an item from specified anchor can go in this frame. Returns -1 if there is no such lane. Does not check if next driver exists, if anchor index is correct or if next driver is connected straight.
        /// </summary>
        private ConnectionData ChooseLaneStraight(int anchorIndex)
        {
            var nextDriver = outAnchors[anchorIndex].conveyor;

            for (int l = 0; l < ConveyorBelt_Driver.LANES_NUMBER; l++)
            {
                if (nextDriver.lanes[l].First == null || nextDriver.GetSignedDistanceFromStart(nextDriver.lanes[l].First.Value) > nextDriver.minItemDistance)
                {
                    Debug.Log(l);
                    return new ConnectionData(l, nextDriver.lanes[l].First);
                }
            }

            return new ConnectionData(-1, null);
        }

        /// <summary>
        /// Returns index of lane of the next ORTHOGONAL conveyor driver where an item from specified anchor can go in this frame. Returns -1 if there is no such lane. Does not check if next driver exists, if anchor index is correct or if next driver is connected orthogonally.
        /// </summary>
        private ConnectionData ChooseLaneOrthogonal(int anchorIndex)
        {
            var nextDriver = outAnchors[anchorIndex].conveyor;
            Vector3 nextDirectionMask = outAnchors[anchorIndex].conveyor.direction.Abs();
            Vector3 anchorReleasePosition = (building.worldPosition2DInt + outAnchors[anchorIndex].localTilePosition).ConvertTo2D().ProjectTo3D() + outAnchors[anchorIndex].direction.ConvertTo2D().ProjectTo3D() / 2f;

            bool laneAvailable(int laneIndex, out Node nextNode)
            {
                float minDist = float.MaxValue;
                nextNode = null;
                Vector3 targetItemPos = nextDriver.GetPositionAlignedToLane(anchorReleasePosition, laneIndex);
                for (Node node = nextDriver.lanes[laneIndex].First; node != null; node = node.Next)
                {
                    float dist = (node.Value.transform.position - targetItemPos).Multiply(nextDirectionMask).magnitude;

                    // check if there is space for the new item
                    if (dist < nextDriver.minItemDistance)
                    {
                        return false;
                    }

                    if (dist < minDist)
                    {
                        minDist = dist;
                        nextNode = node;
                    }
                }

                return true;
            }

            for (int laneIndex = 0; laneIndex < ConveyorBelt_Driver.LANES_NUMBER; laneIndex++)
            {
                if (laneAvailable(laneIndex, out Node nextNode))
                {
                    return new ConnectionData(laneIndex, nextNode);
                }
            }

            return new ConnectionData(-1, null); // no lane available, wait
        }


        private void UpdateAnchorsConnections()
        {
            for (int i = 0; i < outAnchors.Count; i++)
            {
                Building otherBuilding = Map.getBuildingAt(building.mapPosition + outAnchors[i].localTilePosition + outAnchors[i].direction);

                if (otherBuilding == null)
                {
                    outAnchors[i].conveyor = null;
                    continue;
                }

                if (otherBuilding.TryGetComponent(out ConveyorBelt_Building otherConveyor))
                {
                    if ((outAnchors[i].onlyDirectConnections && otherConveyor.moveDirectionClamped == outAnchors[i].direction) ||
                        (!outAnchors[i].onlyDirectConnections && otherConveyor.moveDirectionClamped != -outAnchors[i].direction))
                    {
                        outAnchors[i].conveyor = otherConveyor.driver;
                        continue;
                    }
                }

                outAnchors[i].conveyor = null;
            }
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                for (int i = 0; i < outAnchors.Count; i++)
                {
                    DrawArrow.ForGizmo((building.worldPosition2DInt + outAnchors[i].localTilePosition).ConvertTo2D().ProjectTo3D(), outAnchors[i].direction.ConvertTo2D().ProjectTo3D());
                }
            }
            else
            {
                for (int i = 0; i < outAnchors.Count; i++)
                {
                    DrawArrow.ForGizmo((outAnchors[i].localTilePosition).ConvertTo2D().ProjectTo3D(), outAnchors[i].direction.ConvertTo2D().ProjectTo3D());
                }
            }
        }
    }

    public struct ConnectionData
    {
        public int laneIndex;
        public Node nextItem;

        public ConnectionData(int laneIndex, Node nextItem)
        {
            this.laneIndex = laneIndex;
            this.nextItem = nextItem;
        }
    }
}