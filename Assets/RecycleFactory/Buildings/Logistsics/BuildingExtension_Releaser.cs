using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace RecycleFactory.Buildings
{
    public class BuildingExtension_Releaser : MonoBehaviour
    {
        private Building building;
        public float height;
        [Tooltip("Must be set in inspector")] public List<ConveyorAnchor> outAnchors;

        public void Init(Building building)
        {
            this.building = building;
            foreach (var outAnchor in outAnchors)
            {
                outAnchor.conveyor = null;
                outAnchor.machine = building;
                outAnchor.height = height;
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

        private Vector3 getPosition(int anchorIndex)
        {
            return (building.mapPosition.ConvertTo2D() + outAnchors[anchorIndex].localTilePosition.ConvertTo2D() + outAnchors[anchorIndex].direction.ConvertTo2D() / 2f).ProjectTo3D().WithY(outAnchors[anchorIndex].height);
        }

        public bool CanRelease(int anchorIndex)
        {
            return !outAnchors[anchorIndex].conveyor.CanEnqueueItem();
        }

        public void Release(ConveyorBelt_Item item, int anchorIndex)
        {
            item.transform.position = getPosition(anchorIndex);
            if (!outAnchors[anchorIndex].conveyor.AddItem(item))
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
                if (otherBuilding == null) return;
                if (otherBuilding.TryGetComponent(out ConveyorBelt_Building otherConveyor))
                {
                    // TODO: calculate the closest element of otherConveyor (when merging it is not first)
                    outAnchors[i].conveyor = otherConveyor.driver;
                }
                else
                {
                    outAnchors[i].conveyor = null;
                }
            }
        }
    }
}