using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace RecycleFactory.UI
{
    public class UIProgressBar : MonoBehaviour
    {
        [SerializeField] private RectTransform filler;
        [SerializeField] private TextMeshProUGUI text;
        private string textFormat = "{0}%";

        [ShowNativeProperty] public float progress { get; private set; }

        /// <summary>
        /// Updates progress bar with new value (0<=value<=1)
        /// </summary>
        public void SetValue(float value)
        {
            progress = Mathf.Clamp01(value);
            filler.transform.localScale = filler.transform.localScale.WithX(progress);
            text.text = string.Format(textFormat, (int)(value * 100));
        }
    }
}
