using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace EasyDebug
{
    public class CommandLineSuggestions
    {
        public void UpdateDropdownOptions(TMP_Dropdown dropdown, List<string> options)
        {
            Debug.Log("Dropdown updated");
            // Clear existing options
            dropdown.ClearOptions();

            // Add new options
            dropdown.AddOptions(options);
            dropdown.Show();
        }
    }
}