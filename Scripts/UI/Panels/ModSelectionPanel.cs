using CardDataEditor.DataEditting;
using CardDataEditor.UI.Buttons;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CardDataEditor.UI.Panels {
    public class ModSelectionPanel : MonoBehaviour {
        [Header("Prefabs")]
        public SelectModButton SelectModButtonPrefab;
        [Header("References")]
        public Transform ItemContents;

        private readonly Dictionary<string, SelectModButton> modButtons = new Dictionary<string, SelectModButton>();
        private SelectModButton CurrentSelectedModButton;
        private CardDataEditorUI CardDataEditor;

        private void Awake() {
            CardDataEditor = GetComponentInParent<CardDataEditorUI>();
        }


        public void CreateModButton(string modCategory) {
            if (!modButtons.ContainsKey(modCategory)) {
                GameObject obj = GameObject.Instantiate(SelectModButtonPrefab).gameObject;
                SelectModButton button = obj.GetComponent<SelectModButton>();

                obj.transform.SetParent(ItemContents);
                obj.transform.localScale = Vector3.one;
                button.Init(modCategory);

                modButtons.Add(modCategory, button);
                SortModButtons();
            }
        }

        public void OpenMod(string modCategory) {
            if (modButtons.TryGetValue(modCategory, out SelectModButton modButton)) {
                if(CurrentSelectedModButton != null) {
                    CurrentSelectedModButton.DeselectMod();
                }
                modButton.SelecteMod();

                CurrentSelectedModButton = modButton;
                CardDataEditor.OpenMod(modCategory);
            };
        }


        private void SortModButtons() {
            int index = 0;

            if (modButtons.TryGetValue("Vanilla", out SelectModButton vanillaButton)) {
                vanillaButton.transform.SetSiblingIndex(index++);
            }

            foreach (var entry in modButtons
                .Where(x => x.Key != "Vanilla")
                .OrderBy(x => x.Key)) {
                entry.Value.transform.SetSiblingIndex(index++);
            }
        }
    }
}
