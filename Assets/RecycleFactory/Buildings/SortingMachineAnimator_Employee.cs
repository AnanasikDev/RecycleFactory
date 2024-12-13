using RecycleFactory.Buildings.Logistics;
using System.Collections;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    internal class SortingMachineAnimator_Employee : SortingMachineAnimator
    {
        [SerializeField] private Animator animator;

        private ConveyorBelt_Item item;
        private int anchorIndex;

        public override void Init()
        {
            base.Init();
            animator = GetComponent<Animator>();
        }

        public override void OnReceive(ConveyorBelt_Item item, int anchorIndex)
        {
            this.item = item;
            this.anchorIndex = anchorIndex;
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
            Debug.Log("Prep ended");
            onReadyToReleaseEvent?.Invoke(item, anchorIndex);

            IEnumerator tryUntil() 
            {
                yield return new WaitUntil(() => sortingMachine.Release(item, anchorIndex));
                // success, item released
                OnRelease();
            }

            StartCoroutine(tryUntil());
        }
        
        public override void Release2ReceiveAnimEnded()
        {
            
        }
    }
}
