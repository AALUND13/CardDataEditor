using CardDataEditor.DataEditting.Config;
using TMPro;
using UnityEngine;

namespace CardDataEditor.UI.Properites {
    public class UIFloatProperty : MonoBehaviour {
        [Header("References")]
        public TextMeshProUGUI PropertyNameText;
        public TMP_InputField PropertyInputField;


        private CardPropertyConfigEntry<float> cardProperty;


        public void Init(CardPropertyConfigEntry<float> propertyConfigEntry) {
            cardProperty = propertyConfigEntry;

            PropertyNameText.text = propertyConfigEntry.Name;
            PropertyInputField.text = propertyConfigEntry.ValueTyped.ToString();

            PropertyInputField.onValueChanged.AddListener(text => {
                if (float.TryParse(text, out float value)) {
                    propertyConfigEntry.Value = value;
                }
            });
        }

        public void ResetToDefault() {
            PropertyInputField.text = cardProperty.DefaultValueTyped.ToString();
        }
    }
}
