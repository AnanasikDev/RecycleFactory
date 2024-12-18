﻿using RecycleFactory.Buildings.Logistics;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    [RequireComponent(typeof(BuildingExtension_Receiver))]
    public class BuildingIncinerator : Building
    {
        [Range(-100, 100)] public float metaillicBonus;
        [Range(-100, 100)] public float plasticBonus;
        [Range(-100, 100)] public float organicBonus;
        [Range(-100, 100)] public float paperBonus;

        private int inAnchorsCount;

        protected override void PostInit()
        {
            inAnchorsCount = receiver?.inAnchors?.Count ?? 0;
        }

        private void Update()
        {
            Incinerate();
        }

        private void Incinerate()
        {
            for (int a = 0; a < inAnchorsCount; a++)
            {
                // try receive an item
                if (receiver.TryReceive(a, out ConveyorBelt_Item item))
                {
                    float bonus = 0;
                    bonus += metaillicBonus * item.info.metallic;
                    bonus += plasticBonus * item.info.plastic;
                    bonus += organicBonus * item.info.organic;
                    bonus += paperBonus * item.info.paper;
                    Scripts.Budget.Add((int)bonus);

                    // if recycled properly
                    if (bonus > 0)
                    {
                        item.MarkRecycled((int)bonus);
                    }
                    else // if recycled not properly OR incinerated
                    {
                        item.MarkIncinerated((int)bonus);
                    }
                }
            }
        }
    }
}
