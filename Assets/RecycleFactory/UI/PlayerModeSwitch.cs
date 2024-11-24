using RecycleFactory.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RecycleFactory.UI
{
    [RequireComponent(typeof(UIInputMask))]
    internal class PlayerModeSwitch : MonoBehaviour
    {
        [SerializeField] private Image switchIconImage;

        [SerializeField] private Sprite icon_defaultMode;
        [SerializeField] private Sprite icon_buildingMode;
        [SerializeField] private Sprite icon_demolishingMode;

        private Dictionary<Mode, Sprite> modeToIcon;

        private UIInputMask inputMask;

        private void Start()
        {
            inputMask = GetComponent<UIInputMask>();
            modeToIcon = new Dictionary<Mode, Sprite>()
            {
                { Mode.None, icon_defaultMode },
                { Mode.Build, icon_buildingMode },
                { Mode.Demolish, icon_demolishingMode }
            };
            switchIconImage.sprite = modeToIcon[Mode.Build];
        }

        public void Increment()
        {
            Scripts.PlayerController.IncrementMode();
            switchIconImage.sprite = modeToIcon[Scripts.PlayerController.mode];
        }
    }
}
