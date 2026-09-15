using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardDataEditor.UI.Compoments.Properites {
    public class DropdownItemButton : MonoBehaviour {
        [Header("References")]
        public TextMeshProUGUI PropertyNameText;
        public Button EnumValueButton;


        public void Init(DropdownProperty dropdown, DropdownProperty.DropdownItem dropdownItem) {
            PropertyNameText.text = dropdownItem.name;
            EnumValueButton.onClick.AddListener(() => {
                dropdown.ValueChanged(dropdownItem);
            });
        }
    }
}
