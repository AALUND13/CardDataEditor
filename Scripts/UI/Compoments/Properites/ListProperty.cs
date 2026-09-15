using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace CardDataEditor.UI.Compoments.Properites {
    public class ListProperty : MonoBehaviour {
        public struct ListItem : IEquatable<ListItem> {
            public string name;
            public string value;

            public ListItem(string name, string value) {
                this.name = name;
                this.value = value;
            }

            public bool Equals(ListItem other) {
                return name == other.name && value == other.value;
            }

            public override bool Equals(object obj) {
                return obj is ListItem other && Equals(other);
            }

            public override int GetHashCode() {
                unchecked {
                    int hash = 17;
                    hash = hash * 31 + (name?.GetHashCode() ?? 0);
                    hash = hash * 31 + (value?.GetHashCode() ?? 0);
                    return hash;
                }
            }

            public static bool operator ==(ListItem left, ListItem right) {
                return left.Equals(right);
            }

            public static bool operator !=(ListItem left, ListItem right) {
                return !left.Equals(right);
            }
        }


        [Header("Prefabs")]
        public ListItemElement listButtonPrefab;

        [Header("References")]
        public TextMeshProUGUI PropertyNameText;
        public TextMeshProUGUI PropertyItemAmountText;
        public RectTransform PropertyValuesContent;

        public Dictionary<ListItem, ListItemElement> PropertyListItems = new Dictionary<ListItem, ListItemElement>();

        public Action<IReadOnlyList<ListItem>> OnValueChanged;
        public Action OnAddButtonClicked;

        private readonly List<ListItem> listItems = new List<ListItem>();
        private readonly List<ListItem> defaultListItems = new List<ListItem>();


        public void Init(string name, ListItem[] listItems, ListItem[] defaultListItems) {
            PropertyNameText.text = name;

            this.defaultListItems.AddRange(defaultListItems);

            foreach (ListItem listItem in listItems) {
                AddItem(listItem, false);
            }
        }


        public void ResetToDefault() {
            while (listItems.Count > 0) {
                RemoveItem(listItems.First(), false);
            }

            foreach (ListItem listItem in defaultListItems) {
                AddItem(listItem, false);
            }

            OnValueChanged?.Invoke(listItems.AsReadOnly());
        }

        public void OpenSelectorList() {
            OnAddButtonClicked?.Invoke();
        }


        public void RemoveItem(ListItem item, bool callCallback = true) {
            if (!listItems.Contains(item)) {
                return;
            }

            Destroy(PropertyListItems[item].gameObject);
            PropertyListItems.Remove(item);
            listItems.Remove(item);

            PropertyItemAmountText.text = $"Item Amount: {listItems.Count}";

            for (int i = 0; i < listItems.Count; i++) {
                PropertyListItems[listItems[i]].SetIndex(i + 1);
            }

            if (callCallback) {
                OnValueChanged?.Invoke(listItems.AsReadOnly());
            }
        }

        public void AddItem(ListItem item, bool callCallback = true) {
            if(listItems.Contains(item)) {
                return;
            }

            GameObject ListItemButtonObject = GameObject.Instantiate(listButtonPrefab.gameObject, PropertyValuesContent);
            ListItemElement button = ListItemButtonObject.GetComponent<ListItemElement>();
            ListItemButtonObject.transform.localScale = Vector3.one;
            button.Init(this, item, listItems.Count + 1);

            PropertyListItems.Add(item, button);
            listItems.Add(item);

            PropertyItemAmountText.text = $"Item Amount: {listItems.Count}";

            if (callCallback) {
                OnValueChanged?.Invoke(listItems.AsReadOnly());
            }
        }
    }
}
