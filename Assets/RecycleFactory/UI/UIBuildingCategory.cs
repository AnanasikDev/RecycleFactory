using NaughtyAttributes;
using RecycleFactory.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RecycleFactory.UI
{
    public class UIBuildingCategory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public GameObject expandPanel;
        public float categoryHeight = 70;
        public Image categoryImageHolder;

        public List<UIBuildingOption> optionObjects = new List<UIBuildingOption>();
        [ReadOnly] public UIBuildingCategory_Info info;

        public event Action onOpened;
        public event Action onClosed;

        public void UpdateOptions()
        {
            int vindex = 0;
            foreach (UIBuildingOption option in optionObjects)
            {
                option.vindex = vindex;
                vindex++;
            }
        }

        public void Init()
        {
            categoryImageHolder.sprite = info.sprite;
            for (int i = 0; i < info.options.Length; i++)
            {
                var categoryObject = UIBuildingOption.Create(info.options[i], new Vector2(0, i * UIBuildingOption.optionHeight + categoryHeight), this);
                optionObjects.Add(categoryObject);
            }
            UpdateOptions();
        }

        private void UpdateLocked()
        {
            for (int i = 0; i < optionObjects.Count; i++)
            {
                if (Scripts.LevelController.IsUnlocked(optionObjects[i].info.building))
                {
                    optionObjects[i].gameObject.SetActive(true);
                }
                else
                {
                    optionObjects[i].gameObject.SetActive(false);
                }
            }
        }

        public void Expand()
        {
            UpdateLocked();
            expandPanel.SetActive(true);
            onOpened?.Invoke();
        }

        public void Shrink()
        {
            expandPanel.SetActive(false);
            onClosed?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData) => Expand();
        public void OnPointerExit(PointerEventData eventData) => Shrink();
    }

    [System.Serializable]
    public class UIBuildingCategory_Info
    {
        public string name;
        public UIBuildingOption_Info[] options;
        public Sprite sprite;
    }
}
