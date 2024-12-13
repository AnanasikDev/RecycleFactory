using RecycleFactory.Buildings.Logistics;
using System.Collections;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    internal class SortingMachineAnimator_Employee : SortingMachineAnimator
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Transform handTransform;

        private ConveyorBelt_Item item;
        private int anchorIndex;

        public override void Init()
        {
            base.Init();
            animator = GetComponent<Animator>();
        }

        public override void OnReceive(ConveyorBelt_Item item, int anchorIndex)
        {
            isReadyToReceive = false;
            this.item = item;
            this.anchorIndex = anchorIndex;
            item.transform.SetParent(handTransform);
            item.transform.localPosition = Vector3.zero;
            animator.SetTrigger("forward");
        }

        public override void OnRelease()
        {
            this.item = null;
            this.anchorIndex = -1;
            animator.SetTrigger("backward");
        }

        public override void Receive2ReleaseAnimEnded()
        {
            Debug.Log("Receive2Release anim ended");
            //onReadyToReleaseEvent?.Invoke(item, anchorIndex);

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
            Debug.Log("Release2Receive anim ended");
            isReadyToReceive = true;
        }
    }
}
