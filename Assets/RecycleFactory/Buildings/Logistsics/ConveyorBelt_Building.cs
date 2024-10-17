using UnityEngine;
using System.Collections.Generic;

public class ConveyorBelt_Building : Building
{
    public int capacity = 10;
    public Vector3 direction;
    public float transportTimeSeconds = 5;

    private List<ConveyorBelt_Element> elements;

    public ConveyorBelt_Element first { get { return elements[0]; } }
    public ConveyorBelt_Element last { get { return elements[capacity - 1]; } }

    private void Start()
    {
        elements = new List<ConveyorBelt_Element>();
        for (int i = 0; i < capacity; i++)
        {
            var e = new ConveyorBelt_Element();
            elements.Add(e);
            e.direction = direction;
            e.transportTimeSeconds = transportTimeSeconds;
        }
    }

    private void Update()
    {
        foreach (var element in elements)
        {
            element.MoveItem();
        }
    }
}
