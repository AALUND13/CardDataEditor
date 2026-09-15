using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardDataEditor.UI.Compoments.Properites {
    public class ListItemElement : MonoBehaviour {
        [Header("References")]
        public TextMeshProUGUI IndexText;
        public TextMeshProUGUI NameText;
        public Button RemoveButton;
        public Graphic Graphic;

        [Header("Options")]
        public Color OddIndexColor;
        public Color EvenIndexColor;

        public void Init(ListProperty list, ListProperty.ListItem listItem, int index) {
            NameText.text = listItem.name;
            RemoveButton.onClick.AddListener(() => {
                list.RemoveItem(listItem);
            });
            SetIndex(index);
        }

        public void SetIndex(int index) {
            Graphic.color = index % 2 == 1 ? OddIndexColor : EvenIndexColor;
            IndexText.text = $"{index}.";
        }
    }
}
