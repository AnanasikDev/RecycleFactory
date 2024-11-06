using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RecycleFactory.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private string2Color[] colors;
        public static Dictionary<string, Color> nameToColor = new Dictionary<string, Color>();

        [Required][SerializeField] private UIBuildingCategory _UIBuildingCategoryPrefab;
        public static UIBuildingCategory UIBuildingCategoryPrefab;

        [Required][SerializeField] private UIBuildingOption _UIBuildingOptionPrefab;
        public static UIBuildingOption UIBuildingOptionPrefab;

        [Required][SerializeField] private UIBuildingMenu _UIBuildingMenu;
        public static UIBuildingMenu UIBuildingMenu;

        public static Color GetColor(string name) => nameToColor[name];
        public void Init()
        {
            foreach (var item in colors) nameToColor.Add(item.name, item.color);

            UIBuildingCategoryPrefab = _UIBuildingCategoryPrefab;
            UIBuildingOptionPrefab = _UIBuildingOptionPrefab;

            UIBuildingMenu = _UIBuildingMenu;
            UIBuildingMenu.Init();
        }
    }

    [Serializable]
    public class string2Color
    {
        public string name;
        public Color color;
    }
}