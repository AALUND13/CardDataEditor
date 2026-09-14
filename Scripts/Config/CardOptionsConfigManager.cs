using BepInEx.Logging;
using CardDataEditor.DataEditting.Registries;
using CardDataEditor.Utils.Debug;
using System;

namespace CardDataEditor.DataEditting.Config {
    public static class CardOptionsConfigManager {
        public static CardDataConfigFile Config { get; private set; }
        public static event Action<CardDataConfigFile> OnConfigFirstCreated;

        public static void RegisterConfig() {
            Profiler.Start("CardOptionsConfigManager.RegisterConfig");

            Config = new CardDataConfigFile();
            try {
                LoggerUtils.Log(LogLevel.Info, "Loading cards options config...");
                Profiler.Start("CardOptionsConfigManager.RegisterConfig.AddCategories");
                foreach (var cardOptions in CardOptionRegistry.AllCardOptions) {
                    Config.AddCategory(cardOptions);
                }
                Profiler.End("CardOptionsConfigManager.RegisterConfig.AddCategories");

                Profiler.Start("CardOptionsConfigManager.RegisterConfig.LoadOrCreate");
                if (!CardDataConfigFile.DoesConfigFileExists) {
                    OnConfigFirstCreated?.Invoke(Config);
                    Config.SaveConfig();
                } else {
                    Config.LoadConfig();
                }
                Profiler.End("CardOptionsConfigManager.RegisterConfig.LoadOrCreate");

                LoggerUtils.Log(LogLevel.Info, "CardOptionsConfig loaded successfully.");
            } catch (Exception e) {
                LoggerUtils.Log(LogLevel.Error, $"Failed to load CardOptionsConfig: {e}");
                try {
                    Config.SaveConfig();
                } catch (Exception ex) {
                    LoggerUtils.Log(LogLevel.Error, $"Failed to save new CardOptionsConfig: {ex}");
                }
            }

            Profiler.End("CardOptionsConfigManager.RegisterConfig");
        }
    }
}
