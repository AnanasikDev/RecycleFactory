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
    }
}