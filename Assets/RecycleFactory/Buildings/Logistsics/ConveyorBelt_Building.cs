﻿using System.Collections.Generic;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    public class ConveyorBelt_Building : Building
    {
        public int capacity = 10;
        public Vector2Int moveDirection;
        public float transportTimeSeconds = 5;
        public float distance = 2;

        private List<ConveyorBelt_Element> elements;

        public ConveyorBelt_Element first { get { return elements[0]; } }
        public ConveyorBelt_Element last { get { return elements[capacity - 1]; } }

        protected override void PostInit()
        {
            moveDirection = Utils.RotateXY(moveDirection, rotation);

            elements = new List<ConveyorBelt_Element>();
            for (int i = 0; i < capacity; i++)
            {
                var e = new ConveyorBelt_Element();
                elements.Add(e);
                e.Init(this);
            }

            // last element is not static, has to update next element
            elements[capacity - 1].isStatic = false;

            for (int i = 1; i < capacity; i++)
            {
                elements[i - 1].SetNextElement(elements[i]);
            }
        }

        /*private void OnDestroy()
        {
            for (int i = 0; i < capacity; i++)
            {
                elements[i].OnDestroy();
            }
        }*/

        private void Update()
        {
            foreach (var element in elements)
            {
                element.MoveItem();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            DrawArrow.ForGizmo(transform.position, moveDirection.ConvertTo2D().ProjectTo3D());
        }
    }
}
