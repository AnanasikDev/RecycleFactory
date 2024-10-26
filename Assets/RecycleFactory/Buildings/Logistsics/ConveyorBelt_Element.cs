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
        [ShowNativeProperty] public bool isEmpty { get; private set; } = true;
        [ShowNativeProperty] public bool isWorking { get; private set; }

        public bool isLast = false;

        public Action onDestroyed;
        public Action onStarted;
        public Action onStopped;

        public void Init(ConveyorBelt_Building building)
        { 
            this.conveyorBuilding = building;
            this.direction = building.moveDirectionClamped;
            this.distance = (float)building.lengthTiles/ building.capacity;
            transportTimeSeconds = building.transportTimeSeconds / building.capacity;

            if (isLast)
                Building.onAnyBuiltEvent += FindNextElement;
        }

        public void OnDestroy()
        { 
            if (isLast)
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

        private void ResetNextElement()
        {
            nextElement = null;
        }

        public void SetNextElement(ConveyorBelt_Element next)
        {
            nextElement = next;
        }

        private void FindNextElement()
        {
            Building otherBuilding = Map.getBuildingAt(conveyorBuilding.mapPosition + conveyorBuilding.moveDirectionClamped * conveyorBuilding.lengthTiles);
            if (otherBuilding == null) return;
            if (otherBuilding.TryGetComponent(out ConveyorBelt_Building otherConveyor))
            {
                Debug.Log("found " + otherBuilding.name);
                // TODO: calculate the closest element of otherConveyor (when merging it is not first)
                SetNextElement(otherConveyor.first);

                if (nextElement.isEmpty)
                {
                    Start();
                }
            }
        }

        private void Break()
        {
            isWorking = false;
            if (nextElement != null)
            {
                nextElement.onDestroyed += ResetNextElement;
                nextElement.onStarted += Start;
            }

            onStopped?.Invoke();
        }

        private void Start()
        {
            if (nextElement == null) return;

            isWorking = true;
            onStarted?.Invoke();
        }

        public void MoveItem()
        {
            if (isEmpty || !isWorking) return;
            if (isLast && nextElement == null || !nextElement.isEmpty) return;

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
                    Break();
                }
                elapsedTime = 0;
            }
        }
    }
}
