using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RecycleFactory.UI
{
    public class UIBuildingMenu_Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static bool isHoveringOverGUI = false;
        public static event Action onPointerEnterGUI;
        public static event Action onPointerExitGUI;
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            isHoveringOverGUI = true;
            onPointerEnterGUI?.Invoke();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            isHoveringOverGUI = false;
            onPointerExitGUI?.Invoke();
        }
    }
}