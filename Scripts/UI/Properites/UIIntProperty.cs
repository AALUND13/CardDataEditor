using CardDataEditor.DataEditting.Config;
using TMPro;
using UnityEngine;

namespace CardDataEditor.UI.Properites {
    public class UIIntProperty : MonoBehaviour {
        [Header("References")]
        public TextMeshProUGUI PropertyNameText;
        public TMP_InputField PropertyInputField;

        
        private CardPropertyConfigEntry<int> cardProperty;


        public void Init(CardPropertyConfigEntry<int> propertyConfigEntry) {
            PropertyNameText.text = propertyConfigEntry.Name;
            PropertyInputField.text = propertyConfigEntry.ValueTyped.ToString();

            PropertyInputField.onEndEdit.AddListener(text => {
                if (int.TryParse(text, out int value)) {
                    propertyConfigEntry.Value = value;
                }
            });
        }

        public void ResetToDefault() {
            PropertyInputField.text = cardProperty.DefaultValueTyped.ToString();
        }
    }
}
