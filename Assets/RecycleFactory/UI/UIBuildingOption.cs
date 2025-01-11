using NaughtyAttributes;
using RecycleFactory.Player;
using RecycleFactory.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RecycleFactory.UI
{
    public class UIBuildingOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [ReadOnly] public UIBuildingOption_Info info;
        [Required] public Image iconImage;
        [Required] public Image backgroundImage;

        private UIBuildingCategory category;

        public int vindex = 0;
        public static float optionHeight = 70;

        public Color defaultBackgroundColor;
        public Color hoverBackgroundColor;


        public static UIBuildingOption Create(UIBuildingOption_Info info, Vector2 position, UIBuildingCategory category)
        {
            UIBuildingOption option = Instantiate(UIController.UIBuildingOptionPrefab);
            option.category = category;
            option.transform.SetParent(category.expandPanel.transform);
            option.transform.localPosition = position;
            option.transform.localScale = Vector3.one;
            option.info = info;
            option.iconImage.sprite = info.sprite;
            option.iconImage.preserveAspect = true;
            category.onClosed += option.Shrink;
            option.Init();

            return option;
        }

        public void Init()
        {
            backgroundImage.color = defaultBackgroundColor;
        }

        public void Expand()
        {
            backgroundImage.color = hoverBackgroundColor;
            UIController.UIBuildingMenu.buildingDescription.UpdatePosition(transform.position + new Vector3(150, 0));
            UIController.UIBuildingMenu.buildingDescription.Enable($"{info.building.name}\n{info.building.description}\nCosts ${info.building.cost}");
        }

        public void Shrink()
        {
            backgroundImage.color = defaultBackgroundColor;
            UIController.UIBuildingMenu.buildingDescription.Disable();
        }

        public void Click()
        {
            Scripts.PlayerController.SetMode(Mode.Build);
            Scripts.PlayerBuilder.ForceSelectBuilding(info.building);
            category.Shrink();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => Expand();
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => Shrink();
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) => Click();
    }

    [System.Serializable]
    public class UIBuildingOption_Info
    {
        public RecycleFactory.Buildings.Building building;
        public Sprite sprite;
    }
}
