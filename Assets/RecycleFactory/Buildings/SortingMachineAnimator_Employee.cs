using NaughtyAttributes;
using RecycleFactory.Buildings.Logistics;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    internal class SortingMachineAnimator_Employee : SortingMachineAnimator
    {
        [SerializeField] private Animator animator;
        [SerializeField] private bool attachToMultipleTransforms = true;
        [SerializeField][HideIf("attachToMultipleTransforms")] private Transform singleParent;

        [SerializeField][ShowIf("attachToMultipleTransforms")][Tooltip("An array of transforms, average position of which represent an item position")] private Transform[] averageHandlers;
        private int avgHandlersLength;
        [SerializeField] private bool updateRotation = false;

        private bool isAnimating = false;

        private ConveyorBelt_Item item;
        private int anchorIndex;

        private Building building;

        public override void Init()
        {
            base.Init();
            building = GetComponent<Building>();

            avgHandlersLength = averageHandlers.Length;
            animator = GetComponent<Animator>();

            building.onDemolishedEvent += OnDemolish;
        }

        private void OnDemolish()
        {
            building.onDemolishedEvent -= OnDemolish;
            item.DetachAndDisable();
        }

        private void Update()
        {
            if (!isAnimating || item == null) return;

            if (attachToMultipleTransforms)
            {
                Vector3 pos = averageHandlers.Select(t => t.transform.position).SumVectors();

                item.transform.position = pos / avgHandlersLength;
                if (updateRotation)
                {
                    if (avgHandlersLength > 2)
                    {
                        Debug.LogException(new System.Exception("Average handlers cannot evaluate average rotation"));
                        return;
                    }

                    item.transform.rotation = Quaternion.Slerp(averageHandlers[0].rotation, averageHandlers[1].rotation, 0.5f);
                }
            }
            // if attached to single, it is parented directly, inheriting all transform values
        }

        public override void OnReceive(ConveyorBelt_Item item, int anchorIndex)
        {
            isReadyToReceive = false;
            this.item = item;
            this.anchorIndex = anchorIndex;
            if (attachToMultipleTransforms)
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
