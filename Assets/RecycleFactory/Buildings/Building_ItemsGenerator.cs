using System.Collections.Generic;
using UnityEngine;
using RecycleFactory.Buildings.Logistics;

namespace RecycleFactory.Buildings
{
    [RequireComponent(typeof(BuildingExtension_Releaser))]
    public class Building_ItemsGenerator : Building
    {
        [SerializeField] private float intervalSeconds = 1;

        protected override void PostInit()
        {
            InvokeRepeating("ReleaseItems", intervalSeconds, intervalSeconds);
        }       

        private void ReleaseItems()
        {
            for (int i = 0; i < ConveyorBelt_Driver.LANES_NUMBER; i++)
            {
                ReleaseOneItem();
            }   
        }

        private void ReleaseOneItem()
        {
            int laneIndex = releaser.ChooseLane(0, out var nextNode);
            if (laneIndex == -1) return; // do not create item if no lane is free

            var item = ConveyorBelt_Item.Create(Scripts.LevelController.unlockedItems.RandomElement());
            item.transform.position = transform.position;
            releaser.ForceRelease(0, laneIndex, item, nextNode);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            DrawArrow.ForGizmo(transform.position, releaser.outAnchors[0].direction.ConvertTo2D().ProjectTo3D());
        }
    }
}