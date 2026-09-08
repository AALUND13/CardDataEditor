using CardDataEditor.DataEditting.Config;
using CardDataEditor.UI.Properites;
using CardDataEditor.UI.Registries;
using RarityLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CardDataEditor.DataEditting.Properties {
    public class RarityCardProperty : CardProperty<CardInfo.Rarity> {
        public RarityCardProperty(CardInfo card) : base(card) { }


        public override GameObject CreateUIProperty(CardPropertyConfigEntry entry) {
            GameObject propertyUI = GameObject.Instantiate(UIPropetyRegsitry.Instance.UIEnumPropertyPrefab.gameObject);
            UIPropertyDropdown uIPropertyDropdown = propertyUI.GetComponent<UIPropertyDropdown>();
            
            CardInfo.Rarity[] rarities = Enum.GetValues(typeof(CardInfo.Rarity)).Cast<CardInfo.Rarity>().ToArray();
            UIPropertyDropdown.DropdownItem[] items = rarities.Select(r => new UIPropertyDropdown.DropdownItem(
                $"<color=#{ColorUtility.ToHtmlStringRGB(GetCardRarityColor(r))}>{r.ToString().ToUpper()}{(r.ToString() == entry.DefaultValue.ToString() ? " (Default)" : "")}",
                r.ToString()
            )).ToArray();

            UIPropertyDropdown.DropdownItem selectedItem = items.First(i => i.value == entry.Value.ToString());
            UIPropertyDropdown.DropdownItem defaultItem = items.First(i => i.value == entry.DefaultValue.ToString());

            uIPropertyDropdown.Init(GetPropertyName(), items, selectedItem, defaultItem);
            uIPropertyDropdown.OnValueChanged += (UIPropertyDropdown.DropdownItem dropdownItem) => {
                CardInfo.Rarity theme = Enum.GetValues(typeof(CardInfo.Rarity))
                    .Cast<CardInfo.Rarity>()
                    .FirstOrDefault(r => r.ToString() == dropdownItem.value);

                entry.Value = theme;
            };

            return propertyUI;
        }


        public override string GetCategoryName() {
            return "Basic";
        }

        public override string GetPropertyName() {
            return "Rarity";
        }


        public override void ApplyPropertyToPreviewCard(GameObject toggleCardObject, CardInfo toggleCardInfo) {
            toggleCardInfo.rarity = GetPropertyTyped();

            List<CardRarityColor> rarityColors = toggleCardObject.GetComponentsInChildren<CardRarityColor>(false).ToList();
            rarityColors.ForEach(r => r.Toggle(true));
        }


        public override void ApplyProperty(CardInfo.Rarity value) {
            Card.rarity = value;
        }

        public override CardInfo.Rarity GetPropertyTyped() {
            var result = Card.rarity;
            return result;
        }


        public override byte[] SerializeValueTyped(CardInfo.Rarity rarity) {
            byte[] result = System.Text.Encoding.UTF8.GetBytes(rarity.ToString());
            return result;
        }

        public override CardInfo.Rarity DeserializeValueTyped(byte[] data) {
            string value = System.Text.Encoding.UTF8.GetString(data);

            // Since other mods pqtch "GetValues" to return a custom enum values, and Enum.Parse doesn't work with that, we have to use GetValues and manually find the value.
            CardInfo.Rarity raity = Enum.GetValues(typeof(CardInfo.Rarity))
                    .Cast<CardInfo.Rarity>()
                    .FirstOrDefault(r => r.ToString() == value);

            return raity;
        }

        private Color GetCardRarityColor(CardInfo.Rarity rarity) {
            Color color = RarityUtils.GetRarityData(rarity).color * 0.85f;

            float max = Mathf.Max(color.r, color.g, color.b);
            return (max < 0.20 ? (Color.white * 0.70f) : color) * 0.85f;
        }
    }
}
