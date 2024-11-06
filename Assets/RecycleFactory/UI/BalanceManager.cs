using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace RecycleFactory.UI
{
    public class BalanceManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshPro;
        [SerializeField] private string balanceFormat = "${0}";

        [SerializeField] private int defaultBalance = 24000;
        [ShowNativeProperty] public int balance { get; private set; }

        public void Init()
        {
            SetBalance(defaultBalance);
        }

        public void SetBalance(int balance)
        {
            textMeshPro.text = string.Format(balanceFormat, balance);
        }
    }
}
