using BepInEx.Logging;
using CardDataEditor.DataEditting.Config;
using CardDataEditor.DataEditting.Properties;
using CardDataEditor.UI;
using CardDataEditor.UI.Compoments.Properites;
using CardDataEditor.UI.ContextMenu.Cards;
using CardDataEditor.Utils.Debug;
using ClassesManagerReborn;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnboundLib.Utils;
using UnityEngine;

namespace CardDataEditor.DataEditting.Properties {
    public class BlacklistCardsProperty : CardProperty<List<CardInfo>> {
        private static List<CardInfo> allCards => CardManager.cards.Values
            .Select(v => v.cardInfo)
            .OrderBy(c => c.cardName)
            .ToList();

        private List<CardInfo> savedBlacklist = new List<CardInfo>();


        public override void OnInit() {
            var classObject = ClassesRegistry.Get(Card);
            if (classObject != null) savedBlacklist = classObject.BlackList;
        }


        public override GameObject CreateUIProperty(CardPropertyConfigEntry entry) {
            var propertyUIObj = GameObject.Instantiate(UIRegsitry.Instance.UListPropertyPrefab.gameObject);
            var propertyUI = propertyUIObj.GetComponent<ListProperty>();

            propertyUI.Init(
                GetPropertyName(),
                GetPropertyTyped().Select(c => new ListProperty.ListItem($"{c.cardName.ToUpper()} ({GetCardModCategory(c)})", c.name)).ToArray(),
                savedBlacklist.Select(c => new ListProperty.ListItem($"{c.cardName.ToUpper()} ({GetCardModCategory(c)})", c.name)).ToArray()
            );

            propertyUI.OnValueChanged += (IReadOnlyList<ListProperty.ListItem> blackList) => {
                List<CardInfo> cardsToBlacklist = blackList.Select(bc => allCards.Find(c => c.name == bc.value)).ToList();
                entry.Value = cardsToBlacklist;
            };

            propertyUI.OnAddButtonClicked += () => {
                CardsListContextMenu.Instance.OpenContextMenu((CardOptionsConfigEntry selectedCard) => {
                    propertyUI.AddItem(new ListProperty.ListItem($"{selectedCard.CardOptions.Card.cardName.ToUpper()} ({GetCardModCategory(selectedCard.CardOptions.Card)})", selectedCard.CardOptions.Card.name));
                });
            };

            return propertyUIObj;
        }


        public override string GetPropertyName() {
            return "Blacklist Cards";
        }

        public override string GetSerializeName() {
            return "Blacklist Cards";
        }

        public override string GetCategoryName() {
            return "Conditions";
        }


        public override void ApplyProperty(List<CardInfo> value) {
            var classObject = ClassesRegistry.Get(Card);
            if (classObject == null) {
                classObject = ClassesRegistry.Register(Card, CardType.NonClassCard);
            }

            classObject.BlackList.ForEach(bc => classObject.DeBhitelist(bc));
            value.ForEach(bc => classObject.Blacklist(bc));
        }

        public override List<CardInfo> GetPropertyTyped() {
            var classObject = ClassesRegistry.Get(Card);
            if (classObject != null) {
                return classObject.BlackList;
            } else {
                return new List<CardInfo>();
            }
        }

        public override byte[] SerializeValueTyped(List<CardInfo> value) {
            string[] cardObjectNames = value.Select(c => c.name).ToArray();

            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8)) {
                writer.Write(cardObjectNames.Length);
                foreach (string str in cardObjectNames)
                    writer.Write(str ?? "");

                return stream.ToArray();
            }
        }

        public override List<CardInfo> DeserializeValueTyped(byte[] data) {
            using (MemoryStream stream = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8)) {
                int count = reader.ReadInt32();
                List<string> cardObjectNames = new List<string>(count);
                List<CardInfo> cardsList = new List<CardInfo>();

                for (int i = 0; i < count; i++)
                    cardObjectNames.Add(reader.ReadString());

                foreach (var cardObjName in cardObjectNames) {
                    CardInfo cardInfo = allCards.Find(c => c.name == cardObjName);
                    if (cardInfo == null) {
                        LoggerUtils.Log(LogLevel.Warning, $"Unknow card name '{cardObjName}', skipping card.");
                        continue;
                    } else {
                        cardsList.Add(cardInfo);
                    }
                }

                return cardsList;
            }
        }

        private string GetCardModCategory(CardInfo cardInfo) {
            return CardManager.cards.First(c => c.Value.cardInfo == cardInfo).Value.category;
        }
    }
}
