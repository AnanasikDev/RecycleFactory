using NaughtyAttributes;
using System;
using UnityEngine;

namespace RecycleFactory.Buildings
{
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

        /// <summary>
        /// Whether the element is inside the stack of elements and has a constant next element (true only for the last ones in a stack)
        /// </summary>
        public bool isStatic = true;

        public void Init(ConveyorBelt_Building building)
        { 
            this.conveyorBuilding = building;
            this.direction = building.moveDirection;
            this.distance = building.distance / building.capacity;
            transportTimeSeconds = building.transportTimeSeconds / building.capacity;

            if (!isStatic)
                Building.onAnyBuiltEvent += FindNextElement;
        }

        public void OnDestroy()
        { 
            if (!isStatic)
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
            Building otherBuilding =  Map.getBuildingAt(conveyorBuilding.mapPosition + conveyorBuilding.moveDirection);
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

            currentItem.transform.position += direction.ConvertTo2D().ProjectTo3D() * distance / transportTimeSeconds * Time.deltaTime;
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
