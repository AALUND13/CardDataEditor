using CardDataEditor.DataEditting.Config;
using CardDataEditor.UI.Buttons;
using System.Collections.Generic;
using TMPro;
using UnboundLib.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace CardDataEditor.UI.Panels {
    public class CardSelectionPanel : MonoBehaviour {
        [Header("Prefabs")]
        public ModCardsCategory ModCardsCategoryPrefab;
        [Header("References")]
        public TMP_InputField SearchInputField;
        public TextMeshProUGUI TitleText;
        public Transform ModsContents;

        private readonly Dictionary<string, ModCardsCategory> modCardsCategories = new Dictionary<string, ModCardsCategory>();
        private ModCardsCategory currentModCategory;
        private CardButton currentCardButton;


        private void Awake() {
            SearchInputField.onValueChanged.AddListener((string value) => {
                currentModCategory.SearchCards(value);
            });
        }

        private void Update() {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)ModsContents);
        }


        public void OpenCard(CardOptionsConfigCategory category, CardButton button) {
            if(currentCardButton != null) {
                currentCardButton.IsSelected = false;
                currentCardButton.UpdateVisualAnimation();
            }
            
            currentCardButton = button;
            currentCardButton.IsSelected = true;
            currentCardButton.UpdateVisualAnimation();

            GetComponentInParent<CardDataEditorUI>().OpenCard(category);
        }


        public void CreateModCategory(string modCategory) {
            if (!modCardsCategories.ContainsKey(modCategory)) {
                GameObject obj = GameObject.Instantiate(ModCardsCategoryPrefab).gameObject;
                ModCardsCategory button = obj.GetComponent<ModCardsCategory>();

                obj.transform.SetParent(ModsContents);
                obj.transform.localScale = Vector3.one;

                modCardsCategories.Add(modCategory, button);
            }
        }


        public void OpenModCategory(string modCategory) {
            if (modCardsCategories.TryGetValue(modCategory, out ModCardsCategory category)) {
                if(currentModCategory != null) {
                    currentModCategory.gameObject.SetActive(false);
                }
                
                currentModCategory = category;
                currentModCategory.gameObject.SetActive(true);
                currentModCategory.SearchCards(SearchInputField.text);
                
                TitleText.text = $"CARDS ({modCategory})";

                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)currentModCategory.transform);
            }
        }

        public ModCardsCategory GetModCategory(string modCategory) {
            return modCardsCategories[modCategory];
        }
    }
}
