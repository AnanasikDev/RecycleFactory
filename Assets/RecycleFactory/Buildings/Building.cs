using NaughtyAttributes;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    public abstract class Building : MonoBehaviour
    {
        [HideInInspector] public BuildingExtension_Receiver receiver;
        [HideInInspector] public BuildingExtension_Releaser releaser;

        [MinMaxSlider(1, 4)] public Vector2Int size = Vector2Int.one;

        [ShowNativeProperty] public int rotation { get; private set; }

        public Vector2Int mapPosition;

        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        public static event System.Action onAnyBuiltEvent;
        public static event System.Action onAnyDemolishedEvent;

        public void Init(Vector2Int mapPos)
        {
            mapPosition = mapPos;

            receiver = GetComponent<BuildingExtension_Receiver>();
            releaser = GetComponent<BuildingExtension_Releaser>();

            if (receiver) receiver.Init();
            if (releaser) releaser.Init(this);

            Map.RegisterNewBuilding(this);

            PostInit();

            onAnyBuiltEvent?.Invoke();
        }

        protected virtual void PostInit() { }

        public virtual void Rotate(int delta)
        {
            rotation = (int)Mathf.Repeat(rotation + delta, 4);  
            size = Utils.RotateXY(size, delta);

            transform.Rotate(Vector3.up * delta * 90);

            if (receiver != null) receiver.Rotate(delta);
            if (releaser != null) releaser.Rotate(delta);
        }

        private void OnDestroy()
        {
            // TODO: move to pool
            Map.RemoveBuilding(this);

            onAnyDemolishedEvent?.Invoke();
        }
    }
}