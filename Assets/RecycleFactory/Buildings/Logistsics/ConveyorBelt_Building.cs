using UnityEngine;
using RecycleFactory.Buildings.Logistics;

namespace RecycleFactory.Buildings
{
    public class ConveyorBelt_Building : Building
    {
        [Tooltip("Total number of elements, including the last one")] public int capacity = 4;
        public float beltWidth = 0.8f;
        public Vector2Int moveDirectionClamped;
        public float transportTimeSeconds = 5;
        public int lengthTiles = 2;
        
        public Transform startPivot;
        public Transform endPivot;

        public ConveyorBelt_Driver driver { get; private set; }

        protected override void PostInit()
        {
            // calculate new direction with regards to rotation
            moveDirectionClamped = Utils.RotateXY(moveDirectionClamped, rotation);

            driver = new ConveyorBelt_Driver(this);
            Building.onAnyBuiltEvent += driver.TryFindNext;
        }

        private void OnDestroy()
        {
            if (driver != null)
            {
                Building.onAnyBuiltEvent -= driver.TryFindNext;
                driver.Destroy();
            }
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
