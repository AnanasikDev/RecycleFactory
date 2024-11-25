using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    public class AllBuildings : MonoBehaviour
    {
        [SerializeField] private List<Building> buildingPrefabs;
        private readonly string namingFormat = "Building_{0}";

        public static List<Building> allBuildings;

        [Button("Check (does not set any values)")]
        public void Init()
        {
            allBuildings = buildingPrefabs;

            var props = typeof(AllBuildings).GetProperties().Where(x => x.GetCustomAttributes(typeof(AutoSet), false).Any());

            foreach (var prop in props)
            {
                string targetName = string.Format(namingFormat, prop.Name);
                var val = buildingPrefabs.Find(b => b.name == targetName);
                if (val == null)
                {
                    Debug.LogError($"[ReFa]: On Buildings Init: building with name ({targetName}) to fill in property ({prop.Name}) is not found.");
                    return;
                }
                prop.SetValue(this, val);
            }
            Debug.Log($"All {props.ToList().Count} Buildings initialized correctly");
        }

        [AutoSet] public static Building ItemsGenerator { get; private set; }
        [AutoSet] public static Building ConveyorBelt { get; private set; }
        [AutoSet] public static Building CameraSorter { get; private set; }
        [AutoSet] public static Building MagneticSorter { get; private set; }
        [AutoSet] public static Building EmployeeSorter { get; private set; }
        [AutoSet] public static Building NeutralIncinerator { get; private set; }
        [AutoSet] public static Building MetalIncinerator { get; private set; }
        [AutoSet] public static Building PlasticIncinerator { get; private set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = false)]
    internal class AutoSet : System.Attribute
    {

    }
}
