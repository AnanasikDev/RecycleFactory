using NaughtyAttributes;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    [CreateAssetMenu(menuName = "ItemInfo", fileName = "itemInfo_")]
    public class ConveyorBelt_ItemInfo : ScriptableObject
    {
        public new string name;
        public Mesh mesh;

        [MinMaxSlider(0, 1)] public float magnetic;
        [MinMaxSlider(0, 1)] public float transparency;
        [MinMaxSlider(0, 1)] public float density;
    }
}
