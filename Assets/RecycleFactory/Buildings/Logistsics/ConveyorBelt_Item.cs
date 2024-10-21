using UnityEngine;

namespace RecycleFactory.Buildings
{
    public class ConveyorBelt_Item : MonoBehaviour
    {
        private static Pool<ConveyorBelt_Item> itemsPool = new Pool<ConveyorBelt_Item>(item => item.gameObject.activeSelf);

        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public MeshFilter meshFilter;
        [HideInInspector] public ConveyorBelt_Element currentElement;

        public ConveyorBelt_ItemInfo info;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
        }

        public void Init(ConveyorBelt_ItemInfo info)
        {
            this.info = info;
            meshFilter.mesh = info.mesh;
            gameObject.name = "Item " + info.name;
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
                item.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                itemsPool.RecordNew(item);
            }
            item.Init(info);
            return item;
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
