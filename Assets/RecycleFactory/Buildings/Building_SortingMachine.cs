using NaughtyAttributes;
using RecycleFactory.Buildings.Logistics;
using System;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    [RequireComponent(typeof(BuildingExtension_Receiver))]
    [RequireComponent(typeof(BuildingExtension_Releaser))]
    public class Building_SortingMachine : Building
    {
        [SerializeField] private SortingDefinition[] sortingDefinitions;

        internal event Action<ConveyorBelt_Item, SortingDefinition> onReceivedEvent;

        private SortingMachineAnimator animationController;
        private bool isAnimated;

        protected override void PostInit()
        {
            animationController = GetComponent<SortingMachineAnimator>();
            isAnimated = animationController != null;
            if (isAnimated)
            {
                animationController.Init();
                //animationController.onReadyToReleaseEvent += Release;
            }
        }

        private void Update()
        {
            ManageItem();
        }

        public void ManageItem()
        {
            foreach (SortingDefinition def in sortingDefinitions)
            {
                if (receiver.CanReceive(0, out ConveyorBelt_Item item, (ConveyorBelt_Item i) => isItemSortable(i.info, def)))
                {
                    if (!isAnimated)
                    {
                        int laneIndex = releaser.ChooseLane(def.outAnchorIndex, out var nextNode);
                        if (laneIndex == -1) return; // no other items could be released either, no need to check
                        receiver.ForceReceive(item);
                        onReceivedEvent?.Invoke(item, def);
                        item = ConveyorBelt_Item.Create(item.info);
                        releaser.ForceRelease(def.outAnchorIndex, laneIndex, item, nextNode);
                    }
                    else if (animationController.isReadyToReceive)
                    {
                        receiver.ForceReceive(item, disable: false); // receive without disabling item
                        animationController.OnReceive(item, def.outAnchorIndex);
                        // Item received, wait for animationController.onReadyToReleaseEvent
                    }
                }
            }
        }

        internal bool Release(ConveyorBelt_Item item, int anchorIndex)
        {
            int laneIndex = releaser.ChooseLane(anchorIndex, out var nextNode);
            if (laneIndex == -1) return false;

            releaser.ForceRelease(anchorIndex, laneIndex, item, nextNode);
            return true;
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
