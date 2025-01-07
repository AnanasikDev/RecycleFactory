using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RecycleFactory.UI
{
    public class PermanentUpgradesManager : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI title;

        [SerializeField] private int minLevel = 5;
        [SerializeField] private int cost = 5000;

        private bool isAvailable = false;

        private const string buttonFormat = "Extend area\n {0}+ level, ${1}";

        public void Init()
        {
            Toggle(true);
            title.text = string.Format(buttonFormat, minLevel, cost);
        }

        private void Update()
        {
            isAvailable = IsAvailable();
            Toggle(isAvailable);
        }

        private bool IsAvailable()
        {
            return Scripts.LevelController.levelInProgress >= minLevel &&
                   Scripts.Budget.amount >= cost;
        }

        private void Toggle(bool value)
        {
            if (value) // available
            {
                button.interactable = true;
            }
            else // unavailable
            {
                button.interactable = false;
            }
        }

        public void ExtendField()
        {
            if (!isAvailable) return;

            Scripts.Budget.Add(-cost);
            Scripts.Map.Extend(1, 1, 1, 1);
        }
    }
}
