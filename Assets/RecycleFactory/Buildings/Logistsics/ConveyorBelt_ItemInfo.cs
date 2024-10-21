using NaughtyAttributes;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    [CreateAssetMenu(menuName = "ItemInfo", fileName = "itemInfo_")]
    public class ConveyorBelt_ItemInfo : ScriptableObject
    {
        public new string name;
        public Mesh mesh;
        public Material material;

        [MinValue(0)][MaxValue(1)] public float magnetic;
        [MinValue(0)][MaxValue(1)] public float transparency;
        [MinValue(0)][MaxValue(1)] public float density;
    }
}
