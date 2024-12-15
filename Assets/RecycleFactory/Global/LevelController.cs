using NaughtyAttributes;
using RecycleFactory.Buildings;
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

            levels = new Level[3];

            levels[0] = new Level()
            {
                CanBeUnlocked = () =>
                {
                    return true; // always unlocked
                },
                Unlock = () =>
                {
                    UnlockBuildings(new List<Building>() { AllBuildings.ConveyorBelt, AllBuildings.TrashProvider, AllBuildings.Incinerator });
                },
                id = 0
            };

            levels[1] = new Level()
            {
                CanBeUnlocked = () =>
                {
                    return StatisticsManager.totalItemsIncinerated > 5;
                },
                Unlock = () =>
                {
                    UnlockBuildings(new List<Building>() { AllBuildings.MetalRecycler, AllBuildings.MagneticSorter });
                },
                id = 1
            };

            levels[2] = new Level()
            {
                CanBeUnlocked = () =>
                {
                    return StatisticsManager.itemsRecycledByCategory[Buildings.Logistics.ItemCategories.Metal] > 20;
                },
                Unlock = () =>
                {
                    UnlockBuildings(new List<Building>() { AllBuildings.PaperSorter, AllBuildings.PaperRecycler });
                },
                id = 2
            };

            levels[0].Unlock();
        }

        private void UnlockBuildings(List<Building> buildings)
        {
            foreach (Building building in buildings)
            {
                buildingsStates[building] = true;
            }
        }
    }

    public class Level
    {
        public int id;
        public Func<bool> CanBeUnlocked;
        public Action Unlock;
    }
}
