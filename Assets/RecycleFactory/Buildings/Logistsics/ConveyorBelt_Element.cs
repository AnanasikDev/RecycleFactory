using NaughtyAttributes;
using System;
using UnityEngine;

[Serializable]
public class ConveyorBelt_Element
{
    [ReadOnly] public Vector3 direction;
    public float transportTimeSeconds = 5;
    [ReadOnly] public float elapsedTime = 0;

    private ConveyorBelt_Item currentItem;
    private ConveyorBelt_Element nextElement;
    public bool isEmpty { get; private set; } = true;

    public void SetItem(ConveyorBelt_Item item)
    {
        if (isEmpty)
        {
            currentItem = item;
            isEmpty = false;
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
        if (isEmpty) return;

        currentItem.transform.position += direction * transportTimeSeconds * Time.deltaTime;
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= transportTimeSeconds)
        {
            if (nextElement != null && nextElement.isEmpty)
            {
                nextElement.SetItem(currentItem);
                currentItem = null;
                isEmpty = false;
            }
            elapsedTime = 0;
        }
    }
}
