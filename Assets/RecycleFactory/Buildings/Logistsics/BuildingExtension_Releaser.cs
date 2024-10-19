using UnityEngine;
using System.Collections.Generic;

namespace RecycleFactory.Buildings
{
    public class BuildingExtension_Releaser : MonoBehaviour
    {
        [SerializeField] private List<ConveyorAnchor> outAnchors;

        public List<ConveyorAnchor> OutAnchors => outAnchors;

        public void Rotate(int delta)
        {
            foreach (var outAnchor in outAnchors)
            {
                outAnchor.Revolve(delta);
            }
        }

        public bool TryReleaseAt(ConveyorBelt_Item item, int outAnchorIndex)
        {
            if (OutAnchors[outAnchorIndex].conveyor != null)
            {
                if (OutAnchors[outAnchorIndex].conveyor.isEmpty)
                {
                    // release

                    OutAnchors[outAnchorIndex].conveyor.SetItem(item);

                    return true;
                }
            }

            return false;
        }
    }
}