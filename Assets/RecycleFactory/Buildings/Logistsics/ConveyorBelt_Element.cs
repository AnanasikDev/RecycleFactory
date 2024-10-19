using NaughtyAttributes;
using System;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    [Serializable]
    public class ConveyorBelt_Element
    {
        public ConveyorBelt_Building conveyorBuilding;

        // values are taken from conveyorBuilding
        [ReadOnly] public Vector2Int direction;
        [ReadOnly] public float distance;
        [ReadOnly] public float transportTimeSeconds;
        [ReadOnly] public float elapsedTime = 0;

        private ConveyorBelt_Item currentItem;
        private ConveyorBelt_Element nextElement;
        public bool isEmpty { get; private set; } = true;
        public bool isWorking { get; private set; }

        public void Init(ConveyorBelt_Building building)
        { 
            this.conveyorBuilding = building;
            direction = building.moveDirection;
            transportTimeSeconds = building.transportTimeSeconds / building.capacity;
            Building.onAnyBuiltEvent += FindNextElement;
        }

        public void OnDestroy()
        { 
            Building.onAnyBuiltEvent -= FindNextElement;
        }

        public void SetItem(ConveyorBelt_Item item)
        {
            if (isEmpty)
            {
                currentItem = item;
                isEmpty = false;
                isWorking = true;
            }
        }

        private void FindNextElement()
        {
            Building otherBuilding =  Map.getBuildingAt(conveyorBuilding.mapPosition + conveyorBuilding.moveDirection * conveyorBuilding.size);
            if (otherBuilding == null) return;
            if (otherBuilding.TryGetComponent(out ConveyorBelt_Building otherConveyor))
            {
                // TODO: calculate the closest element of otherConveyor (when merging it is not first)
                SetNextElement(otherConveyor.first);
                otherConveyor.first.SetNextElement(this);
            }
        }

        public void SetNextElement(ConveyorBelt_Element next)
        {
            nextElement = next;
        }

        public void ResetNextElement()
        {
            nextElement = null;
        }

        public void MoveItem()
        {
            if (isEmpty || !isWorking) return;

            currentItem.transform.position += direction.ConvertTo2D().ConvertTo3D() * distance / conveyorBuilding.capacity / transportTimeSeconds * Time.deltaTime;
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= transportTimeSeconds)
            {
                if (nextElement != null && nextElement.isEmpty)
                {
                    nextElement.SetItem(currentItem);
                    currentItem = null;
                    isEmpty = true;
                }
                else
                {
                    isWorking = false;
                }
                elapsedTime = 0;
            }
        }
    }
}
