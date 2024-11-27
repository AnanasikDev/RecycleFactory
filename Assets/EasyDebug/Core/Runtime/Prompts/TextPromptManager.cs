using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class TextPromptManager
{
    public static readonly float TextSize = 0.6f;
    public static readonly float PromptDistance = 0.15f;
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
}
