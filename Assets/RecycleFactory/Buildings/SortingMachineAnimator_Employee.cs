using NaughtyAttributes;
using RecycleFactory.Buildings.Logistics;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.LightTransport.PostProcessing;

namespace RecycleFactory.Buildings
{
    internal class SortingMachineAnimator_Employee : SortingMachineAnimator
    {
        [SerializeField] private Animator animator;
        [SerializeField] private bool attachToOneTransform = true;
        [SerializeField][ShowIf("attachToOneTransform")] private Transform singleParent;

        [SerializeField][HideIf("attachToOneTransform")][Tooltip("An array of transforms, average position of which represent an item position")] private Transform[] averageHandlers;

        private bool isAnimating = false;

        private ConveyorBelt_Item item;
        private int anchorIndex;

        public override void Init()
        {
            base.Init();
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (!isAnimating || item == null) return;

            if (!attachToOneTransform)
            {
                Vector3 pos = Vector3.zero;
                foreach (var handler in averageHandlers)
                {
                    pos += handler.position;
                }
                item.transform.position = pos / averageHandlers.Length;
            }
        }

        public override void OnReceive(ConveyorBelt_Item item, int anchorIndex)
        {
            isReadyToReceive = false;
            this.item = item;
            this.anchorIndex = anchorIndex;
            if (attachToOneTransform)
                item.transform.SetParent(singleParent);
            else
            item.transform.localPosition = Vector3.zero;
            animator.SetTrigger("forward");
            isAnimating = true;
        }

        public override void OnRelease()
        {
            this.item = null;
            this.anchorIndex = -1;
            animator.SetTrigger("backward");
            isAnimating = true;
        }

        public override void Receive2ReleaseAnimEnded()
        {
            Debug.Log("Receive2Release anim ended");
            //onReadyToReleaseEvent?.Invoke(item, anchorIndex);

            isAnimating = false;
            IEnumerator tryUntil()
            {
                // try to release until succeed
                yield return new WaitUntil(() => sortingMachine.Release(item, anchorIndex));

                // success, item released
                OnRelease();
            }

            StartCoroutine(tryUntil());
        }
        
        public override void Release2ReceiveAnimEnded()
        {
            isAnimating = false;
            Debug.Log("Release2Receive anim ended");
            isReadyToReceive = true;
        }
    }
}
