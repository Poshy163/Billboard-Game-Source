#region

using TMPro;
using UnityEngine;

#endregion

// ReSharper disable RedundantDefaultMemberInitializer

namespace TextMesh_Pro.Scripts
{
    public class DropdownSample:MonoBehaviour
    {
        [SerializeField] private readonly TMP_Dropdown dropdownWithoutPlaceholder = null;

        [SerializeField] private readonly TMP_Dropdown dropdownWithPlaceholder = null;
        [SerializeField] private readonly TextMeshProUGUI text = null;

        public void OnButtonClick ()
        {
            text.text = dropdownWithPlaceholder.value > -1
                ? "Selected values:\n" + dropdownWithoutPlaceholder.value + " - " + dropdownWithPlaceholder.value
                : "Error: Please make a selection";
        }
    }
}