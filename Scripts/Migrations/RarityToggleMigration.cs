using BepInEx;
using BepInEx.Configuration;
using CardDataEditor.DataEditting.Config;
using CardDataEditor.Utils.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnboundLib;
using UnboundLib.Utils;

namespace CardDataEditor.Scripts.Migration {
    [BepInDependency("AALUND13.Card.Data.Editor", BepInDependency.DependencyFlags.HardDependency)]

    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class RarityToggleMigration : BaseUnityPlugin {
        private const string ModId = "Rarity.Toggle";
        private const string ModName = "'Rarity Toggle' -> 'Card Data Editor' Migration";
        public const string Version = "9999.9999.9999";


        private void Start() {
            CardOptionsConfigManager.OnConfigFirstCreated += (CardOptionsConfig config) => MigrateRarityToggle(config);
        }


        private readonly Regex sanitizeRegex = new Regex(@"[\n\t\\""'\[\]]+", RegexOptions.Compiled);
        private List<CardInfo> allCards => CardManager.cards.Values
            .Select(v => v.cardInfo)
            .OrderBy(c => c.cardName)
            .ToList();


        public void MigrateRarityToggle(CardOptionsConfig configFile) {
            Profiler.Start("RarityToggleMigration.MigrateRarityToggle");

            var sanitizedCardNames = new Dictionary<string, CardInfo>();
            foreach (var card in allCards) {
                sanitizedCardNames.Add(SanitizeText(card.name), card);
            }

            configFile.SaveOnChange = false;
            foreach (var entry in (Dictionary<ConfigDefinition, string>)Config.GetPropertyValue("OrphanedEntries")) {
                if (sanitizedCardNames.TryGetValue(entry.Key.Key, out var cardInfo)) {
                    if (entry.Value == "DEFAULT") continue;

                    var cardOptions = configFile.FindCategory(cardInfo.name);
                    var rarityProperty = cardOptions.GetEntry<CardInfo.Rarity>();

                    // Since other mods pqtch "GetValues" to return a custom enum values, and Enum.Parse doesn't work with that, we have to use GetValues and manually find the value.
                    bool isValidRarity = Enum.GetValues(typeof(CardInfo.Rarity))
                        .Cast<CardInfo.Rarity>()
                        .Any(r => r.ToString() == entry.Value);
                    if (!isValidRarity) continue;

                    CardInfo.Rarity raity = Enum.GetValues(typeof(CardInfo.Rarity))
                            .Cast<CardInfo.Rarity>()
                            .FirstOrDefault(r => r.ToString() == entry.Value);

                    rarityProperty.ValueTyped = raity;
                }
            }
            configFile.SaveOnChange = true;

            Profiler.End("RarityToggleMigration.MigrateRarityToggle");
        }

        private string SanitizeText(string text) {
            return sanitizeRegex.Replace(text, "");
        }
    }
}
