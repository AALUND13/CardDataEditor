using CardDataEditor.DataEditting.Config;
using CardDataEditor.UI.Buttons;
using CardDataEditor.UI.Panels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardDataEditor.UI {
    public class UICategory : MonoBehaviour {
        [Header("Prefabs")]
        public CardButton UICardOptionsPrefab;

        [Header("References")]
        public TextMeshProUGUI CategoryText;
        public Button DropdownButton;

        public Sprite dropdownOpenImage;
        public Sprite dropdownCloseImage;

        public GameObject Viewport;

        public GameObject ButtonContents;
        public GameObject CategoryContents;

        // Inspector Hidden Properties
        public UICategory ParentCategory { get; private set; }
        public bool IsOpened { get; private set; }

        internal ModCardsCategory modCardsCategory;


        public CardButton CreateButton(CardOptionsConfigCategory category) {
            var uiCardOptionsObject = GameObject.Instantiate(UICardOptionsPrefab).gameObject;
            var uiCardOptions = uiCardOptionsObject.GetComponent<CardButton>();

            uiCardOptionsObject.transform.SetParent(ButtonContents.transform);
            uiCardOptionsObject.transform.localScale = Vector3.one;
            uiCardOptions.Init(category);

            ButtonContents.SetActive(true);
            return uiCardOptions;
        }

        public UICategory CreateCategory(string categoryName) {
            var uiCategoryObject = GameObject.Instantiate(modCardsCategory.UICategoryPrefab).gameObject;
            var uICategory = uiCategoryObject.GetComponent<UICategory>();

            uiCategoryObject.transform.SetParent(CategoryContents.transform);
            uiCategoryObject.transform.localScale = Vector3.one;
            uICategory.CategoryText.text = categoryName;
            uICategory.modCardsCategory = modCardsCategory;
            uICategory.ParentCategory = this;

            CategoryContents.SetActive(true);
            return uICategory;
        }


        public void ToggleCategory() {
            if (IsOpened) CloseCategory();
            else OpenCategory();
        }

        public void OpenCategory() {
            if (!IsOpened) {
                Viewport.SetActive(true);

                DropdownButton.image.sprite = dropdownOpenImage;
                IsOpened = true;
            }
        }

        public void CloseCategory() {
            if (IsOpened) {
                Viewport.SetActive(false);

                DropdownButton.image.sprite = dropdownCloseImage;
                IsOpened = false;
            }
        }
    }
}
