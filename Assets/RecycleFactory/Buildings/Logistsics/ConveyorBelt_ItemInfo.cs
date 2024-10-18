using UnityEngine;

namespace RecycleFactory.Buildings
{
    [CreateAssetMenu(menuName = "ItemInfo", fileName = "itemInfo_")]
    public class ConveyorBelt_ItemInfo : ScriptableObject
    {
        public new string name;
        public Mesh mesh;
        // other properties
    }
}