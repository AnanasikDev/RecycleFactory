using NaughtyAttributes;
using RecycleFactory.Buildings;
using RecycleFactory.Buildings.Logistics;
using RecycleFactory.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace RecycleFactory
{
    public class LevelController : MonoBehaviour
    {
        public Level[] levels;
        [ShowNativeProperty] public int levelInProgress { get; private set; } = 0;

        private Dictionary<Building, bool> buildingsStates;
        private Dictionary<ConveyorBelt_ItemInfo, bool> itemsStates;

        public List<ConveyorBelt_ItemInfo> unlockedItems = new List<ConveyorBelt_ItemInfo>();

        [SerializeField] private UIProgressBar progressBar;
        private float progressToNext = 0.0f;

        [SerializeField] private TextMeshProUGUI nextStageDescription;

        private const float threshold = 1.0f - 10e-4f;

        #region inspector_helpers

        [Button("Unlock next")]
        internal void cmd_UnlockNext()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Unavailable in edit mode.");
                return;
            }
            if (levelInProgress < levels.Length - 1)
            {
                levels[levelInProgress + 1].Unlock();
                levelInProgress++;
            }
        }

        [Button("Unlock all levels")]
        internal void cmd_UnlockAllLevels()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Unavailable in edit mode.");
                return;
            }

            for (; levelInProgress < levels.Length; levelInProgress++)
            {
                levels[levelInProgress].Unlock();
            }
        }

        [Button("Unlock all buildings")]
        internal void cmd_UnlockAllBuildings()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Unavailable in edit mode.");
                return;
            }
            foreach (Building building in AllBuildings.allBuildings)
            {
                buildingsStates[building] = true;
            }
        }

        #endregion inspector_helpers

        public bool IsUnlocked(Building building)
        {
            return buildingsStates[building];
        }

        public void Init()
        {
            buildingsStates = AllBuildings.allBuildings.ToDictionary(x => x, x => false);
            itemsStates = AllItems.allItemInfos.ToDictionary(x => x, x => false);

            levels = new Level[6];

            levels[0] = new Level()
            {
                GetProgress = () =>
                {
                    return 1; // always unlocked
                },
                Unlock = () =>
                {
                    UnlockBuildings(new List<Building>() { AllBuildings.ConveyorBelt, AllBuildings.TrashProvider, AllBuildings.Incinerator });
                    UnlockItems(new List<ConveyorBelt_ItemInfo>() { AllItems.Apple, AllItems.Bottle, AllItems.Box, AllItems.Bolt, AllItems.Handle });
                },
                GetDescription = () =>
                {
                    return "";
                },
                id = 0
            };

            levels[1] = new Level()
            {
                GetProgress = () =>
                {
                    return StatisticsManager.totalItemsIncinerated / 12f;
                },
                Unlock = () =>
                {
                    UnlockBuildings(new List<Building>() { AllBuildings.PaperRecycler, AllBuildings.PaperSorter });
                    UnlockItems(new List<ConveyorBelt_ItemInfo>() { AllItems.Book });
                    Scripts.Budget.Add(5400);
                },
                GetDescription = () =>
                {
                    return $"Next level: 2\n" +
                           $"Items collected: {StatisticsManager.totalItemsIncinerated}/12\n" +
                           $"Unlocks Paper\n" +
                           $"New items: book";
                },
                id = 1
            };

            levels[2] = new Level()
            {
                GetProgress = () =>
                {
                    return GetItemCategoryProgress(ItemCategories.Paper, 20);
                },
                Unlock = () =>
                {
                    UnlockBuildings(new List<Building>() { AllBuildings.MetalRecycler, AllBuildings.MagneticSorter });
                    UnlockItems(new List<ConveyorBelt_ItemInfo>() { AllItems.Lock });
                    Scripts.Budget.Add(3200);
                },
                GetDescription = () =>
                {
                    return $"Next level: 3\n" +
                           $"Metal recycled: {GetItemCategoryAmount(ItemCategories.Paper)}/20\n" +
                           $"Unlocks Metal\n" +
                           $"New items: lock";
                },
                id = 2
            };

            levels[3] = new Level()
            {
                GetProgress = () =>
                {
                    return GetItemCategoryProgress(ItemCategories.Paper, 40) * 4/7f + 
                    GetItemCategoryProgress(ItemCategories.Metal, 30) * 3/7f;
                },
                Unlock = () =>
                {
                    UnlockBuildings(new List<Building>() { AllBuildings.TransparencySorter, AllBuildings.PlasticRecycler });
                    Scripts.Budget.Add(1100);
                },
                GetDescription = () =>
                {
                    return $"Next level: 4\n" +
                            $"Paper recycled: {GetItemCategoryAmount(ItemCategories.Paper)}/40\n" +
                            $"Metal recycled: {GetItemCategoryAmount(ItemCategories.Metal)}/30\n" +
                            $"Unlocks Plastic\n" +
                            $"New items: yoghurt";
                },
                id = 3
            };

            levels[4] = new Level()
            {
                GetProgress = () =>
                {
                    return GetItemCategoryProgress(ItemCategories.Paper, 60) * 6/13f + 
                    GetItemCategoryProgress(ItemCategories.Metal, 50) * 5/13f + 
                    GetItemCategoryProgress(ItemCategories.Plastic, 20) * 2/13f;
                },
                Unlock = () =>
                {
                    UnlockBuildings(new List<Building>() { AllBuildings.ElectromagneticSorter, AllBuildings.DensitySorter, AllBuildings.BatteryRecycler });
                    UnlockItems(new List<ConveyorBelt_ItemInfo>() { AllItems.Battery, AllItems.Lightbulb });
                    Scripts.Budget.Add(1000);
                },
                GetDescription = () =>
                {
                    return $"Next level: 5\n" +
                            $"Paper recycled: {GetItemCategoryAmount(ItemCategories.Paper)}/60\n" +
                            $"Metal recycled: {GetItemCategoryAmount(ItemCategories.Metal)}/50\n" +
                            $"Plastic recycled: {GetItemCategoryAmount(ItemCategories.Plastic)}/20\n" +
                            $"Unlocks Batteries\n" +
                            $"New items: battery, lightbulb";
                },
                id = 4
            };

            levels[5] = new Level()
            {
                GetProgress = () =>
                {
                    return GetItemCategoryProgress(ItemCategories.Paper, 70) * 70/225f + 
                    GetItemCategoryProgress(ItemCategories.Metal, 70) * 70/225f + 
                    GetItemCategoryProgress(ItemCategories.Plastic, 70) * 70 / 225f +
                    GetItemCategoryProgress(ItemCategories.Battery, 15) * 15/225f;
                },
                Unlock = () =>
                {
                    UnlockBuildings(new List<Building>() { AllBuildings.LightbulbRecycler });
                    UnlockItems(new List<ConveyorBelt_ItemInfo>() { AllItems.Banana });
                    Scripts.Budget.Add(2200);
                },
                GetDescription = () =>
                {
                    return $"Next level: 6\n" +
                            $"Paper recycled: {GetItemCategoryAmount(ItemCategories.Paper)}/60\n" +
                            $"Metal recycled: {GetItemCategoryAmount(ItemCategories.Metal)}/50\n" +
                            $"Plastic recycled: {GetItemCategoryAmount(ItemCategories.Plastic)}/20\n" +
                            $"Unlocks Batteries\n" +
                            $"New items: banana";
                },
                id = 5
            };

            levels[0].Unlock();
            levelInProgress++;
            progressBar.SetValue(0);
        }

        private void UnlockBuildings(List<Building> buildings)
        {
            foreach (Building building in buildings)
            {
                buildingsStates[building] = true;
            }
        }

        private void UnlockItems(List<ConveyorBelt_ItemInfo> items)
        {
            foreach (ConveyorBelt_ItemInfo item in items)
            {
                itemsStates[item] = true;
                unlockedItems.Add(item);
            }
        }

        private float GetItemCategoryProgress(ItemCategories category, int targetAmount, bool scale01 = true)
        {
            return Mathf.Clamp01(StatisticsManager.itemsRecycledByCategory.TryGetValue(category, out int val) ? 
                val / (float)targetAmount : 
                0) * (scale01 ? 1 : targetAmount);
        }
        private float GetItemCategoryAmount(ItemCategories category)
        {
            return StatisticsManager.itemsRecycledByCategory.TryGetValue(category, out int val) ?
                val : 0;
        }

        private void Update()
        {
            if (levelInProgress > levels.Length - 1) return;

            progressToNext = levels[levelInProgress].GetProgress();
            progressBar.SetValue(progressToNext);
            nextStageDescription.text = levels[levelInProgress].GetDescription();

            if (progressToNext >= threshold)
            {
                levels[levelInProgress].Unlock();
                levelInProgress++;
            }
        }
    }

    public class Level
    {
        public int id;
        public Func<float> GetProgress;
        public Action Unlock;
        /// <summary>
        /// Shown on the corresponding panel on HUD until the level is researched
        /// </summary>
        public Func<string> GetDescription;
    }
}
