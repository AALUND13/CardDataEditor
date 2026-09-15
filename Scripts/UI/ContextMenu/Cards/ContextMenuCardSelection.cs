using CardDataEditor.DataEditting.Config;
using CardDataEditor.UI.Compoments.Buttons;
using CardDataEditor.UI.Compoments.Categories;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardDataEditor.UI.ContextMenu.Cards {
    public class ContextMenuCardSelection : MonoBehaviour {
        [Header("Prefabs")]
        public ModCardsCategory ModCardsCategoryPrefab;

        [Header("References")]
        public TMP_InputField SearchInputField;
        public Transform ModsContents;


        private readonly Dictionary<string, ModCardsCategory> modCardsCategories = new Dictionary<string, ModCardsCategory>();
        private CardsListContextMenu CardsListContextMenu;
        private ModCardsCategory currentModCategory;
        private CardButton currentCardButton;


        private void Awake() {
            CardsListContextMenu = GetComponentInParent<CardsListContextMenu>();
        }

        private void Update() {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)ModsContents);
        }


        public void OnCardClicked(CardOptionsConfigEntry category, CardButton cardButton) {
            CardsListContextMenu.OpenCard(category);

            if (currentCardButton != null) {
                currentCardButton.IsSelected = false;
                currentCardButton.UpdateVisualAnimation();
            }

            currentCardButton = cardButton;
            currentCardButton.IsSelected = true;
            currentCardButton.UpdateVisualAnimation();
        }

        public void CreateModCategory(string modCategory) {
            if (!modCardsCategories.ContainsKey(modCategory)) {
                var obj = GameObject.Instantiate(ModCardsCategoryPrefab).gameObject;
                var button = obj.GetComponent<ModCardsCategory>();

                obj.transform.SetParent(ModsContents);
                obj.transform.localScale = Vector3.one;

                modCardsCategories.Add(modCategory, button);
            }
        }


        public void OpenModCategory(string modCategory) {
            if (modCardsCategories.TryGetValue(modCategory, out ModCardsCategory category)) {
                if (currentModCategory != null) {
                    currentModCategory.gameObject.SetActive(false);
                }

                currentModCategory = category;
                currentModCategory.gameObject.SetActive(true);
                currentModCategory.SearchCards(SearchInputField.text);

                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)currentModCategory.transform);
            }
        }

        public ModCardsCategory GetModCategory(string modCategory) {
            return modCardsCategories[modCategory];
        }
    }
}
