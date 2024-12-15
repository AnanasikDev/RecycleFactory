using NaughtyAttributes;
using System;
using TMPro;
using UnityEngine;

namespace RecycleFactory
{
    public sealed class Budget : MonoBehaviour
    {
        [SerializeField] private int defaultAmount = 24000;

        [ShowNativeProperty] public int amount { get; private set; }

        [SerializeField] private string textMeshProFormat = "${0}";
        [SerializeField] private TextMeshProUGUI textMeshPro;

        public void Init()
        {
            amount = defaultAmount;
        }

        public void Add(int delta)
        {
            SetAmount(amount + delta);
        }

        public void SetAmount(int newValue)
        {
            amount = newValue;
            textMeshPro.text = string.Format(textMeshProFormat, amount);
        }
    }
}