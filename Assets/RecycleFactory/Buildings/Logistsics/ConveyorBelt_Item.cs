using NaughtyAttributes;
using UnityEngine;

namespace RecycleFactory.Buildings.Logistics
{
    public class ConveyorBelt_Item : MonoBehaviour
    {
        public static int ID = 0;
        public int id;
        
        public static readonly float SCALE = 0.3f;
        private static Pool<ConveyorBelt_Item> itemsPool = new Pool<ConveyorBelt_Item>(item => item && item.gameObject.activeSelf);

        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public MeshFilter meshFilter;
        [HideInInspector] public ConveyorBelt_Driver currentDriver;

        [ReadOnly] public ConveyorBelt_Building holder;
        public ConveyorBelt_ItemInfo info;
        [Tooltip("Index of lane of the conveyor at which it is at now")] public int currentLaneIndex;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
        }

        public void Init(ConveyorBelt_ItemInfo info)
        {
            this.info = info;
            meshFilter.mesh = info.mesh;
            meshRenderer.materials = info.materials;  
            gameObject.name = "Item " + info.name;
            Enable();
        }

        /// <summary>
        /// Takes an unused object from pool (is possible) or instantiates a new one and registers it in the pool.
        /// </summary>
        public static ConveyorBelt_Item Create(ConveyorBelt_ItemInfo info)
        {
            itemsPool.TryTakeInactive(out ConveyorBelt_Item item);
            if (item == null)
            {
                item = new GameObject().AddComponent<ConveyorBelt_Item>();
                item.meshRenderer = item.gameObject.AddComponent<MeshRenderer>();
                item.meshFilter = item.gameObject.AddComponent<MeshFilter>();
                item.transform.localScale = Vector3.one * SCALE;
                item.id = ID++;
                itemsPool.RecordNew(item);
            }
            item.Init(info);
            return item;
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Disables the item in pool, detaches from its owning conveyor
        /// </summary>
        public void Disable()
        {
            holder.driver.RemoveItem(this);
            gameObject.transform.SetParent(null);
            gameObject.SetActive(false);
        }
    }
}
