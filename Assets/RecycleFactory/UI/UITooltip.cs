using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Versatile tool for creating tooltips.
/// </summary>
public class UITooltip : MonoBehaviour
{
    [SerializeField] protected GameObject handler;
    [SerializeField] protected Image background;
    [SerializeField] protected TextMeshProUGUI textMeshPro;

    public string text { get; private set; }

    public event Action onEnabled;
    public event Action onDisabled;

    public static UITooltip Create()
    {
        UITooltip tooltip = new GameObject().AddComponent<UITooltip>();
        return tooltip;
    }

    public virtual void Init()
    {
    }

    public void SetText(string text)
    {
        this.text = text;
        textMeshPro.text = text;
    }

    public virtual void UpdatePosition(Vector2 position)
    {
        transform.position = position.ConvertTo3D();
    }
    
    public virtual void Enable(string text)
    {
        this.text = text;
        handler.gameObject.SetActive(true);
        onEnabled?.Invoke();
    }

    public virtual void Disable()
    {
        handler.gameObject.SetActive(false);
        onDisabled?.Invoke();
    }
}