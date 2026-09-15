using CardDataEditor.UI.Compoments.Buttons;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CardDataEditor.UI.ContextMenu.Cards {
    public class ContextMenuModsSelection : MonoBehaviour {
        [Header("Prefabs")]
        public SelectModButton SelectModButtonPrefab;
        [Header("References")]
        public Transform ItemContents;


        private readonly Dictionary<string, SelectModButton> modButtons = new Dictionary<string, SelectModButton>();
        private CardsListContextMenu CardsListContextMenu;
        private SelectModButton CurrentSelectedModButton;


        private void Awake() {
            CardsListContextMenu = GetComponentInParent<CardsListContextMenu>();
        }

        public void CreateModButton(string modCategory) {
            if (!modButtons.ContainsKey(modCategory)) {
                var obj = GameObject.Instantiate(SelectModButtonPrefab).gameObject;
                var button = obj.GetComponent<SelectModButton>();

                obj.transform.SetParent(ItemContents);
                obj.transform.localScale = Vector3.one;
                button.Init(modCategory, OpenMod);

                modButtons.Add(modCategory, button);
                SortModButtons();
            }
        }

        public void OpenMod(string modCategory) {
            if (modButtons.TryGetValue(modCategory, out SelectModButton modButton)) {
                CurrentSelectedModButton?.DeselectMod();
                modButton.SelecteMod();

                CurrentSelectedModButton = modButton;
                CardsListContextMenu.OpenMod(modCategory);
            }
        }


        private void SortModButtons() {
            int index = 0;
            if (modButtons.TryGetValue("Vanilla", out SelectModButton vanillaButton)) {
                vanillaButton.transform.SetSiblingIndex(index++);
            }

            var sortedButton = modButtons.Where(x => x.Key != "Vanilla").OrderBy(x => x.Key);
            foreach (var entry in sortedButton) {
                entry.Value.transform.SetSiblingIndex(index++);
            }
        }
    }
}
