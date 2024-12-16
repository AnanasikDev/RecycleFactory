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
                itemsRecycled[info] = itemsRecycled.ContainsKey(info) ? itemsRecycled[info] + 1 : 1;

                itemsRecycledMoneyEarned[info] = itemsRecycledMoneyEarned.ContainsKey(info) ? itemsRecycledMoneyEarned[info] + bonus : bonus;

                itemsRecycledByCategory[info.category] = itemsRecycledByCategory.ContainsKey(info.category) ? itemsRecycledByCategory[info.category] + 1 : 1;

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
