using CardDataEditor.DataEditting.Config;
using CardDataEditor.UI.Panels;
using System.Collections.Generic;
using System.Linq;
using UnboundLib.Utils;
using UnityEngine;

namespace CardDataEditor.UI {
    public class CardDataEditorMenu : MonoBehaviour {
        public static CardDataEditorMenu Instance { get; private set; }

        [Header("References")]
        public ModSelectionPanel ModSelectionPanel;
        public CardSelectionPanel CardsSelectionPanel;
        public CardPreviewPanel CardPreviewPanel;
        public CardPropertiesPanel CardPropertiesPanel;


        private readonly List<string> createdModCategories = new List<string>();


        private void Awake() {
            Instance = this;
        }

        private void Start() {
            gameObject.SetActive(false);
        }


        public void Init(CardOptionsConfig configFile) {
            foreach (CardOptionsConfigCategory category in configFile.Categories) {
                string modCategory = GetCardModCategory(category.CardOptions.Card);
                if (!createdModCategories.Contains(modCategory)) {
                    ModSelectionPanel.CreateModButton(modCategory);
                    CardsSelectionPanel.CreateModCategory(modCategory);
                    createdModCategories.Add(modCategory);
                }

                ModCardsCategory modCardsCategory = CardsSelectionPanel.GetModCategory(modCategory);
                modCardsCategory.CreateCardButton(category);
            }
        }


        public void OpenMenu() {
            gameObject.SetActive(true);
            ModSelectionPanel.OpenMod("Vanilla");
            CardsSelectionPanel.GetModCategory("Vanilla").CardButtons[0].OpenCard();
        }

        public void CloseMenu() {
            gameObject.SetActive(false);
        }


        public void OpenMod(string modCategory) {
            CardsSelectionPanel.OpenModCategory(modCategory);
        }


        public void OpenCard(CardOptionsConfigCategory category) {
            CardPreviewPanel.CreateCardPreview(category);
            CardPropertiesPanel.CreateCardProperties(category);
        }


        private string GetCardModCategory(CardInfo cardInfo) {
            return CardManager.cards.First(c => c.Value.cardInfo == cardInfo).Value.category;
        }
    }
}
