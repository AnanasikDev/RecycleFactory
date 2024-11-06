using NaughtyAttributes;
using RecycleFactory.UI;
using System.Collections.Generic;
using UnityEngine;

namespace RecycleFactory.UI
{
    public class UIBuildingMenu : MonoBehaviour
    {
        public UIBuildingCategory_Info[] Categories;
        private List<UIBuildingCategory> CategoryObjects = new List<UIBuildingCategory>();

        public float width = 140f;
        public AnchorPosition anchor;

        public Vector2 initialPosition = Vector2.zero;

        public Transform holderTransform;
        private RectTransform holder;
        [ReadOnly][SerializeField] private Vector3 initialHolderPosition;

        public UITooltip buildingDescription;

        public void Init()
        {
            holder = holderTransform.GetComponent<RectTransform>();

            for (int i = 0; i < Categories.Length; i++)
            {
                CreateCategory(Categories[i], i);
            }
            buildingDescription.Init();
            UIBuildingMenu_Hover.onPointerExitGUI += buildingDescription.Disable;
        }

        private Vector2 CalculatePosition(int i)
        {
            switch (anchor)
            {
                case AnchorPosition.Left:
                    return initialPosition + Vector2.right * width * i;
                case AnchorPosition.Center:
                    return initialPosition - new Vector2((Categories.Length - 1) * width / 2f, 0) + Vector2.right * width * i;
                case AnchorPosition.Right:
                    return initialPosition - Vector2.right * width * i;
            }
            return Vector2.zero;
        }

        private UIBuildingCategory CreateCategory(UIBuildingCategory_Info categoryInfo, int i)
        {
            UIBuildingCategory category = Instantiate(UIController.UIBuildingCategoryPrefab);
            category.transform.SetParent(holderTransform);
            category.transform.localPosition = CalculatePosition(i);
            category.transform.localScale = Vector3.one;
            category.info = categoryInfo;
            category.Init();

            CategoryObjects.Add(category);

            return category;
        }

        public void UpdateOptions()
        {
            foreach (var category in CategoryObjects) category.UpdateOptions();
        }
    }

    public enum AnchorPosition
    {
        Left,
        Center,
        Right
    }
}