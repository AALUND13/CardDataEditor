using BepInEx;
using CardDataEditor.DataEditting.Config;
using CardDataEditor.Interfaces;
using CardDataEditor.UI.Compoments.Buttons;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CardDataEditor.UI.Compoments.Categories {
    public class ModCardsCategory : MonoBehaviour {
        [Header("Prefabs")]
        public CardButton UICardOptionsPrefab;
        public UICategory UICategoryPrefab;

        [Header("References")]
        public Transform CategoryButtonContents;
        public Transform CardsButtonContents;


        public readonly List<CardButton> CardButtons = new List<CardButton>();
        public readonly List<UICategory> CategoryToCollapses = new List<UICategory>();

        private readonly Dictionary<string, UICategory> Subcategories = new Dictionary<string, UICategory>();
        private readonly Dictionary<CardButton, UICategory> ButtonsToCategory = new Dictionary<CardButton, UICategory>();

        private static readonly bool toggleCardCategoriesExist = CardDataEditor.Plugins.Exists(plugin => plugin.Info.Metadata.GUID == "com.aalund13.rounds.toggle_cards_categories");

        private void Awake() {
            gameObject.SetActive(false);
        }


        public void SearchCards(string searchValue) {
            foreach (UICategory categoryToCollapse in CategoryToCollapses) {
                categoryToCollapse.CloseCategory();
            }
            CategoryToCollapses.Clear();

            foreach (CardButton cardButton in CardButtons) {
                string cardName = cardButton.Category.CardOptions.Card.cardName;
                bool matches = string.IsNullOrEmpty(searchValue) || cardName.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) >= 0;

                cardButton.gameObject.SetActive(matches);
                if (matches && !searchValue.IsNullOrWhiteSpace()) {
                    var categories = GetCardCategories(cardButton);
                    foreach (UICategory category in categories) {
                        if (CategoryToCollapses.Contains(category) || category.IsOpened) {
                            continue;
                        }

                        CategoryToCollapses.Add(category);
                        category.OpenCategory();
                    }
                }
            }
        }


        public CardButton CreateCardButton(CardOptionsConfigCategory category, UnityAction<CardButton> onButtonClicked) {
            string subcategory = toggleCardCategoriesExist ? ToggleCardsCategorieInterface.GetCardSubcategory(category.CardOptions.Card) : "";

            if (!subcategory.IsNullOrWhiteSpace()) {
                var uICategory = GetOrCreateCategory(subcategory);
                var cardButton = uICategory.CreateButton(category, onButtonClicked);

                CardButtons.Add(cardButton);
                ButtonsToCategory.Add(cardButton, uICategory);

                return cardButton;
            } else {
                var cardButtonObject = GameObject.Instantiate(UICardOptionsPrefab).gameObject;
                var cardButton = cardButtonObject.GetComponent<CardButton>();

                cardButtonObject.transform.SetParent(CardsButtonContents);
                cardButtonObject.transform.localScale = Vector3.one;
                cardButton.Init(category, onButtonClicked);
                CardButtons.Add(cardButton);

                return cardButton;
            }
        }

        public UICategory CreateCategory(string categoryName) {
            var uiCategoryObject = GameObject.Instantiate(UICategoryPrefab).gameObject;
            var uiCategory = uiCategoryObject.GetComponent<UICategory>();

            uiCategoryObject.transform.SetParent(CategoryButtonContents);
            uiCategoryObject.transform.localScale = Vector3.one;
            uiCategory.CategoryText.text = categoryName;

            return uiCategory;
        }

        public List<UICategory> GetCardCategories(CardButton cardButton) {
            var categories = new List<UICategory>();
            if (ButtonsToCategory.TryGetValue(cardButton, out UICategory uICategory)) {
                var currentCategory = uICategory;
                while (currentCategory != null) {
                    categories.Add(currentCategory);
                    currentCategory = currentCategory.ParentCategory;
                }
            }
            return categories;
        }


        private UICategory GetOrCreateCategory(string categoryPath) {
            string[] pathSegments = categoryPath.Split('/');

            string fullPath = pathSegments[0];
            UICategory currentCategory = null;

            for (int i = 0; i < pathSegments.Length; i++) {
                string segment = pathSegments[i];
                if (i > 0) fullPath += $"/{segment}";

                if (Subcategories.ContainsKey(fullPath)) {
                    currentCategory = Subcategories[fullPath];
                } else if (!Subcategories.ContainsKey(fullPath)) {
                    if (currentCategory == null) {
                        currentCategory = CreateCategory(segment);
                        Subcategories.Add(fullPath, currentCategory);
                    } else {
                        currentCategory = currentCategory.CreateCategory(segment);
                        Subcategories.Add(fullPath, currentCategory);
                    }
                }
            }

            return currentCategory;
        }
    }
}
