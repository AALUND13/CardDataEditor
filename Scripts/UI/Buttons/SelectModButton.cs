using CardDataEditor.UI.Panels;
using CardDataEditor.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardDataEditor.UI.Buttons {
    public class SelectModButton : MonoBehaviour {
        [Header("References")]
        public Graphic SelectionGraphic;
        public Button Button;
        public TextMeshProUGUI ModText;

        [Header("Selection Options")]
        public Color SelectionColor = Color.white * 0.75f;
        public Color DeselectionColor = Color.white * 0.45f;


        private Coroutine colorAnimation;


        private void Awake() {
            SelectionGraphic.color = DeselectionColor;
        }


        public void Init(string modCategory) {
            ModText.text = modCategory;
            Button.onClick.AddListener(() => {
                GetComponentInParent<ModSelectionPanel>().OpenMod(modCategory);
            });
        }


        public void SelecteMod() {
            if (colorAnimation != null) StopCoroutine(colorAnimation);

            Color startColor = SelectionGraphic.color;
            colorAnimation = StartCoroutine(EaseUtils.EaseCoroutine(
                0.25f,
                EaseUtils.EaseType.easeInOutSine,
                t => SelectionGraphic.color = Color.Lerp(startColor, SelectionColor, t)
            ));
        }

        public void DeselectMod() {
            Color startColor = SelectionGraphic.color;
            colorAnimation = StartCoroutine(EaseUtils.EaseCoroutine(
                0.25f,
                EaseUtils.EaseType.easeInOutSine,
                t => SelectionGraphic.color = Color.Lerp(startColor, DeselectionColor, t)
            ));
        }
    }
}
