using CardDataEditor.DataEditting.Config;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardDataEditor.UI.Properites {
    public class UIBoolProperty : MonoBehaviour {
        [Header("References")]
        public TextMeshProUGUI PropertyNameText;
        public Toggle PropertyToggle;


        private CardPropertyConfigEntry<bool> cardProperty;


        public void Init(CardPropertyConfigEntry<bool> propertyConfigEntry) {
            cardProperty = propertyConfigEntry;

            PropertyNameText.text = propertyConfigEntry.CardOptionProperty.GetPropertyName();
            PropertyToggle.isOn = propertyConfigEntry.ValueTyped;

            PropertyToggle.onValueChanged.AddListener(value => {
                propertyConfigEntry.Value = value;
            });
        }

        public void ResetToDefault() {
            PropertyToggle.isOn = cardProperty.DefaultValueTyped;
        }
    }
}
