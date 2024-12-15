using RecycleFactory.Buildings.Logistics;
using System.Collections.Generic;

namespace RecycleFactory
{
    internal static class StatisticsManager
    {

        public static readonly Dictionary<ConveyorBelt_ItemInfo, int> itemsRecycled = new();
        public static int totalItemsIncinerated { get; private set; } = 0;

        /// <summary>
        /// For each item there is a sum of money earned by recycling this type of item
        /// </summary>
        public static readonly Dictionary<ConveyorBelt_ItemInfo, int> itemsRecycledMoneyEarned = new();
        public static int totalMoneyEarnedFromRecycling { get; private set; } = 0;


        public static readonly Dictionary<ItemCategories, int> itemsRecycledByCategory = new();
        
        public static void Init()
        {
            ConveyorBelt_Item.onItemRecycledEvent += (ConveyorBelt_ItemInfo info, int bonus) =>
            {
                itemsRecycled.Add(info, itemsRecycled.TryGetValue(info, out int val) ? val + 1 : 1);
                itemsRecycledMoneyEarned.Add(info, itemsRecycledMoneyEarned.TryGetValue(info, out int val2) ? val2 + bonus : bonus);
                itemsRecycledByCategory.Add(info.category, itemsRecycledByCategory.TryGetValue(info.category, out int val3) ? val3 + 1 : 1);
                totalMoneyEarnedFromRecycling += bonus;
            };

            ConveyorBelt_Item.onItemIncineratedEvent += (ConveyorBelt_ItemInfo info, int bonus) =>
            {
                totalItemsIncinerated++;
                totalMoneyEarnedFromRecycling += bonus;
            };
        }
    }
}
