using UnityEngine;
using System.Collections.Generic;

namespace RecycleFactory.Buildings
{
    public class BuildingExtension_Releaser : MonoBehaviour
    {
        [SerializeField] private List<ConveyorOutAnchor> outAnchors;

        public List<ConveyorOutAnchor> OutAnchors => outAnchors;
    }
}