using CardDataEditor.DataEditting.Config;
using CardDataEditor.UI.Properites;
using ClassesManagerReborn;
using UnityEngine;

namespace CardDataEditor.DataEditting.Properties {
    public class AmountCardProperty : CardProperty<int> {
        public AmountCardProperty(CardInfo card) : base(card) { }


        public override GameObject CreateUIProperty(CardPropertyConfigEntry entry) {
            var propertyUI = GameObject.Instantiate(UIPropetyRegsitry.Instance.UIIntPropertyPrefab.gameObject);
            propertyUI.GetComponent<UIIntProperty>().Init((CardPropertyConfigEntry<int>)entry);
            return propertyUI;
        }


        public override string GetCategoryName() {
            return "Basic";
        }

        public override string GetPropertyName() {
            return "Card Limit";
        }

        public override string GetSerializeName() {
            return "Amount";
        }
        

        public override void ApplyProperty(int value) {
            var classObject = ClassesRegistry.Get(Card);
            if (classObject == null) {
                classObject = ClassesRegistry.Register(Card, CardType.NonClassCard, value);
            }

            classObject.cap = value;
            if (value > 1) {
                Card.allowMultiple = true;
            } else if (value > 0) {
                Card.allowMultiple = false;
            } else if (value <= 0) {
                Card.allowMultiple = true;
            }
        }


        public override int GetPropertyTyped() {
            var classObject = ClassesRegistry.Get(Card);
            int result;

            if (classObject == null) {
                result = Card.allowMultiple ? 0 : 1;
            } else {
                result = classObject.cap;
            }
            return result;
        }


        public override byte[] SerializeValueTyped(int value) {
            byte[] result = System.BitConverter.GetBytes(value);
            return result;
        }

        public override int DeserializeValueTyped(byte[] data) {
            return System.BitConverter.ToInt32(data, 0);
        }
    }
}
