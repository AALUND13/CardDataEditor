using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardDataEditor.UI.Properites {
    public class UIDropdownItemButton : MonoBehaviour {
        [Header("References")]
        public TextMeshProUGUI PropertyNameText;
        public Button EnumValueButton;


        public void Init(UIPropertyDropdown dropdown, UIPropertyDropdown.DropdownItem dropdownItem) {
            PropertyNameText.text = dropdownItem.name;
            EnumValueButton.onClick.AddListener(() => {
                dropdown.ValueChanged(dropdownItem);
            });
        }
    }
}
