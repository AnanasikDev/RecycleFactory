using System;
using System.Collections.Generic;
using UnityEngine;

namespace RecycleFactory.Buildings.Logistics
{
    /// <summary>
    /// Component that can be attached to Building_SortingMachine, which is automatically then
    /// considered from there and responsibility for correct item transfer moves to SortingMachineAnimator.
    /// SortingMachineAnimator is abstract because each specific machine or group of machines
    /// have their unique animations and since item ownership and transform are dependend on them,
    /// animations are easier to be controlled via code, hence different implementation classes for different
    /// machines.
    /// 
    /// SortingMachineAnimator encapsulates OnReceive method, used by Building_SortingMachine (when it receives an item, it transfers item ownership to animator) 
    /// </summary>
    internal abstract class SortingMachineAnimator : MonoBehaviour
    {
        protected Building_SortingMachine sortingMachine;

        public bool isReadyToReceive = true;

        public Action<ConveyorBelt_Item, int> onReadyToReleaseEvent;
        public Action<ConveyorBelt_Item, int> onReadyToReceiveEvent;

        public virtual void Init()
        {
            sortingMachine = GetComponent<Building_SortingMachine>();
        }

        /// <summary>
        /// Item has just been received, begin preparation for release (receive-release animation)
        /// </summary>
        public abstract void OnReceive(ConveyorBelt_Item item, int anchorIndex);

        /// <summary>
        /// Receive->Release animation ended, it's time to release
        /// </summary>
        public abstract void Receive2ReleaseAnimEnded();

        /// /// <summary>
        /// Item has just been released, begin preparation for receive (release->receive animation)
        /// </summary>
        public abstract void OnRelease();

        /// <summary>
        /// Release->Receive animation ended, it's time to receive
        /// </summary>
        public abstract void Release2ReceiveAnimEnded();
    }
}
