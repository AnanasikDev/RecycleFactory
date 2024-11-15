using UnityEngine;
using System;
using RecycleFactory.UI;

namespace RecycleFactory.Player
{
    public class PlayerDemolisher : MonoBehaviour
    {
        internal Vector2Int selectedCell { get; private set; }

        private Func<bool> demolishTrigger = () => Input.GetMouseButtonDown(0) && !UIInputMask.isPointerOverUI;

        public void Init()
        {

        }

        public void _Update()
        {
            selectedCell = Scripts.PlayerController.GetSelectedCell();
            if (demolishTrigger())
            {
                Map.getBuildingAt(selectedCell)?.Demolish();
                // shake camera
                // play demolish SFX & VFX
            }
        }
    }
}
