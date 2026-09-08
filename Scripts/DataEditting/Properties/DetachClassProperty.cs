using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using CardDataEditor.DataEditting.Config;
using CardDataEditor.UI.Properites;
using ClassesManagerReborn;
using ClassesManagerReborn.Util;
using System.Linq;
using UnboundLib;
using UnityEngine;

namespace CardDataEditor.DataEditting.Properties {
    public class DetachClassProperty : CardProperty<bool> {
        private bool haveBeenDetach = false;
        private CardInfo[][] savedRequiredClassesTree = new CardInfo[1][] { new CardInfo[0] };
        private CardType savedCardType = CardType.NonClassCard;

        public DetachClassProperty(CardInfo card) : base(card) {
            var classObject = ClassesRegistry.Get(Card);
            if (classObject != null) {
                savedRequiredClassesTree = classObject.RequiredClassesTree;
                savedCardType = classObject.type;
            }
        }


        public override GameObject CreateUIProperty(CardPropertyConfigEntry entry) {
            var propertyUI = GameObject.Instantiate(UIPropetyRegsitry.Instance.UIBoolPropertyPrefab.gameObject);
            propertyUI.GetComponent<UIBoolProperty>().Init((CardPropertyConfigEntry<bool>)entry);
            return propertyUI;
        }


        public override string GetCategoryName() {
            return "Classes";
        }

        public override string GetPropertyName() {
            return "Detach Class";
        }


        public override void ApplyPropertyToPreviewCard(GameObject cardObject, CardInfo cardInfo) {
            var classObject = ClassesRegistry.Get(Card);
            var classNameMono = cardObject.GetComponent<ClassNameMono>();
            if (classObject == null || classNameMono == null) return;

            var bottomLeftRect = cardObject.GetComponentsInChildren<RectTransform>(true).FirstOrDefault(x => x.name == "EdgePart (1)");
            var modNameObj = bottomLeftRect.transform.Find("ExtraCardText(Clone)");

            if (GetPropertyTyped()) {
                classNameMono.enabled = false;
                if (modNameObj != null) modNameObj.gameObject.SetActive(false);
            } else {
                classNameMono.enabled = true;
                if (modNameObj != null) modNameObj.gameObject.SetActive(true);
            }
        }


        public override bool CanShowProperty() {
            ClassObject classObject = ClassesRegistry.Get(Card);
            if (classObject != null && (CustomCardCategories.instance.CardCategory("ClassDetachable") || CardDataEditorConfig.DangerMode.Value)) {
                return (classObject.type != CardType.NonClassCard && classObject.type != CardType.Entry) || haveBeenDetach;
            }
            return haveBeenDetach;
        }

        public override void ApplyProperty(bool value) {
            var classObject = ClassesRegistry.Get(Card);
            if (classObject == null) return;

            var className = Card.GetComponent<ClassNameMono>();
            if (value) {
                if (className != null) className.enabled = false;
                classObject.RequiredClassesTree = new CardInfo[1][] { new CardInfo[0] };
                classObject.SetPropertyValue("type", CardType.NonClassCard);
                haveBeenDetach = true;
            } else {
                if (className != null) className.enabled = true;
                classObject.RequiredClassesTree = savedRequiredClassesTree;
                classObject.SetPropertyValue("type", savedCardType);
                haveBeenDetach = false;
            }
        }


        public override bool GetPropertyTyped() {
            return haveBeenDetach;
        }


        public override byte[] SerializeValueTyped(bool value) {
            byte[] result = System.BitConverter.GetBytes(value);
            return result;
        }

        public override bool DeserializeValueTyped(byte[] data) {
            return System.BitConverter.ToBoolean(data, 0);
        }
    }
}
