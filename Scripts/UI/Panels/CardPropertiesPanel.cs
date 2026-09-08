using CardDataEditor.DataEditting.Config;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CardDataEditor.UI.Panels {
    public class CardPropertiesPanel : MonoBehaviour {
        [Header("Prefabs")]
        public GameObject HeaderText;

        [Header("References")]
        public RectTransform PropertiesHolder;


        private readonly Dictionary<string, List<GameObject>> CategoryToPropertiesMap
            = new Dictionary<string, List<GameObject>>();
        private readonly List<GameObject> CurrentlyActiveProperties
            = new List<GameObject>();


        public void CreateCardProperties(CardOptionsConfigCategory category) {
            CategoryToPropertiesMap.Clear();
            foreach (var property in CurrentlyActiveProperties) {
                Destroy(property);
            }

            foreach (var property in category.CardOptions.Properties.Values) {
                if (property.CanShowProperty()) {
                    string propertyCategory = property.GetCategoryName();
                    if (!CategoryToPropertiesMap.ContainsKey(propertyCategory)) {
                        CategoryToPropertiesMap.Add(propertyCategory, new List<GameObject>());
                    }

                    var propertyUIObj = property.CreateUIProperty(category.GetEntry(property.GetPropertyName()));
                    CategoryToPropertiesMap[propertyCategory].Add(propertyUIObj);
                }
            }

            foreach (var propertiesCategory in CategoryToPropertiesMap) {
                var headerObj = GameObject.Instantiate(HeaderText);
                var headerText = headerObj.GetComponentInChildren<TextMeshProUGUI>();

                headerText.text = propertiesCategory.Key;
                headerObj.transform.SetParent(PropertiesHolder);
                headerObj.transform.localScale = Vector3.one;

                CurrentlyActiveProperties.Add(headerObj);
                foreach (var propertyObj in propertiesCategory.Value) {
                    propertyObj.transform.SetParent(PropertiesHolder);
                    propertyObj.transform.localScale = Vector3.one;
                    CurrentlyActiveProperties.Add(propertyObj);
                }

            }
        }
    }
}
