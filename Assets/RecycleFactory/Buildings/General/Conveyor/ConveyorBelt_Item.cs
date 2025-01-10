using NaughtyAttributes;
using System;
using UnityEngine;

namespace RecycleFactory.Buildings.Logistics
{
    public class ConveyorBelt_Item : MonoBehaviour
    {
        public static int ID = 0;
        public int id;

        public static readonly bool enableShadowcasting = false;
        
        public static readonly float SCALE = 0.3f;
        private static Pool<ConveyorBelt_Item> itemsPool = new Pool<ConveyorBelt_Item>(item => item && item.gameObject.activeSelf);

        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public MeshFilter meshFilter;
        [HideInInspector] public ConveyorBelt_Driver currentDriver;

        [ReadOnly] public ConveyorBelt_Building holder;
        public ConveyorBelt_ItemInfo info;
        [Tooltip("Index of lane of the conveyor at which it is at now")] public int currentLaneIndex;

        public static event Action<ConveyorBelt_ItemInfo, int> onItemIncineratedEvent;
        public static event Action<ConveyorBelt_ItemInfo, int> onItemRecycledEvent;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
        }

        public void Init(ConveyorBelt_ItemInfo info)
        {
            this.info = info;
            transform.localScale = Vector3.one * SCALE;
            meshFilter.mesh = info.mesh;
            meshRenderer.materials = info.materials;  
            gameObject.name = "Item_" + info.name + "_[" + id + "]";
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
                item.meshRenderer.shadowCastingMode = enableShadowcasting ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
                item.id = ID++;
                item.transform.rotation = Quaternion.identity;
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
        public void DetachAndDisable()
        {
            Detach();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Detaches from its owning conveyor
        /// </summary>
        public void Detach()
        {
            holder?.driver?.RemoveItem(this);
            gameObject.transform.SetParent(null);
        }

        public void MarkIncinerated(int moneyBonus)
        {
            onItemIncineratedEvent?.Invoke(info, moneyBonus);
        }

        public void MarkRecycled(int moneyBonus)
        {
            onItemRecycledEvent?.Invoke(info, moneyBonus);
        }
    }
}
