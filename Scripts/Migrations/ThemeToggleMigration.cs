using BepInEx;
using BepInEx.Configuration;
using CardDataEditor.DataEditting.Config;
using CardDataEditor.Utils.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;

namespace CardDataEditor.Scripts.Migration {
    [BepInDependency("AALUND13.Card.Data.Editor", BepInDependency.DependencyFlags.HardDependency)]

    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class ThemeToggleMigration : BaseUnityPlugin {
        private const string ModId = "Theme.Toggle";
        private const string ModName = "'Theme Toggle' -> 'Card Data Editor' Migration";
        public const string Version = "9999.9999.9999";


        private void Start() {
            CardOptionsConfigManager.OnConfigFirstCreated += (CardOptionsConfig config) => MigrateRarityToggle(config);
        }


        public void MigrateRarityToggle(CardOptionsConfig configFile) {
            Profiler.Start("ThemeToggleMigration.MigrateRarityToggle");

            configFile.SaveOnChange = false;
            foreach (var entry in (Dictionary<ConfigDefinition, string>)Config.GetPropertyValue("OrphanedEntries")) {
                if (int.TryParse(entry.Value, out int value)) {
                    if (value == -1) continue;

                    var cardInfo = ModdingUtils.Utils.Cards.instance.GetCardWithObjectName(entry.Key.Key);
                    var cardOptions = configFile.FindCategory(cardInfo.name);
                    var rarityProperty = cardOptions.GetEntry<CardThemeColor.CardThemeColorType>();

                    // Since other mods pqtch "GetValues" to return a custom enum values, and Enum.Parse doesn't work with that, we have to use GetValues and manually find the value.
                    bool isValidRarity = Enum.GetValues(typeof(CardThemeColor.CardThemeColorType))
                        .Cast<CardThemeColor.CardThemeColorType>()
                        .Any(t => (int)t == value);

                    if (!isValidRarity) continue;
                    var raity = Enum.GetValues(typeof(CardThemeColor.CardThemeColorType))
                            .Cast<CardThemeColor.CardThemeColorType>()
                            .FirstOrDefault(t => (int)t == value);

                    rarityProperty.ValueTyped = raity;
                }
            }
            configFile.SaveOnChange = true;

            Profiler.End("ThemeToggleMigration.MigrateRarityToggle");
        }
    }
}
