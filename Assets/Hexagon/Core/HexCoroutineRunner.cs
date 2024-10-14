using UnityEngine;

/// <summary>
/// Should be attached to any one object on each scene where Hexagon is used. Provides access to the MonoBehaviour functions such as Coroutines. If this functionality is of no need HexMain may be omitted.
/// </summary>
public class HexCoroutineRunner : MonoBehaviour
{
    public static HexCoroutineRunner instance;

    private void Awake() => instance = this;
}