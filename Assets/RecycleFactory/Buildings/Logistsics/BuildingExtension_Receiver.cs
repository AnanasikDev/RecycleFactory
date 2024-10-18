using UnityEngine;
using System.Collections.Generic;

namespace RecycleFactory.Buildings
{
    public class BuildingExtension_Receiver : MonoBehaviour
    {
        [SerializeField] private List<ConveyorAnchor> inAnchors;

        public List<ConveyorAnchor> InAnchors => inAnchors;

        public void Rotate(int delta)
        {
            foreach (var inAnchor in inAnchors)
            {
                inAnchor.Revolve(delta);
            }
        }
    }
}