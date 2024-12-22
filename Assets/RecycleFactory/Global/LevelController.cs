using NaughtyAttributes;
using RecycleFactory.Buildings;
using RecycleFactory.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RecycleFactory
{
    public class LevelController : MonoBehaviour
    {
        public Level[] levels;
        [ShowNativeProperty] public int currentLevel { get; private set; } = 0;

        private Dictionary<Building, bool> buildingsStates;

        [SerializeField] private UIProgressBar progressBar;
        private float progressToNext = 0.0f;

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
            if (currentLevel < levels.Length)
            {
                levels[currentLevel + 1].Unlock();
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
            if (currentLevel < levels.Length)
            {
                for (int i = currentLevel + 1; i < levels.Length; i++)
                {
                    levels[i].Unlock();
                }
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

            levels = new Level[4];

            levels[0] = new Level()
            {
                GetProgress = () =>
                {
                    return 1; // always unlocked
                },
                Unlock = () =>
                {
                    UnlockBuildings(new List<Building>() { AllBuildings.ConveyorBelt, AllBuildings.TrashProvider, AllBuildings.Incinerator });
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
                    UnlockBuildings(new List<Building>() { AllBuildings.MetalRecycler, AllBuildings.MagneticSorter });
                    Scripts.Budget.Add(6000);
                },
                id = 1
            };

            levels[2] = new Level()
            {
                GetProgress = () =>
                {
                    return StatisticsManager.itemsRecycledByCategory.TryGetValue(Buildings.Logistics.ItemCategories.Metal, out var val) ? val / 25f : 0;
                },
                Unlock = () =>
                {
                    UnlockBuildings(new List<Building>() { AllBuildings.PaperSorter, AllBuildings.PaperRecycler });
                    Scripts.Budget.Add(3000);
                },
                id = 2
            };

            levels[3] = new Level()
            {
                GetProgress = () =>
                {
                    return StatisticsManager.itemsRecycledByCategory.TryGetValue(Buildings.Logistics.ItemCategories.Paper, out var val) ? val / 50f : 0;
                },
                Unlock = () =>
                {
                    UnlockBuildings(new List<Building>() { AllBuildings.TransparencySorter, AllBuildings.PlasticRecycler });
                    Scripts.Budget.Add(1000);
                },
                id = 3
            };

            levels[0].Unlock();
            progressBar.SetValue(0);
        }

        private void UnlockBuildings(List<Building> buildings)
        {
            foreach (Building building in buildings)
            {
                buildingsStates[building] = true;
            }
        }

        private void Update()
        {
            if (currentLevel == levels.Length - 1) return;

            progressToNext = levels[currentLevel + 1].GetProgress();
            progressBar.SetValue(progressToNext);
            if (progressToNext >= threshold)
            {
                levels[currentLevel + 1].Unlock();
                currentLevel++;
            }
        }
    }

    public class Level
    {
        public int id;
        public Func<float> GetProgress;
        public Action Unlock;
    }
}
