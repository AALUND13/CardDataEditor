using CardDataEditor.DataEditting.Config;
using CardDataEditor.UI.Panels;
using CardDataEditor.Utils;
using RarityLib.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardDataEditor.UI.Buttons {
    public class CardButton : MonoBehaviour {
        [Header("References")]
        public TextMeshProUGUI CardNameText;
        public Graphic RarityGraphic;
        public Graphic ThemeGraphic;
        public Button OpenButton;

        [Header("Options")]
        public float SelectMultiplier = 0.85f;
        public float DeselectMultiplier = 0.70f;

        [HideInInspector] public bool IsSelected = false;
        [HideInInspector] public CardOptionsConfigCategory Category;

        private Coroutine colorAnimation;

        public void Init(CardOptionsConfigCategory category) {
            this.Category = category;
            this.Category.OnEntryChanged += (_) => {
                UpdateVisual();
            };
            UpdateVisual();
        }


        public void OpenCard() {
            GetComponentInParent<CardSelectionPanel>().OpenCard(Category, this);
        }

        public void UpdateVisualAnimation() {
            if (colorAnimation != null) StopCoroutine(colorAnimation);

            Color startColor = RarityGraphic.color;
            if(gameObject.activeInHierarchy) {
                colorAnimation = StartCoroutine(EaseUtils.EaseCoroutine(
                    0.25f,
                    EaseUtils.EaseType.easeInOutSine,
                    t => RarityGraphic.color = Color.Lerp(startColor, GetCardRarityColor(Category.CardOptions.Card), t)
                ));
            } else {
                UpdateVisual();
            }
        }

        public void UpdateVisual() {
            CardNameText.text = Category.CardOptions.Card.cardName.ToUpper();
            RarityGraphic.color = GetCardRarityColor(Category.CardOptions.Card);
            ThemeGraphic.color = CardChoice.instance.GetCardColor(Category.CardOptions.Card.colorTheme);
            if (colorAnimation != null) StopCoroutine(colorAnimation);
        }

        private Color GetCardRarityColor(CardInfo card) {
            float multiplier = IsSelected ? SelectMultiplier : DeselectMultiplier;
            
            var color = RarityUtils.GetRarityData(card.rarity).color * multiplier;
            float max = Mathf.Max(color.r, color.g, color.b);

            return (max < 0.20 ? (Color.white * 0.70f) : color) * multiplier;
        }
    }
}
