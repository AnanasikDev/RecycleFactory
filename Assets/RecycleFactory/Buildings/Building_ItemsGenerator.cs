using System.Collections.Generic;
using UnityEngine;

public class Building_ItemsGenerator : Building
{
    [SerializeField] private List<ConveyorBelt_ItemInfo> itemInfos;
    [SerializeField] private ConveyorBelt_Building target;

    private void Update()
    {
        if (target.first.isEmpty)
        {
            target.first.SetItem(ConveyorBelt_Item.Create(itemInfos.RandomElement()));
        }
    }
}