using System.Collections.Generic;
using UnityEngine;
using RecycleFactory.Buildings.Logistics;

namespace RecycleFactory.Buildings
{
    public class BuildingExtension_Releaser : MonoBehaviour
    {
        private Building building;
        public float height;
        [Tooltip("Must be set in inspector")] public List<ConveyorBelt_Anchor> outAnchors;

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
            Building.onAnyBuiltEvent += TryConnect;
            Building.onAnyDemolishedEvent += TryConnect;
        }

        private void OnDestroy()
        {
            Building.onAnyBuiltEvent -= TryConnect;
            Building.onAnyDemolishedEvent -= TryConnect;
        }

        /// <summary>
        /// Returns position where the specified anchor would release its items
        /// </summary>
        private Vector3 GetReleasePosition(int anchorIndex)
        {
            return (building.mapPosition.ConvertTo2D() + outAnchors[anchorIndex].localTilePosition.ConvertTo2D() + outAnchors[anchorIndex].direction.ConvertTo2D() / 2f).ProjectTo3D().WithY(outAnchors[anchorIndex].height);
        }

        public bool CanRelease(int anchorIndex)
        {
            return outAnchors[anchorIndex].conveyor != null && outAnchors[anchorIndex].conveyor.CanEnqueueAnyItem();
        }

        public void Release(ConveyorBelt_Item item, int anchorIndex)
        {
            item.transform.position = GetReleasePosition(anchorIndex);
            if (!outAnchors[anchorIndex].conveyor.AddToStart(item))
            {
                Debug.Log("Releaser halted due to fail to release an item");
            }
        }

        /// <summary>
        /// Updates connections on all outAnchors
        /// </summary>
        private void TryConnect()
        {
            for (int i = 0; i < outAnchors.Count; i++)
            {
                Building otherBuilding = Map.getBuildingAt(building.mapPosition + outAnchors[i].localTilePosition + outAnchors[i].direction);

                if (otherBuilding == null || !otherBuilding.TryGetComponent(out ConveyorBelt_Building otherConveyor))
                {
                    outAnchors[i].conveyor = null;
                }
                else
                {
                    outAnchors[i].conveyor = otherConveyor.driver;
                }
            }
        }
    }
}