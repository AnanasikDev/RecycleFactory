using RecycleFactory.Buildings;
using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

namespace RecycleFactory
{
    public class LevelController : MonoBehaviour
    {
        public Level[] levels;
        [ShowNativeProperty] public int currentLevel { get; private set; } = 0;

        private Dictionary<Building, bool> buildingsStates;

        public bool IsUnlocked(Building building)
        {
            return buildingsStates[building];
        }

        [Button]
        internal void UnlockNext()
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

        public void Init()
        {
            buildingsStates = AllBuildings.allBuildings.ToDictionary(x => x, x => false);

            levels = new Level[2];

            levels[0] = new Level()
            {
                CanBeUnlocked = () =>
                {
                    return true;
                },
                Unlock = () =>
                {
                    UnlockBuildings(new List<Building>() { AllBuildings.ConveyorBelt, AllBuildings.ItemsGenerator, AllBuildings.NeutralIncinerator });
                },
                id = 0
            };

            levels[1] = new Level()
            {
                CanBeUnlocked = () =>
                {
                    return true;
                },
                Unlock = () =>
                {
                    UnlockBuildings(new List<Building>() { AllBuildings.MetalIncinerator, AllBuildings.MagneticSorter });
                },
                id = 1
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
