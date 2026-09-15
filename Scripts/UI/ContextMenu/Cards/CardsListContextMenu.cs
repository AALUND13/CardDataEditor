using CardDataEditor.DataEditting.Config;
using CardDataEditor.UI.Compoments.Buttons;
using CardDataEditor.UI.Compoments.Categories;
using CardDataEditor.UI.Compoments.Previews;
using CardDataEditor.UI.Menus;
using CardDataEditor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace CardDataEditor.UI.ContextMenu.Cards {
    public class CardsListContextMenu : MonoBehaviour {
        public static CardsListContextMenu Instance { get; private set; }

        [Header("References")]
        public CanvasGroup CanvasGroup;
        public RectTransform ContextMenuPanel;
        public Image BackgroundImage;

        [Header("Compoment References")]
        public ContextMenuCardSelection CardSelection;
        public ContextMenuModsSelection ModsSelection;
        public CardPreview CardPreview;

        [Header("Options")]
        public float SelectTimeThreshold = 1.0f;
        public float DarkenAlpha = 0.75f;


        private readonly List<string> createdModCategories = new List<string>();
        private float timeToSelect = 0;

        private Action<CardOptionsConfigEntry> selectCard;
        private CardOptionsConfigEntry currentCardOptions;

        private Canvas canvas;

        private bool isOpened;


        private void Awake() {
            Instance = this;
            canvas = GetComponentInParent<Canvas>();

            float moveContextMenuTo = (canvas.pixelRect.height / 2) + (ContextMenuPanel.sizeDelta.y / 2) + 10;
            BackgroundImage.color = new Color(0f, 0f, 0f, 0f);
            ContextMenuPanel.anchoredPosition = new Vector2(0, moveContextMenuTo * 50);
        }


        public void Init(CardDataConfigFile config) {
            CardDataEditorMenu.Instance.OnMenuClose += HideContextMenu;

            foreach (CardOptionsConfigEntry category in config.Categories) {
                string modCategory = GetCardModCategory(category.CardOptions.Card);
                if (!createdModCategories.Contains(modCategory)) {
                    ModsSelection.CreateModButton(modCategory);
                    CardSelection.CreateModCategory(modCategory);
                    createdModCategories.Add(modCategory);
                }

                ModCardsCategory modCardsCategory = CardSelection.GetModCategory(modCategory);
                CardButton cardButton = modCardsCategory.CreateCardButton(category, (CardButton button) => {
                    CardSelection.OnCardClicked(category, button);
                });
            }
        }


        public void OpenCard(CardOptionsConfigEntry category) {
            if (timeToSelect > Time.time && category == currentCardOptions) {
                selectCard?.Invoke(category);
                HideContextMenu();
            } else {
                CardPreview.CreateCardPreview(category);

                timeToSelect = Time.time + SelectTimeThreshold;
                currentCardOptions = category;
            }
        }

        public void OpenMod(string modCategory) {
            CardSelection.OpenModCategory(modCategory);
        }


        public void SelectCurrentItem() {
            if (currentCardOptions != null) {
                selectCard?.Invoke(currentCardOptions);
                HideContextMenu();
            }
        }


        public void OpenContextMenu(Action<CardOptionsConfigEntry> selectCardAction) {
            if (isOpened) return;

            isOpened = true;
            selectCard = selectCardAction;

            StopAllCoroutines();

            float moveContextMenuFrom = (canvas.pixelRect.height / 2) + (ContextMenuPanel.sizeDelta.y / 2) + 10;
            StartCoroutine(EaseUtils.EaseCoroutine(
                0.25f,
                EaseUtils.EaseType.easeInOutSine,
                (t) => BackgroundImage.color = new Color(0f, 0f, 0f, t),
                null,
                0f, DarkenAlpha
            ));
            StartCoroutine(EaseUtils.EaseCoroutine(
                0.5f,
                EaseUtils.EaseType.easeInOutSine,
                (t) => ContextMenuPanel.anchoredPosition = new Vector2(0, t),
                null,
                moveContextMenuFrom, 0f
            ));

            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.interactable = true;
        }

        public void HideContextMenu() {
            if (!isOpened) return;

            isOpened = false;
            selectCard = null;

            StopAllCoroutines();

            float moveContextMenuTo = (canvas.pixelRect.height / 2) + (ContextMenuPanel.sizeDelta.y / 2) + 10;
            if (gameObject.activeInHierarchy) {
                StartCoroutine(EaseUtils.EaseCoroutine(
                    0.25f,
                    EaseUtils.EaseType.easeInOutSine,
                    (t) => BackgroundImage.color = new Color(0f, 0f, 0f, t),
                    null,
                    DarkenAlpha, 0f
                ));
                StartCoroutine(EaseUtils.EaseCoroutine(
                    0.5f,
                    EaseUtils.EaseType.easeInOutSine,
                    (t) => ContextMenuPanel.anchoredPosition = new Vector2(0, t),
                    () => ContextMenuPanel.anchoredPosition = new Vector2(0, moveContextMenuTo * 50),
                    0f, moveContextMenuTo
                ));
            } else {
                BackgroundImage.color = new Color(0f, 0f, 0f, 0f);
                ContextMenuPanel.anchoredPosition = new Vector2(0, moveContextMenuTo * 50);
            }

            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.interactable = false;
        }

        private string GetCardModCategory(CardInfo cardInfo) {
            return CardManager.cards.First(c => c.Value.cardInfo == cardInfo).Value.category;
        }
    }
}
