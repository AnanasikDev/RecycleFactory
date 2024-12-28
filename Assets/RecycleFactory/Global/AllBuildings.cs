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
                var val = buildingPrefabs.Find(b => b.gameObject.name == targetName);
                if (val == null)
                {
                    Debug.LogError($"[ReFa]: On Buildings Init: building with name ({targetName}) to fill in property ({prop.Name}) is not found.");
                    return;
                }
                prop.SetValue(this, val);
            }
            Debug.Log($"All {props.ToList().Count} Buildings initialized correctly");
        }

        [AutoSet] public static Building TrashProvider { get; private set; }
        [AutoSet] public static Building ConveyorBelt { get; private set; }
        [AutoSet] public static Building Incinerator { get; private set; }
        [AutoSet] public static Building MagneticSorter { get; private set; }
        [AutoSet] public static Building TransparencySorter { get; private set; }
        [AutoSet] public static Building PaperSorter { get; private set; }
        [AutoSet] public static Building DensitySorter { get; private set; }
        [AutoSet] public static Building ElectromagneticSorter { get; private set; }
        [AutoSet] public static Building MetalRecycler { get; private set; }
        [AutoSet] public static Building PlasticRecycler { get; private set; }
        [AutoSet] public static Building PaperRecycler { get; private set; }
        [AutoSet] public static Building BatteryRecycler { get; private set; }
    }

    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field, AllowMultiple = false)]
    internal class AutoSet : System.Attribute
    {

    }
}
