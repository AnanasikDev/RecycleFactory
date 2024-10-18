using NaughtyAttributes;
using UnityEngine;

namespace RecycleFactory.Buildings
{
    public abstract class Building : MonoBehaviour
    {
        [HideInInspector] public BuildingExtension_Receiver receiver;
        [HideInInspector] public BuildingExtension_Releaser releaser;

        [ShowNativeProperty] public int rotation { get; private set; }

        public void Init()
        {
            receiver = GetComponent<BuildingExtension_Receiver>();
            releaser = GetComponent<BuildingExtension_Releaser>();
        }

        public virtual void Rotate(int delta)
        {
            rotation = (int)Mathf.Repeat(rotation + delta, 4);  
            if (receiver != null) receiver.Rotate(delta);
            if (releaser != null) releaser.Rotate(delta);
        }
    }
}