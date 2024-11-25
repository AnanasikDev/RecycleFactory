using RecycleFactory.Buildings;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RecycleFactory.UI
{
    public class LevelController : MonoBehaviour
    {
        public Level[] levels;

        public void Init()
        {
            levels = new Level[2];

            levels[0] = new Level()
            {
                CanBeUnlocked = () =>
                {
                    return true;
                },
                Unlock = () =>
                {
                    UnlockBuildings(new List<Building>() {  });
                },
                id = 0
            };
        }

        private void UnlockBuildings(List<Building> buildings)
        {

        }
    }

    public class Level
    {
        public int id;
        public Func<bool> CanBeUnlocked;
        public Action Unlock;
    }
}
