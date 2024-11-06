using NaughtyAttributes;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    public class BuildingRenderer : MonoBehaviour
    {
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        
        [ShowNativeProperty] public Vector3 scale { get; private set; } = Vector3.one;

        public void Init()
        {
            scale = meshFilter.transform.localScale;
        }
    }
}
