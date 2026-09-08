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

            GameObject cardFrontObject = FindObjectInChildren(CurrentPreviewCard, "Front");
            GameObject back = FindObjectInChildren(CurrentPreviewCard, "Back");
            GameObject damagable = FindObjectInChildren(CurrentPreviewCard, "Damagable");

            Destroy(back);
            Destroy(damagable);

            CardVisuals componentsInChild = CurrentPreviewCard.GetComponentInChildren<CardVisuals>();
            componentsInChild.firstValueToSet = true;

            FindObjectInChildren(CurrentPreviewCard, "BlockFront")?.SetActive(false);
            var canvasGroups = CurrentPreviewCard.GetComponentsInChildren<CanvasGroup>().ToList();
            canvasGroups.ForEach((CanvasGroup canvasGroup) => canvasGroup.alpha = 1);

            GameObject uiParticleObject = FindObjectInChildren(cardFrontObject.gameObject, "UI_ParticleSystem");
            Destroy(uiParticleObject);

            var backgroundObj = FindObjectInChildren(cardFrontObject.gameObject, "Background");
            var rectTransform = backgroundObj.GetComponent<RectTransform>();
            backgroundObj.transform.localScale = new Vector3(1, 1, 1);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(1500f, 1500f);

            var imageComponent = backgroundObj.gameObject.GetComponentInChildren<Image>(true);
            if (imageComponent != null) {
                imageComponent.preserveAspect = true;
                imageComponent.color = new Color(0.16f, 0.16f, 0.16f, 1f);
            }

            var maskComponent = backgroundObj.gameObject.GetComponentInChildren<Mask>(true);
            if (maskComponent != null) {
                maskComponent.showMaskGraphic = true;
            }

            RectTransform rect = CurrentPreviewCard.GetOrAddComponent<RectTransform>();
            rect.localScale = 16f * Vector3.one;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.localPosition = new Vector3(0, -10, 0);

            var textName = cardFrontObject.transform.GetComponentInChildren<TextMeshProUGUI>();
            textName.text = cardOptions.CardOptions.Card.cardName.ToUpper();

            foreach (CardProperty property in cardOptions.CardOptions.Properties.Values) {
                property.ApplyPropertyToPreviewCard(CurrentPreviewCard, CurrentPreviewCard.GetComponent<CardInfo>());
            }

            CurrentCardOptions.OnEntryChanged += ApplyPropertyToPreviewCard;
        }

        private void ApplyPropertyToPreviewCard(CardPropertyConfigEntry cardPropertyConfigEntry) {
            cardPropertyConfigEntry.CardOptionProperty.ApplyPropertyToPreviewCard(CurrentPreviewCard, CurrentPreviewCard.GetComponent<CardInfo>());
        }

        private static GameObject FindObjectInChildren(GameObject gameObject, string gameObjectName) {
            Transform[] children = gameObject.GetComponentsInChildren<Transform>(true);
            return (from item in children where item.name == gameObjectName select item.gameObject).FirstOrDefault();
        }
    }
}
