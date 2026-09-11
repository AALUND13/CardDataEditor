using CardDataEditor.UI.Compoments.Categories;
using CardDataEditor.UI.Compoments.Properites;
using UnityEngine;

namespace CardDataEditor.UI {
    public class UIRegsitry : MonoBehaviour {
        public static UIRegsitry Instance { get; private set; }

        [Header("References")]
        public UIIntProperty UIIntPropertyPrefab;
        public UIFloatProperty UIFloatPropertyPrefab;
        public UIBoolProperty UIBoolPropertyPrefab;
        public UIPropertyDropdown UIEnumPropertyPrefab;
        public UICategory UICategoryPrefab;

        internal static void Init() {
            Instance = CardDataEditor.Assets.LoadAsset<GameObject>("UI Propety Regsitry").GetComponent<UIRegsitry>();
        }
    }
}
