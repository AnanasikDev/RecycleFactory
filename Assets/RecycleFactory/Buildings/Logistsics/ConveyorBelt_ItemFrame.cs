using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace RecycleFactory.Buildings
{
    /// <summary>
    /// A fixed frame of a few items. Moved and stored as a single object, can be decomposed for merging with other frames.
    /// </summary>
    internal class ConveyorBelt_ItemFrame : MonoBehaviour
    {
        public static readonly int capacity = 3;
        public static readonly Pool<ConveyorBelt_ItemFrame> framesPool = new Pool<ConveyorBelt_ItemFrame>((ConveyorBelt_ItemFrame fr) => fr.gameObject.activeSelf);

        [SerializeField]
        [Tooltip("Local positions of slots in the frame where an item could be")]
        private Vector2[] slots;
        
        [ShowNonSerializedField]
        private ConveyorBelt_Item[] items;

        [HideInInspector] public ConveyorBelt_ItemFrame nextFrame;

        public void Init()
        {
            items = new ConveyorBelt_Item[capacity];

            Assert.IsTrue(slots.Length == capacity);
            Assert.IsTrue(items.Length == capacity);
        }

        public bool isFull()
        {
            return items.All(i => i != null);
        }

        public Vector2 GetFreeSlotPosition()
        {
            for (int i = 0; i < capacity; i++)
            {
                if (items[i] == null)
                    return slots[i];
            }
            return Vector2.zero;
        }

        public void Move(Vector3 direction)
        {
            transform.position += direction;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            foreach (var slot in slots)
            {
                Gizmos.DrawWireCube(transform.position + slot.ProjectTo3D(), Vector3.one / 3f);
            }
        }
    }
}
