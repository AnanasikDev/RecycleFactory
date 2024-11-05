using UnityEngine;
using RecycleFactory.Buildings.Logistics;

namespace RecycleFactory.Buildings
{
    [RequireComponent(typeof(BuildingExtension_Receiver))]
    [RequireComponent(typeof(BuildingExtension_Releaser))]
    public class Building_SortingMachine : Building
    {
        public void ManageItem(ConveyorBelt_Item item)
        {
            
        }
    }
}
