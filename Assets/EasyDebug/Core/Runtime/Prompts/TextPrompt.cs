using TMPro;
using UnityEngine;

public class TextPrompt
{
    public string Key { get; }
    public int Priority { get; private set; }

    private TextMeshPro _textMeshPro;
    private Transform _transform;

    public TextPrompt(string key, string value, int priority, Transform parent)
    {
        Key = key;
        Priority = priority;

        var textObject = new GameObject($"Prompt_{key}");
        _transform = textObject.transform;
        _transform.SetParent(parent);

        _textMeshPro = Create3DText(value, Vector3.zero, _transform);
        _textMeshPro.fontSize = TextPromptManager.TextSize;
        _textMeshPro.alignment = TextAlignmentOptions.Center;

        textObject.AddComponent<TextPromptFaceCamera>();
    }

    /// <summary>
    /// Instantiates a 3D TextMeshPro object with essential setup.
    /// </summary>
    /// <param name="text">The initial text to display.</param>
    /// <param name="position">The position of the 3D text object.</param>
    /// <param name="parent">Optional parent transform for the text object.</param>
    /// <returns>The created TextMeshPro object.</returns>
    public static TextMeshPro Create3DText(string text, Vector3 position, Transform parent = null)
    {
        // Create a new GameObject and set its position and parent
        var textObject = new GameObject("3D_TextMeshPro");
        textObject.transform.position = position;
        if (parent != null) textObject.transform.SetParent(parent, true);

        // Add and configure the TextMeshPro component
        var textMeshPro = textObject.AddComponent<TextMeshPro>();
        textMeshPro.text = text;
        textMeshPro.fontSize = TextPromptManager.TextSize;
        textMeshPro.alignment = TextAlignmentOptions.Center;
        textMeshPro.textWrappingMode = TextWrappingModes.Normal;

        // Assign material (uses the default TMP material)
        textObject.GetComponent<Renderer>().material = textMeshPro.fontMaterial;

        return textMeshPro;
    }

    public void UpdateValue(string value, int priority)
    {
        _textMeshPro.fontSize = TextPromptManager.TextSize;
        _textMeshPro.text = value;
        Priority = priority;
    }

    public void SetLocalPosition(Vector3 localPosition)
    {
        _transform.localPosition = localPosition;
    }
}
