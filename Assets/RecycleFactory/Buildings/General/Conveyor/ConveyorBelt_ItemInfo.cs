using UnityEngine;

namespace RecycleFactory.Buildings.Logistics
{
    [CreateAssetMenu(menuName = "ItemInfo", fileName = "itemInfo_")]
    public class ConveyorBelt_ItemInfo : ScriptableObject
    {
        public new string name;
        public ItemCategories category;
        public Mesh mesh;
        public Material[] materials;

        [Range(0, 1)] public float metallic;
        [Range(0, 1)] public float plastic;
        [Range(0, 1)] public float organic;
        [Range(0, 1)] public float paper;
        
        [Range(0, 1)] public float magnetic;
        [Range(0, 1)] public float transparency;
        [Range(0, 1)] public float density;

        public bool battery;
        public bool lightbulb;
    }

    public enum ItemCategories
    {
        Paper,
        Glass,
        Plastic,
        Metal,
        Organic,
        Battery,
        Lightbulb
    }
}
