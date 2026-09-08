using CardDataEditor.DataEditting.Config;
using CardDataEditor.UI.Properites;
using System;
using System.Linq;
using UnityEngine;

namespace CardDataEditor.DataEditting.Properties {
    public class ThemeCardProperty : CardProperty<CardThemeColor.CardThemeColorType> {
        public ThemeCardProperty(CardInfo card) : base(card) { }


        public override GameObject CreateUIProperty(CardPropertyConfigEntry entry) {
            var propertyUI = GameObject.Instantiate(UIPropetyRegsitry.Instance.UIEnumPropertyPrefab.gameObject);
            var uIPropertyDropdown = propertyUI.GetComponent<UIPropertyDropdown>();

            var rarities = Enum.GetValues(typeof(CardThemeColor.CardThemeColorType)).Cast<CardThemeColor.CardThemeColorType>().ToArray();
            var items = rarities.Select(t => new UIPropertyDropdown.DropdownItem(
                $"<color=#{ColorUtility.ToHtmlStringRGB(CardChoice.instance.GetCardColor(t))}>{t.ToString().ToUpper()}{(t.ToString() == entry.DefaultValue.ToString() ? " (Default)" : "")}",
                t.ToString()
            )).ToArray();

            var selectedItem = items.First(i => i.value == entry.Value.ToString());
            var defaultItem = items.First(i => i.value == entry.DefaultValue.ToString());

            uIPropertyDropdown.Init(GetPropertyName(), items, selectedItem, defaultItem);
            uIPropertyDropdown.OnValueChanged += (UIPropertyDropdown.DropdownItem dropdownItem) => {
                CardThemeColor.CardThemeColorType theme = Enum.GetValues(typeof(CardThemeColor.CardThemeColorType))
                    .Cast<CardThemeColor.CardThemeColorType>()
                    .FirstOrDefault(r => r.ToString() == dropdownItem.value);

                entry.Value = theme;
            };

            return propertyUI;
        }


        public override string GetCategoryName() {
            return "Basic";
        }

        public override string GetPropertyName() {
            return "Theme";
        }


        public override void ApplyPropertyToPreviewCard(GameObject toggleCardObject, CardInfo toggleCardInfo) {
            toggleCardInfo.colorTheme = GetPropertyTyped();
            var cardVisuals = toggleCardObject.GetComponentInChildren<CardVisuals>(false);

            if (cardVisuals != null) {
                cardVisuals.images.ToList().ForEach(t => t.color = CardChoice.instance.GetCardColor(toggleCardInfo.colorTheme));
                cardVisuals.nameText.color = CardChoice.instance.GetCardColor(toggleCardInfo.colorTheme);
            }
        }


        public override void ApplyProperty(CardThemeColor.CardThemeColorType value) {
            Card.colorTheme = value;
        }

        public override CardThemeColor.CardThemeColorType GetPropertyTyped() {
            var result = Card.colorTheme;
            return result;
        }


        public override byte[] SerializeValueTyped(CardThemeColor.CardThemeColorType theme) {
            byte[] result = System.Text.Encoding.UTF8.GetBytes(theme.ToString());
            return result;
        }

        public override CardThemeColor.CardThemeColorType DeserializeValueTyped(byte[] data) {
            string value = System.Text.Encoding.UTF8.GetString(data);

            // Since other mods pqtch "GetValues" to return a custom enum values, and Enum.Parse doesn't work with that, we have to use GetValues and manually find the value.
            CardThemeColor.CardThemeColorType theme = Enum.GetValues(typeof(CardThemeColor.CardThemeColorType))
                    .Cast<CardThemeColor.CardThemeColorType>()
                    .FirstOrDefault(r => r.ToString() == value);

            return theme;
        }
    }
}
