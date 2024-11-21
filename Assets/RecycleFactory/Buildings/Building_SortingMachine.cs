using UnityEngine;
using RecycleFactory.Buildings.Logistics;

namespace RecycleFactory.Buildings
{
    [RequireComponent(typeof(BuildingExtension_Receiver))]
    [RequireComponent(typeof(BuildingExtension_Releaser))]
    public class Building_SortingMachine : Building
    {
        private void Update()
        {
            ManageItem();
        }
        public void ManageItem()
        {
            if (receiver.CanReceive(0, out ConveyorBelt_Item item)){
                var info = item.info;
                if (info.magnetic > 0.8f)
                {
                    if (!releaser.CanRelease(0)) return;
                    receiver.ForceReceive(item);
                    item = ConveyorBelt_Item.Create(info);
                    releaser.Release(item, 0);
                }
                else
                {
                    if (!releaser.CanRelease(1)) return;
                    receiver.ForceReceive(item);
                    item = ConveyorBelt_Item.Create(info);
                    releaser.Release(item, 1);
                }
            }
        }
    }
}
