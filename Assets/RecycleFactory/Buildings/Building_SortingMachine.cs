﻿using UnityEngine;
using RecycleFactory.Buildings.Logistics;
using NaughtyAttributes;
using System;

namespace RecycleFactory.Buildings
{
    [RequireComponent(typeof(BuildingExtension_Receiver))]
    [RequireComponent(typeof(BuildingExtension_Releaser))]
    public class Building_SortingMachine : Building
    {
        [SerializeField] private SortingDefinition[] sortingDefinitions;

        private void Update()
        {
            ManageItem();
        }
        public void ManageItem()
        {
            foreach (var def in sortingDefinitions)
            {
                if (receiver.CanReceive(0, out ConveyorBelt_Item item, (ConveyorBelt_Item i) => isItemSortable(i.info, def)))
                {
                    int laneIndex = releaser.ChooseLane(def.outAnchorIndex, out var nextNode);
                    if (laneIndex == -1) return; //?

                    receiver.ForceReceive(item);
                    item = ConveyorBelt_Item.Create(item.info);
                    releaser.ForceRelease(def.outAnchorIndex, laneIndex, item, nextNode);
                }
            }
        }

        internal bool isItemSortable(ConveyorBelt_ItemInfo item, SortingDefinition def)
        {
            return def.range_magnetic.Contains(item.magnetic) &&
                   def.range_density.Contains(item.density) &&
                   def.range_transparency.Contains(item.transparency) &&
                   def.range_metallic.Contains(item.metallic) &&
                   def.range_plastic.Contains(item.plastic) &&
                   def.range_organic.Contains(item.organic) &&
                   def.range_paper.Contains(item.paper);
        }
    }

    [Serializable]
    public struct SortingDefinition
    {
        [MinMaxSlider(0f, 1f)] public Vector2 range_magnetic;
        [MinMaxSlider(0f, 1f)] public Vector2 range_density;
        [MinMaxSlider(0f, 1f)] public Vector2 range_transparency;
        [MinMaxSlider(0f, 1f)] public Vector2 range_metallic;
        [MinMaxSlider(0f, 1f)] public Vector2 range_plastic;
        [MinMaxSlider(0f, 1f)] public Vector2 range_organic;
        [MinMaxSlider(0f, 1f)] public Vector2 range_paper;

        public int outAnchorIndex;
    }
}
