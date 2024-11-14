using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

namespace RecycleFactory.UI
{
    public class UIInputMask : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public bool isPointerInside { get; private set; }
        public bool isPointerDown { get; private set; }

        public event Action onPointerDownEvent;
        public event Action onPointerUpEvent;
        public event Action onPointerEnterEvent;
        public event Action onPointerExitEvent;

        public static List<UIInputMask> masks = new List<UIInputMask>();
        public static bool isPointerOverUI { get { return masks.Any(m => m.isPointerInside); } }

        private void Start()
        {
            masks.Add(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isPointerInside = true;
            onPointerEnterEvent?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerInside = false;
            onPointerExitEvent?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerDown = true;
            onPointerDownEvent?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerDown = false;
            onPointerUpEvent?.Invoke();
        }
    }
}