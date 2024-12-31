using NaughtyAttributes;
using RecycleFactory.Buildings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ItemInfo = RecycleFactory.Buildings.Logistics.ConveyorBelt_ItemInfo;

namespace RecycleFactory
{
    public class AllItems : MonoBehaviour
    {

        [SerializeField] private List<ItemInfo> _itemInfos;
        private readonly string namingFormat = "{0}";

        public static List<ItemInfo> allItemInfos;

        [Button("Check (does not set any values)")]
        public void Init()
        {
            allItemInfos = _itemInfos;

            var props = typeof(AllItems).GetProperties().Where(x => x.GetCustomAttributes(typeof(AutoSet), false).Any());

            foreach (var prop in props)
            {
                string targetName = string.Format(namingFormat, prop.Name.ToLower());
                var val = _itemInfos.Find(info => info.name == targetName);
                if (val == null)
                {
                    Debug.LogError($"[ReFa]: On Items Init: iteminfo with name ({targetName}) to fill in property ({prop.Name}) is not found.");
                    return;
                }
                prop.SetValue(this, val);
            }
            Debug.Log($"All {props.ToList().Count} Items initialized correctly");
        }

        [AutoSet] public static ItemInfo Apple { get; private set; }
        [AutoSet] public static ItemInfo Banana { get; private set; }
        [AutoSet] public static ItemInfo Box { get; private set; }
        [AutoSet] public static ItemInfo Bolt { get; private set; }
        [AutoSet] public static ItemInfo Battery { get; private set; }
        [AutoSet] public static ItemInfo Handle { get; private set; }
        [AutoSet] public static ItemInfo Bottle { get; private set; }
        [AutoSet] public static ItemInfo Lock { get; private set; }
        [AutoSet] public static ItemInfo Book { get; private set; }
        [AutoSet] public static ItemInfo Yoghurt { get; private set; }
        [AutoSet] public static ItemInfo Lightbulb { get; private set; }
    }
}
