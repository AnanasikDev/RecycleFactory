using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

namespace RecycleFactory.UI
{
    public class UIInputMask : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public bool isPointerInside { get; private set; }
        public bool isPointerClick { get; private set; }

        public event Action onPointerClickedEvent;
        public event Action onPointerEnterEvent;
        public event Action onPointerExitEvent;

        public static List<UIInputMask> masks = new List<UIInputMask>();
        public static bool isPointerOverUI { get { return masks.Any(m => m.isPointerInside); } }

        private void Start()
        {
            masks.Add(this);
        }

        void OnDisable()
        {
            isPointerInside = false;
            onPointerExitEvent?.Invoke();
        }

        void OnDestroy()
        {
            isPointerInside = false;
            onPointerExitEvent?.Invoke();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            isPointerInside = true;
            onPointerEnterEvent?.Invoke();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            isPointerInside = false;
            onPointerExitEvent?.Invoke();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            isPointerClick = true;
            onPointerClickedEvent?.Invoke();
        }
    }
}