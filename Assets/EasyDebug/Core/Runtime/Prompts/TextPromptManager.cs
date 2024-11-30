using System.Collections.Generic;
using UnityEngine;

namespace EasyDebug.Prompts
{
    public static class TextPromptManager
    {
        public static float TextSize = 1.3f;
        public static float PromptDistance = 0.4f;

        private static bool _showAll = true;
        public static bool ShowAll
        {
            get
            {
                return _showAll;
            }
            set
            {
                if (_showAll != value)
                {
                    foreach (var container in PromptContainers.Values)
                    {
                        container.GetAllPrompts().ForEach(p => p.ToggleState(value));
                    }
                }
                _showAll = value;
            }
        }
        public static string ShowOnlyWithName = null;
        public static Vector3 StartLocalOffset = new Vector3(0, 1.5f, 0);

        private static readonly Dictionary<GameObject, PromptContainer> PromptContainers = new();

        /// <summary>
        /// Updates or creates a text prompt above a gameobject.
        /// </summary>
        /// <param name="gameobject">The target GameObject.</param>
        /// <param name="key">The key for the text prompt.</param>
        /// <param name="value">The text to display.</param>
        /// <param name="priority">Priority for stacking the text prompt.</param>
        public static void UpdateText(GameObject gameobject, string key, string value, int priority = 0)
        {
            if (!PromptContainers.TryGetValue(gameobject, out var container))
            {
                container = new PromptContainer(gameobject);
                PromptContainers[gameobject] = container;
            }

            container.UpdatePrompt(key, value, priority);
        }

        public static void DestroyAllPrompts(GameObject gameobject)
        {
            if (PromptContainers.TryGetValue(gameobject, out var container))
            {
                PromptContainers.Remove(gameobject);
                GameObject.Destroy(gameobject);
            }
        }
    }
}
