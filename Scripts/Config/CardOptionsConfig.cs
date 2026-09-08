using BepInEx;
using BepInEx.Logging;
using CardDataEditor.Utils.Debug;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CardDataEditor.DataEditting.Config {
    public class CardOptionsConfig {
        public readonly List<CardOptionsConfigCategory> Categories = new List<CardOptionsConfigCategory>();

        private const string ConfigFileName = "CardOptions.bin";
        private const string ConfigFolderName = "CardDataEditor";

        private static string ConfigPath 
            => Path.Combine(Paths.ConfigPath, ConfigFolderName, ConfigFileName);
        public static bool DoesConfigFileExists 
            => File.Exists(ConfigPath);

        public bool SaveOnChange = true;
        public event Action<CardOptionsConfigCategory, CardPropertyConfigEntry> CategoryChanged;


        public CardOptionsConfigCategory AddCategory(CardOptions cardOptions) {
            var category = new CardOptionsConfigCategory(cardOptions, this);
            category.OnEntryChanged += (CardPropertyConfigEntry entry) => {
                CategoryChanged?.Invoke(category, entry);
                if (SaveOnChange) SaveConfig();
            };
            Categories.Add(category);
            return category;
        }


        public void SaveConfig() {
            Profiler.Start("CardOptionsConfig.SaveConfig");
            string directory = Path.GetDirectoryName(ConfigPath);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var stream = new FileStream(ConfigPath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (var writer = new BinaryWriter(stream)) {
                var nonDefaultCategories = Categories
                    .Where(category => !category.IsDefaultValues)
                    .ToArray();

                writer.Write((byte)1);
                writer.Write((ushort)nonDefaultCategories.Length);

                foreach (var category in nonDefaultCategories) {
                    if (!category.IsDefaultValues) {
                        string categoryName = category.CardOptions.Card.name;
                        writer.Write(categoryName);

                        byte[] data = category.SerializeCategory();
                        writer.Write(data.Length);
                        writer.Write(data);
                    }
                }
            }
            Profiler.End("CardOptionsConfig.SaveConfig");
        }

        public void SyncWithConfig() {
            Profiler.Start("CardOptionsConfig.SyncWithConfig");
            foreach (var category in Categories) {
                category.SyncWithConfig();
            }
            Profiler.End("CardOptionsConfig.SyncWithConfig");
        }

        public void LoadConfig() {
            Profiler.Start("CardOptionsConfig.LoadConfig");
            if (!File.Exists(ConfigPath)) return;
            SaveOnChange = false;

            using (var stream = new FileStream(ConfigPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var reader = new BinaryReader(stream)) {
                byte version = reader.ReadByte();
                if (version != 1) {
                    throw new InvalidDataException($"Unsupported CardOptionsConfig version: {version}");
                }

                ushort categoryCount = reader.ReadUInt16();
                if (categoryCount < 0) {
                    throw new InvalidDataException("Invalid category count.");
                }

                for (int i = 0; i < categoryCount; i++) {
                    string categoryName = reader.ReadString();
                    int length = reader.ReadInt32();

                    if (length < 0 || length > stream.Length - stream.Position) {
                        throw new InvalidDataException($"Invalid data length for category '{categoryName}'.");
                    }

                    byte[] data = reader.ReadBytes(length);
                    if (data.Length != length) {
                        throw new EndOfStreamException($"Unexpected end of config while reading category '{categoryName}'.");
                    }

                    CardOptionsConfigCategory category = FindCategory(categoryName);
                    if (category == null) {
                        LoggerUtils.Log(LogLevel.Warning, $"Unknow category name '{categoryName}', skipping category.");
                        continue;
                    }
                    category.DeserializeCategory(data);
                }
            }

            SaveOnChange = true;
            Profiler.End("CardOptionsConfig.LoadConfig");
        }


        public CardOptionsConfigCategory FindCategory(string name) {
            foreach (var category in Categories) {
                if (category.CardOptions.Card.name == name)
                    return category;
            }

            return null;
        }
    }
}
