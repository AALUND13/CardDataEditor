using BepInEx.Logging;
using CardDataEditor.Utils.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CardDataEditor.UI.Properites {
    public class UIPropertyDropdown : MonoBehaviour {
        public struct DropdownItem : IEquatable<DropdownItem> {
            public string name;
            public string value;

            public DropdownItem(string name, string value) {
                this.name = name;
                this.value = value;
            }

            public bool Equals(DropdownItem other) {
                return name == other.name && value == other.value;
            }

            public override bool Equals(object obj) {
                return obj is DropdownItem other && Equals(other);
            }

            public override int GetHashCode() {
                unchecked {
                    int hash = 17;
                    hash = hash * 31 + (name?.GetHashCode() ?? 0);
                    hash = hash * 31 + (value?.GetHashCode() ?? 0);
                    return hash;
                }
            }

            public static bool operator ==(DropdownItem left, DropdownItem right) {
                return left.Equals(right);
            }

            public static bool operator !=(DropdownItem left, DropdownItem right) {
                return !left.Equals(right);
            }
        }

        [Header("Prefabs")]
        public UIDropdownItemButton propertyButtonPrefab;

        [Header("References")]
        public TextMeshProUGUI PropertyNameText;
        public RectTransform PropertyValuesContent;
        public RectTransform PropertyDropdownViewport;
        public TMP_InputField PropertySearchInputField;

        public Action<DropdownItem> OnValueChanged;
        public Dictionary<DropdownItem, UIDropdownItemButton> PropertyDropdownItems = new Dictionary<DropdownItem, UIDropdownItemButton>();

        public DropdownItem[] DropdownItems;
        public DropdownItem SelectedDropdownItem;
        public DropdownItem DefaultDropdownItem;


        public void Init(string name, DropdownItem[] dropdownItems, DropdownItem selectedDropdownItem, DropdownItem defaultDropdownItem) {
            if (!dropdownItems.Contains(selectedDropdownItem)) {
                throw new ArgumentException(
                    $"Selected dropdown item '{selectedDropdownItem.value}' does not exist in the dropdown items.",
                    nameof(selectedDropdownItem)
                );
            } else if (!dropdownItems.Contains(defaultDropdownItem)) {
                throw new ArgumentException(
                    $"Default dropdown item '{selectedDropdownItem.value}' does not exist in the dropdown items.",
                    nameof(defaultDropdownItem)
                );
            }

            PropertyNameText.text = name;
            DropdownItems = dropdownItems;
            SelectedDropdownItem = selectedDropdownItem;
            DefaultDropdownItem = defaultDropdownItem;

            foreach (DropdownItem dropdownItem in dropdownItems) {
                GameObject dropdownButtonObject = GameObject.Instantiate(propertyButtonPrefab.gameObject);
                UIDropdownItemButton dropdownButton = dropdownButtonObject.GetComponent<UIDropdownItemButton>();

                dropdownButtonObject.transform.SetParent(PropertyValuesContent);
                dropdownButton.Init(this, dropdownItem);

                PropertyDropdownItems.Add(dropdownItem, dropdownButton);
            }

            PropertySearchInputField.onValueChanged.AddListener(FilterItems);
            PropertySearchInputField.onSubmit.AddListener(SelectItem);
            PropertySearchInputField.onSelect.AddListener(OnSearchSelected);
            PropertySearchInputField.SetTextWithoutNotify(selectedDropdownItem.name);
        }

        private void OnSearchSelected(string value) {
            LoggerUtils.Log(LogLevel.Info, "Dropdown have been selected.");

            PropertyDropdownViewport.gameObject.SetActive(true);

            foreach (DropdownItem item in DropdownItems) {
                PropertyDropdownItems[item].gameObject.SetActive(true);
            }

            PropertySearchInputField.Select();
            PropertySearchInputField.selectionAnchorPosition = 0;
            PropertySearchInputField.selectionFocusPosition = PropertySearchInputField.text.Length;
        }

        private void FilterItems(string search) {
            search = RemoveTags(search.Trim());

            foreach (DropdownItem item in DropdownItems) {
                string value = RemoveTags(item.value);

                bool matches =
                    string.IsNullOrEmpty(search) ||
                    value.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

                PropertyDropdownItems[item].gameObject.SetActive(matches);
            }
        }

        private void SelectItem(string search) {
            DropdownItem[] dropdownItems = GetItemsFromSearch(search);

            if (dropdownItems.Length > 0) {
                ValueChanged(dropdownItems[0]);
            } else {
                ValueChanged(DefaultDropdownItem);
            }
        }



        public DropdownItem[] GetItemsFromSearch(string value) {
            value = RemoveTags(value);

            return DropdownItems
                .Where(i => RemoveTags(i.value)
                    .IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToArray();
        }

        public void ValueChanged(DropdownItem value) {
            EventSystem.current.SetSelectedGameObject(null);

            SelectedDropdownItem = value;
            PropertySearchInputField.SetTextWithoutNotify(value.name);
            PropertyDropdownViewport.gameObject.SetActive(false);
            
            OnValueChanged?.Invoke(value);
        }


        public void ResetToDefault() {
            ValueChanged(DefaultDropdownItem);
        }


        private string RemoveTags(string value) {
            return Regex.Replace(value, "<.*?>", "");
        }
    }
}
