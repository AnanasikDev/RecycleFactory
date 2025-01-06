using UnityEngine;

namespace RecycleFactory.UI
{
    public class PermanentUpgradesManager : MonoBehaviour
    {

        public void ExtendField()
        {
            Scripts.Map.Extend(1, 1, 1, 1);
        }

    }
}
