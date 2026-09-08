using CardDataEditor.UI.Properites;
using UnityEngine;

namespace CardDataEditor.UI.Registries {
    public class UIPropetyRegsitry : MonoBehaviour {
        public static UIPropetyRegsitry Instance;

        public UIIntProperty UIIntPropertyPrefab;
        public UIFloatProperty UIFloatPropertyPrefab;
        public UIBoolProperty UIBoolPropertyPrefab;
        public UIPropertyDropdown UIEnumPropertyPrefab;

        internal static void Init() {
            Instance = CardDataEditor.Assets.LoadAsset<GameObject>("UI Propety Regsitry").GetComponent<UIPropetyRegsitry>();
        }
    }
}
