using RecycleFactory.Buildings.Logistics;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    [RequireComponent(typeof(BuildingExtension_Receiver))]
    public class BuildingIncinerator : Building
    {
        protected override void PostInit()
        {

        }

        private void Update()
        {
            Incinerate();
        }

        private void Incinerate()
        {
            // try receive an item
            if (receiver.TryReceive(0, out ConveyorBelt_ItemInfo info))
            {
                Debug.Log("INCINERATED SUCCESSFULLY");
            }

            // if success, destroy it
        }
    }
}
