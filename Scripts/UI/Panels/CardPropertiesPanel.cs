using CardDataEditor.DataEditting.Config;
using CardDataEditor.DataEditting.Properties;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnboundLib.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace CardDataEditor.UI.Panels {
    public class CardPropertiesPanel : MonoBehaviour {
        private struct CardPropertyRef {
            public CardPropertyConfigEntry CardProperty;
            public GameObject ProperyUIObject;
            public string Category;

            public CardPropertyRef(CardPropertyConfigEntry cardProperty, GameObject properyUIObject, string category) {
                CardProperty = cardProperty;
                ProperyUIObject = properyUIObject;
                Category = category;
            }
        }

        [Header("Prefabs")]
        public GameObject HeaderText;

        [Header("References")]
        public RectTransform PropertiesHolder;


        private readonly Dictionary<string, List<CardPropertyRef>> CategoryToPropertiesMap
            = new Dictionary<string, List<CardPropertyRef>>();

        private readonly Dictionary<string, GameObject> CategoryHeaderMap
            = new Dictionary<string, GameObject>();
        private readonly Dictionary<CardPropertyConfigEntry, GameObject> PropertiesMap 
            = new Dictionary<CardPropertyConfigEntry, GameObject>();

        private CardOptionsConfigEntry cardOptionsConfigEntry;


        private void Update() {
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)PropertiesHolder);
        }


        public void CreateCardProperties(CardOptionsConfigEntry entry) {
            if(cardOptionsConfigEntry != null)
                cardOptionsConfigEntry.OnEntryChanged -= UpdateProperties;
            
            cardOptionsConfigEntry = entry;
            cardOptionsConfigEntry.OnEntryChanged += UpdateProperties;

            foreach (var property in CategoryHeaderMap)
                Destroy(property.Value);
            foreach (var property in PropertiesMap)
                Destroy(property.Value);

            CategoryToPropertiesMap.Clear();
            CategoryHeaderMap.Clear();
            PropertiesMap.Clear();


            foreach (var property in entry.ConfigEntries) {
                if (property.CardOptionProperty.CanShowProperty()) {
                    string propertyCategory = property.CardOptionProperty.GetCategoryName();
                    if (!CategoryToPropertiesMap.ContainsKey(propertyCategory)) {
                        CategoryToPropertiesMap.Add(propertyCategory, new List<CardPropertyRef>());
                    }

                    var propertyUIObj = property.CardOptionProperty.CreateUIProperty(entry.GetEntry(property.CardOptionProperty.GetSerializeName()));
                    CategoryToPropertiesMap[propertyCategory].Add(
                        new CardPropertyRef(
                            property,
                            propertyUIObj,
                            propertyCategory
                        )
                    );
                }
            }

            foreach (var propertiesCategory in CategoryToPropertiesMap) {
                var headerObj = GameObject.Instantiate(HeaderText);
                var headerText = headerObj.GetComponentInChildren<TextMeshProUGUI>();

                headerText.text = propertiesCategory.Key;
                headerObj.transform.SetParent(PropertiesHolder);
                headerObj.transform.localScale = Vector3.one;

                CategoryHeaderMap.Add(propertiesCategory.Key, headerObj);
                foreach (var property in propertiesCategory.Value) {
                    property.ProperyUIObject.transform.SetParent(PropertiesHolder);
                    property.ProperyUIObject.transform.localScale = Vector3.one;
                    PropertiesMap.Add(property.CardProperty, property.ProperyUIObject);
                }
            }


            
        }

        public void UpdateProperties(CardPropertyConfigEntry cardPropertyConfigEntry) {
            bool isNewProperty = !PropertiesMap.ContainsKey(cardPropertyConfigEntry) && cardPropertyConfigEntry.CardOptionProperty.CanShowProperty();

            if (isNewProperty) {
                CreateCardProperties(cardOptionsConfigEntry);
                return;
            } else if(cardPropertyConfigEntry.CardOptionProperty.CanShowProperty()) {
                return;
            }

            GameObject propertyObj = PropertiesMap[cardPropertyConfigEntry];

            PropertiesMap.Remove(cardPropertyConfigEntry);
            Destroy(propertyObj);

            string category = cardPropertyConfigEntry.CardOptionProperty.GetCategoryName();

            if (CategoryToPropertiesMap.TryGetValue(category, out var properties)) {
                properties.RemoveAll(p => p.CardProperty == cardPropertyConfigEntry);

                if (properties.Count == 0) {
                    if (CategoryHeaderMap.TryGetValue(category, out var headerObj)) {
                        Destroy(headerObj);
                        CategoryHeaderMap.Remove(category);
                    }

                    CategoryToPropertiesMap.Remove(category);
                }
            }
        }
    }
}
