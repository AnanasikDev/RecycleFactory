using UnityEngine;
using System.Collections.Generic;
using RecycleFactory.Buildings.Logistics;

namespace RecycleFactory.Buildings
{
    public class BuildingExtension_Receiver : MonoBehaviour
    {
        private Building building;
        public float height;
        [Tooltip("Must be set in inspector")] public List<ConveyorBelt_Anchor> inAnchors;

        public void Init(Building building, int rotation)
        {
            this.building = building;
            foreach (var inAnchor in inAnchors)
            {
                inAnchor.conveyor = null;
                inAnchor.machine = building;
                inAnchor.height = height;
                inAnchor.Revolve(rotation);
            }
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