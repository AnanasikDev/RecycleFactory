﻿using UnityEngine;

public class ConveyorInAnchor : MonoBehaviour
{
    [SerializeField] private Vector3 localTilePosition;

    public Vector3 LocalTilePosition
    {
        get => localTilePosition;
        set => localTilePosition = value;
    }
}
