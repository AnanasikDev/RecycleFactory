using UnityEngine;

namespace RecycleFactory.Buildings
{
    public class ConveyorBelt_Item : MonoBehaviour
    {
        private static Pool<ConveyorBelt_Item> itemsPool = new Pool<ConveyorBelt_Item>(item => item.gameObject.activeSelf);

        [HideInInspector] public MeshRenderer meshRenderer;
        [HideInInspector] public MeshFilter meshFilter;
        [HideInInspector] public ConveyorBelt_Element currentElement;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
        }

        public void Init(ConveyorBelt_ItemInfo info)
        {
            meshFilter.mesh = info.mesh;
            gameObject.name = "Item " + info.name;
        }

        public static ConveyorBelt_Item Create(ConveyorBelt_ItemInfo info)
        {
            itemsPool.TryTakeInactive(out ConveyorBelt_Item item);
            if (item == null)
            {
                item = new GameObject().AddComponent<ConveyorBelt_Item>();
                item.meshRenderer = item.gameObject.AddComponent<MeshRenderer>();
                item.meshFilter = item.gameObject.AddComponent<MeshFilter>();
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
