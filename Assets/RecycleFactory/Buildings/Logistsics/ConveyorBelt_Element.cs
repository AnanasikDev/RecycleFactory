using NaughtyAttributes;
using System;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    [Serializable]
    public class ConveyorBelt_Element
    {
        [ReadOnly] public Vector2Int direction;
        public float transportTimeSeconds;
        [ReadOnly] public float elapsedTime = 0;

        private ConveyorBelt_Item currentItem;
        private ConveyorBelt_Element nextElement;
        public bool isEmpty { get; private set; } = true;
        public bool isWorking { get; private set; }

        public void SetItem(ConveyorBelt_Item item)
        {
            if (isEmpty)
            {
                currentItem = item;
                isEmpty = false;
                isWorking = true;
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

            currentItem.transform.position += direction.ConvertTo2D().ConvertTo3D() / transportTimeSeconds * Time.deltaTime;
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
