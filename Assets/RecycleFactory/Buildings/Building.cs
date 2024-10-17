using UnityEngine;

public class Building : MonoBehaviour
{
    [HideInInspector] public BuildingExtension_Receiver receiver;
    [HideInInspector] public BuildingExtension_Releaser releaser;

    private void Awake()
    {
        receiver = GetComponent<BuildingExtension_Receiver>();
        releaser = GetComponent<BuildingExtension_Releaser>();
    }
}
