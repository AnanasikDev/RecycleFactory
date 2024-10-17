using UnityEngine;

public class ConveyorOutAnchor : MonoBehaviour
{
    [SerializeField] private Vector3 localTilePosition;

    public Vector3 LocalTilePosition
    {
        get => localTilePosition;
        set => localTilePosition = value;
    }
}
