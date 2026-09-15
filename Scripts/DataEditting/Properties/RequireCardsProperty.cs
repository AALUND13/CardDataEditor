using BepInEx.Logging;
using CardDataEditor.Conditions;
using CardDataEditor.DataEditting.Config;
using CardDataEditor.UI;
using CardDataEditor.UI.Compoments.Properites;
using CardDataEditor.UI.ContextMenu.Cards;
using CardDataEditor.Utils.Debug;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnboundLib;
using UnboundLib.Utils;
using UnityEngine;

namespace CardDataEditor.DataEditting.Properties {
    public class RequireCardsProperty : CardProperty<List<CardInfo>> {
        private static List<CardInfo> allCards => CardManager.cards.Values
                    .Select(v => v.cardInfo)
                    .OrderBy(c => c.cardName)
                    .ToList();


        public override GameObject CreateUIProperty(CardPropertyConfigEntry entry) {
            var propertyUIObj = GameObject.Instantiate(UIRegsitry.Instance.UListPropertyPrefab.gameObject);
            var propertyUI = propertyUIObj.GetComponent<ListProperty>();

            propertyUI.Init(
                GetPropertyName(),
                GetPropertyTyped().Select(c => new ListProperty.ListItem($"{c.cardName.ToUpper()} ({GetCardModCategory(c)})", c.name)).ToArray(),
                new ListProperty.ListItem[0]
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
            return "Require Cards";
        }

        public override string GetSerializeName() {
            return "Require Cards";
        }

        public override string GetCategoryName() {
            return "Conditions";
        }


        public override void ApplyProperty(List<CardInfo> value) {
            if(!RequireCardsCondition.RequireCards.ContainsKey(Card)) {
                RequireCardsCondition.RequireCards.Add(Card, value);
            } else {
                RequireCardsCondition.RequireCards[Card] = value;
            }
        }

        public override List<CardInfo> GetPropertyTyped() {
            return RequireCardsCondition.RequireCards.GetValueOrDefault(Card, new List<CardInfo>());
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
