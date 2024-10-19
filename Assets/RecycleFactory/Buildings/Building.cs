using NaughtyAttributes;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    public abstract class Building : MonoBehaviour
    {
        [HideInInspector] public BuildingExtension_Receiver receiver;
        [HideInInspector] public BuildingExtension_Releaser releaser;

        public Vector2Int size = Vector2Int.one;

        [ShowNativeProperty] public int rotation { get; private set; }
        [ShowNativeProperty] public Vector2Int direction { get { return Utils.directions[rotation]; } }
        public Vector2Int mapPosition;

        public static event System.Action onAnyBuiltEvent;
        public static event System.Action onAnyDemolishedEvent;

        public void Init(Vector2Int mapPos)
        {
            mapPosition = mapPos;
            receiver = GetComponent<BuildingExtension_Receiver>();
            releaser = GetComponent<BuildingExtension_Releaser>();

            onAnyBuiltEvent?.Invoke();
        }

        public virtual void Rotate(int delta)
        {
            rotation = (int)Mathf.Repeat(rotation + delta, 4);  
            if (receiver != null) receiver.Rotate(delta);
            if (releaser != null) releaser.Rotate(delta);
            transform.Rotate(Vector3.up * delta * 90);
            size = Utils.Rotate(size, rotation).Abs();
        }

        private void OnDestroy()
        {
            // TODO: move to pool
            onAnyDemolishedEvent?.Invoke();
        }
    }
}