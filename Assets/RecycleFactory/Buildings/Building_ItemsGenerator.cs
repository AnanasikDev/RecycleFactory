using System.Collections.Generic;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    [RequireComponent(typeof(BuildingExtension_Releaser))]
    public class Building_ItemsGenerator : Building
    {
        [SerializeField] private List<ConveyorBelt_ItemInfo> itemInfos;

        protected override void PostInit()
        {
            
        }       

        private void Update()
        {
            if (releaser.IsFreeToRelease(0))
            {
                var item = ConveyorBelt_Item.Create(itemInfos.RandomElement());
                item.transform.position = transform.position;
                releaser.ForceRelease(item, 0);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            DrawArrow.ForGizmo(transform.position, releaser.outAnchors[0].direction.ConvertTo2D().ProjectTo3D());
        }
    }
}