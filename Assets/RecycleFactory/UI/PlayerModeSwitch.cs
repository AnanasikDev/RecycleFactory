﻿using RecycleFactory.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RecycleFactory.UI
{
    [RequireComponent(typeof(UIInputMask))]
    public class PlayerModeSwitch : MonoBehaviour
    {
        [SerializeField] private Image switchIconImage;

        [SerializeField] private Sprite icon_defaultMode;
        [SerializeField] private Sprite icon_buildingMode;
        [SerializeField] private Sprite icon_demolishingMode;

        [SerializeField] private Texture2D cursor_defaultMode;
        [SerializeField] private Texture2D cursor_buildingMode;
        [SerializeField] private Texture2D cursor_demolishingMode;

        private Dictionary<Mode, Sprite> modeToIcon;
        private Dictionary<Mode, Texture2D> modeToCursor;

        private UIInputMask inputMask;

        public void Init()
        {
            inputMask = GetComponent<UIInputMask>();
            modeToIcon = new Dictionary<Mode, Sprite>()
            {
                { Mode.Edit, icon_defaultMode },
                { Mode.Build, icon_buildingMode },
                { Mode.Demolish, icon_demolishingMode }
            };
            modeToCursor = new Dictionary<Mode, Texture2D>()
            {
                { Mode.Edit, cursor_defaultMode },
                { Mode.Build, cursor_buildingMode },
                { Mode.Demolish, cursor_demolishingMode }
            };
            switchIconImage.sprite = modeToIcon[Mode.Build];
        }

        /// <summary>
        /// Sets mode switch icon according to current player mode
        /// </summary>
        public void UpdateModeIcon()
        {
            switchIconImage.sprite = modeToIcon[Scripts.PlayerController.mode];
            Vector2 hotspot = new Vector2(64, 64);
            if (Scripts.PlayerController.mode == Mode.Edit)
                hotspot = Vector2.zero;
            Cursor.SetCursor(modeToCursor[Scripts.PlayerController.mode], hotspot, CursorMode.Auto);
        }
    }
}
