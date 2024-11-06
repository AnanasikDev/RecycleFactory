using NaughtyAttributes;
using NUnit.Framework;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    [RequireComponent(typeof(BuildingRenderer))]
    public abstract class Building : MonoBehaviour
    {
        private static int _id = -1;
        protected int id;

        public string description;

        [HideInInspector] public BuildingExtension_Receiver receiver;
        [HideInInspector] public BuildingExtension_Releaser releaser;
        public BuildingRenderer buildingRenderer;

        [MinMaxSlider(1, 4)] public Vector2Int size = Vector2Int.one;

        [ShowNativeProperty] public int rotation { get; private set; }

        public Vector2Int mapPosition;

        public static event System.Action onAnyBuiltEvent;
        public static event System.Action onAnyDemolishedEvent;

        public void Init(Vector2Int mapPos)
        {
            id = ++_id;
            gameObject.name += " " + id;
            mapPosition = mapPos;

            Assert.That(buildingRenderer != null);
            buildingRenderer.Init();
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