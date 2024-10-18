using UnityEngine;
using System.Collections.Generic;

namespace RecycleFactory.Buildings
{
    public class BuildingExtension_Receiver : MonoBehaviour
    {
        [SerializeField] private List<ConveyorInAnchor> inAnchors;

        public List<ConveyorInAnchor> InAnchors => inAnchors;
    }
}