using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using CardDataEditor.DataEditting.Config;
using CardDataEditor.UI.Properites;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnityEngine;
using WillsWackyManagers.Utils;

namespace CardDataEditor.DataEditting.Properties {
    public class DetechCurseProperty : CardProperty<bool> {
        private readonly bool hasCurseCategory = false;
        private bool haveBeenDetach = false;
        private bool isCurseCard = false;

        public DetechCurseProperty(CardInfo card) : base(card) {
            hasCurseCategory = Card.categories.Contains(CustomCardCategories.instance.CardCategory("Curse"));
            isCurseCard = CurseManager.instance.IsCurse(Card);
        }


        public override GameObject CreateUIProperty(CardPropertyConfigEntry entry) {
            var propertyUI = GameObject.Instantiate(UIPropetyRegsitry.Instance.UIBoolPropertyPrefab.gameObject);
            propertyUI.GetComponent<UIBoolProperty>().Init((CardPropertyConfigEntry<bool>)entry);
            return propertyUI;
        }


        public override string GetCategoryName() {
            return "Detachments";
        }

        public override string GetPropertyName() {
            return "Detach From Curse";
        }

        public override string GetSerializeName() {
            return "Detach Curse";
        }


        public override bool CanShowProperty() {
            bool isCurse = CurseManager.instance.IsCurse(Card);
            return isCurseCard && (isCurse || haveBeenDetach);
        }

        public override void ApplyProperty(bool value) {
            if (!isCurseCard) return;

            List<CardInfo> curses = (List<CardInfo>)CurseManager.instance.GetFieldValue("curses");
            CardCategory curseCategory = CustomCardCategories.instance.CardCategory("Curse");

            if (value) {
                if (hasCurseCategory) {
                    Card.categories = Card.categories
                        .Where(category => category != curseCategory)
                        .ToArray();
                }

                curses.Remove(Card);

                haveBeenDetach = true;
            } else {
                if (hasCurseCategory && !Card.categories.Contains(curseCategory)) {
                    Card.categories = Card.categories
                        .Append(curseCategory)
                        .ToArray();
                }

                if (!curses.Contains(Card)) {
                    curses.Add(Card);
                }

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
