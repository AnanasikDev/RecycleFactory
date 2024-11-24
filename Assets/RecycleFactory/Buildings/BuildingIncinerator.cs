using NaughtyAttributes;
using RecycleFactory.Buildings.Logistics;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    [RequireComponent(typeof(BuildingExtension_Receiver))]
    public class BuildingIncinerator : Building
    {
        [Range(-100, 100)] public float metaillicBonus;
        [Range(-100, 100)] public float plasticBonus;
        [Range(-100, 100)] public float organicBonus;
        [Range(-100, 100)] public float paperBonus;

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
                float bonus = 0;
                bonus += metaillicBonus * info.metallic;
                bonus += plasticBonus   * info.plastic;
                bonus += organicBonus   * info.organic;
                bonus += paperBonus     * info.paper;
                Scripts.Budget.Add((int)bonus);
                Debug.Log("INCINERATED SUCCESSFULLY");
            }

            // if success, destroy it
        }
    }
}
