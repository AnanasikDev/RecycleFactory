using UnityEngine;
using System.Collections.Generic;

public class BuildingExtension_Releaser : MonoBehaviour
{
    [SerializeField] private List<ConveyorOutAnchor> outAnchors;

    public List<ConveyorOutAnchor> OutAnchors => outAnchors;
}
