using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

namespace RecycleFactory.Buildings
{
    [RequireComponent(typeof(BuildingRenderer))]
    public abstract class Building : MonoBehaviour
    {
        private static int _id = -1;
        protected int id;

        public new string name;
        public string description;
        public int cost;

        [HideInInspector] public bool isAlive = true;

        [HideInInspector] public BuildingExtension_Receiver receiver;
        [HideInInspector] public BuildingExtension_Releaser releaser;
        public BuildingRenderer buildingRenderer;

        public Vector2Int size = Vector2Int.one;
        public Vector2Int shift = Vector2Int.zero;

        [ShowNativeProperty] public int rotation { get; private set; }

        public Vector2Int mapPosition;

        public static event System.Action onAnyBuiltEvent;
        public static event System.Action onAnyDemolishedEvent;

        public event System.Action onDemolishedEvent;

        public void Init(Vector2Int mapPos, int selectedRotation)
        {
            // rotation has been done beforehand in PlayerBuilder

            id = ++_id;
            gameObject.name += " " + id;
            mapPosition = mapPos;
            isAlive = true;

            Assert.IsTrue(buildingRenderer != null, "[ReFa]: BuildingRenderer must be not null!");
            receiver = GetComponent<BuildingExtension_Receiver>();
            releaser = GetComponent<BuildingExtension_Releaser>();
            buildingRenderer.Init();

            Rotate(selectedRotation);

            if (receiver) receiver.Init(this, selectedRotation);
            if (releaser) releaser.Init(this, selectedRotation);

            Map.RegisterNewBuilding(this);

            PostInit();

            onAnyBuiltEvent?.Invoke();
        }

        protected virtual void PostInit() { }

        public virtual void Rotate(int delta)
        {
            rotation = (int)Mathf.Repeat(rotation + delta, 4);  
            size = Utils.RotateXY(size, delta);
            shift = Utils.RotateXY(shift, delta);

            transform.Rotate(Vector3.up * delta * 90);
        }

        public void Demolish()
        {
            isAlive = false;
            Map.RemoveBuilding(this);
            OnDemolish();

            onDemolishedEvent?.Invoke();
            onAnyDemolishedEvent?.Invoke();
            Destroy(gameObject);
        }

        protected virtual void OnDemolish() { }

        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying == false) return;

            var pos = mapPosition + shift;
            for (int _x = 0; _x < Mathf.Abs(size.x); _x++)
            {
                for (int _y = 0; _y < Mathf.Abs(size.y); _y++)
                {
                    int ypos = pos.y + _y * (int)Mathf.Sign(size.y);
                    int xpos = pos.x + _x * (int)Mathf.Sign(size.x);

                    bool isFree = Map.buildingsAt[pos.y + _y * (int)Mathf.Sign(size.y), pos.x + _x * (int)Mathf.Sign(size.x)] == null;
                    Gizmos.color = isFree ? Color.white : Color.black;

                    Gizmos.DrawCube(new Vector3(xpos, 0, ypos), Vector3.one / 2f);
                }
            }
        }
    }
}