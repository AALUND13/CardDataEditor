using BepInEx.Logging;
using CardDataEditor.Utils.Debug;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnboundLib.Utils;

namespace CardDataEditor.DataEditting.Registries {
    public static class CardOptionRegistry {
        private static List<CardInfo> allCards => CardManager.cards.Values
            .Select(v => v.cardInfo)
            .OrderBy(c => c.cardName)
            .ToList();

        private static Dictionary<CardInfo, CardOptions> cardOptionsRegistry = new Dictionary<CardInfo, CardOptions>();
        public static IEnumerable<CardOptions> AllCardOptions => cardOptionsRegistry.Values;

        internal static void RegisterAllCardOptions() {
            Profiler.Start("RegisterAllCardOptions");

            foreach (var card in allCards) {
                RegisterCardOptions(card);
            }

            Profiler.End("RegisterAllCardOptions");
        }

        public static void RegisterCardOptions(CardInfo cardInfo) {
            Profiler.Start("RegisterCardOptions");

            if (!cardOptionsRegistry.ContainsKey(cardInfo)) {
                var cardOptions = new CardOptions(cardInfo);
                PropertyRegistry.AddProperties(cardOptions);
                cardOptionsRegistry[cardInfo] = cardOptions;
            }

            Profiler.End("RegisterCardOptions");
        }


        public static CardOptions GetCardOptions(CardInfo cardInfo) {
            if (cardOptionsRegistry.TryGetValue(cardInfo, out var cardOptions)) {
                return cardOptions;
            }
            return null;
        }


        public static byte[] Serialize() {
            Profiler.Start("CardOptionRegistry.Serialize");
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream)) {
                writer.Write(cardOptionsRegistry.Values.Count);

                foreach (var card in cardOptionsRegistry.Values) {
                    writer.Write(card.Card.name);

                    byte[] data = card.Serialize();

                    writer.Write(data.Length);
                    writer.Write(data);
                }

                var result = stream.ToArray();
                Profiler.End("CardOptionRegistry.Serialize");
                return result;
            }
        }

        public static void Deserialize(byte[] data) {
            Profiler.Start("CardOptionRegistry.Deserialize");
            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream)) {
                int cardCount = reader.ReadInt32();

                for (int i = 0; i < cardCount; i++) {
                    string cardName = reader.ReadString();
                    var cardInfo = ModdingUtils.Utils.Cards.instance.GetCardWithObjectName(cardName);

                    if (cardInfo == null) {
                        LoggerUtils.Log(LogLevel.Warning,
                            $"Could not find CardInfo for card '{cardName}', skipping deserialization for this card."
                        );
                        continue;
                    }

                    var cardOptions = cardOptionsRegistry[cardInfo];
                    int length = reader.ReadInt32();
                    byte[] cardData = reader.ReadBytes(length);

                    if (cardData.Length != length) {
                        LoggerUtils.Log(LogLevel.Warning,
                            $"Expected {length} bytes for card '{cardName}', but only read {cardData.Length} bytes. Skipping deserialization for this card."
                        );
                        continue;
                    }

                    cardOptions.Deserialize(cardData);
                }
            }
            Profiler.End("CardOptionRegistry.Deserialize");
        }
    }
}
