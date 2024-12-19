using NaughtyAttributes;
using UnityEngine;

namespace RecycleFactory.UI
{
    public class UIProgressBar : MonoBehaviour
    {
        [SerializeField] private RectTransform filler;
        private float length;

        [ShowNativeProperty] public float progress { get; private set; }

        private void Awake()
        {
            length = filler.rect.width;
        }

        /// <summary>
        /// Updates progress bar with new value (0<=value<=1)
        /// </summary>
        public void SetValue(float value)
        {
            progress = Mathf.Clamp01(value);
            filler.transform.localPosition = new Vector3(-length, 0, 0) + filler.transform.localPosition.WithX(length * progress);
        }
    }
}
