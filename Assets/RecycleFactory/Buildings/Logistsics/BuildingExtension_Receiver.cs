using UnityEngine;
using System.Collections.Generic;
using RecycleFactory.Buildings.Logistics;

namespace RecycleFactory.Buildings
{
    public class BuildingExtension_Receiver : MonoBehaviour
    {
        [SerializeField] private List<ConveyorBelt_Anchor> inAnchors;

        public List<ConveyorBelt_Anchor> InAnchors => inAnchors;

        public void Init()
        {

        }

        public void Rotate(int delta)
        {
            foreach (var inAnchor in inAnchors)
            {
                inAnchor.Revolve(delta);
            }
        }
    }
}