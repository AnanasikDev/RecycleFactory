using System.Collections.Generic;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    public class ConveyorBelt_Building : Building
    {
        [Tooltip("Total number of elements, including the last one")] public int capacity = 4;
        public Vector2Int moveDirectionClamped;
        public float transportTimeSeconds = 5;
        public int lengthTiles = 2;
        
        public Transform startPivot;
        public Transform endPivot;

        public float speed { get { return lengthTiles / transportTimeSeconds; } }

        public ConveyorBelt_Driver driver;

        protected override void PostInit()
        {
            driver.Init(this);
            moveDirectionClamped = Utils.RotateXY(moveDirectionClamped, rotation);
        }

        private void Update()
        {
            driver.Update();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            DrawArrow.ForGizmo(transform.position, moveDirectionClamped.ConvertTo2D().ProjectTo3D());

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(startPivot.transform.position, 0.4f);
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(endPivot.transform.position, 0.4f);
        }
    }
}
