using CardDataEditor.DataEditting;
using CardDataEditor.DataEditting.Config;
using CardDataEditor.DataEditting.Properties;
using System.Linq;
using TMPro;
using UnboundLib;
using UnityEngine;
using UnityEngine.UI;

namespace CardDataEditor.UI.Panels {
    public class CardPreviewPanel : MonoBehaviour {
        [Header("References")]
        public Transform CardPreviewHolder;

        private GameObject CurrentPreviewCard;
        private CardOptionsConfigCategory CurrentCardOptions;


        public void CreateCardPreview(CardOptionsConfigCategory cardOptions) {
            if(CurrentPreviewCard != null) {
                CurrentCardOptions.OnEntryChanged -= ApplyPropertyToPreviewCard;
                Destroy(CurrentPreviewCard);
            }

            CurrentCardOptions = cardOptions;
            CurrentPreviewCard = Instantiate(cardOptions.CardOptions.Card.gameObject, CardPreviewHolder);
            CurrentPreviewCard.SetActive(true);

            var cardFrontObj = FindObjectInChildren(CurrentPreviewCard, "Front");
            var backObj = FindObjectInChildren(CurrentPreviewCard, "Back");
            var damagableObj = FindObjectInChildren(CurrentPreviewCard, "Damagable");

            Destroy(backObj);
            Destroy(damagableObj);

            var cardVisuals = CurrentPreviewCard.GetComponentInChildren<CardVisuals>();
            cardVisuals.firstValueToSet = true;

            FindObjectInChildren(CurrentPreviewCard, "BlockFront")?.SetActive(false);
            var canvasGroups = CurrentPreviewCard.GetComponentsInChildren<CanvasGroup>().ToList();
            canvasGroups.ForEach((CanvasGroup canvasGroup) => canvasGroup.alpha = 1);

            var uiParticleObj = FindObjectInChildren(cardFrontObj.gameObject, "UI_ParticleSystem");
            Destroy(uiParticleObj);

            var backgroundObj = FindObjectInChildren(cardFrontObj.gameObject, "Background");
            var backgroundRect = backgroundObj.GetComponent<RectTransform>();
            backgroundRect.localScale = new Vector3(1, 1, 1);
            backgroundRect.anchorMin = new Vector2(0.5f, 0.5f);
            backgroundRect.anchorMax = new Vector2(0.5f, 0.5f);
            backgroundRect.sizeDelta = new Vector2(1500f, 1500f);

            var backgroundImage = backgroundObj.gameObject.GetComponentInChildren<Image>(true);
            if (backgroundImage != null) {
                backgroundImage.preserveAspect = true;
                backgroundImage.color = new Color(0.16f, 0.16f, 0.16f, 1f);
            }

            var backgroundMask = backgroundObj.gameObject.GetComponentInChildren<Mask>(true);
            if (backgroundMask != null) {
                backgroundMask.showMaskGraphic = true;
            }

            var cardPreviewRect = CurrentPreviewCard.GetOrAddComponent<RectTransform>();
            cardPreviewRect.localScale = 16f * Vector3.one;
            cardPreviewRect.anchorMin = Vector2.zero;
            cardPreviewRect.anchorMax = Vector2.one;
            cardPreviewRect.offsetMin = Vector2.zero;
            cardPreviewRect.offsetMax = Vector2.zero;
            cardPreviewRect.pivot = new Vector2(0.5f, 0.5f);
            cardPreviewRect.localPosition = new Vector3(0, -10, 0);

            var nameText = cardFrontObj.transform.GetComponentInChildren<TextMeshProUGUI>();
            nameText.text = cardOptions.CardOptions.Card.cardName.ToUpper();

            foreach (var property in cardOptions.CardOptions.Properties.Values) {
                property.ApplyPropertyToPreviewCard(CurrentPreviewCard, CurrentPreviewCard.GetComponent<CardInfo>());
            }

            CurrentCardOptions.OnEntryChanged += ApplyPropertyToPreviewCard;
        }

        private void ApplyPropertyToPreviewCard(CardPropertyConfigEntry cardPropertyConfigEntry) {
            cardPropertyConfigEntry.CardOptionProperty.ApplyPropertyToPreviewCard(CurrentPreviewCard, CurrentPreviewCard.GetComponent<CardInfo>());
        }

        private static GameObject FindObjectInChildren(GameObject gameObject, string gameObjectName) {
            var children = gameObject.GetComponentsInChildren<Transform>(true);
            return children.FirstOrDefault(i => i.gameObject.name == gameObjectName).gameObject;
        }
    }
}
