using UnityEngine;
using System.Collections.Generic;

public class BuildingExtension_Receiver : MonoBehaviour
{
    [SerializeField] private List<ConveyorInAnchor> inAnchors;

    public List<ConveyorInAnchor> InAnchors => inAnchors;
}
