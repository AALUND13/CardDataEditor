using BepInEx.Logging;
using CardDataEditor.Utils.Debug;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CardDataEditor.DataEditting.Config {
    public class CardOptionsConfigCategory {
        public readonly CardOptions CardOptions;
        public readonly CardOptionsConfig ConfigFile;
        public readonly List<CardPropertyConfigEntry> ConfigEntries = new List<CardPropertyConfigEntry>();

        public event Action<CardPropertyConfigEntry> OnEntryChanged;

        public bool IsDefaultValues =>
            ConfigEntries.TrueForAll(entry => entry.IsDefaultValue);


        public CardOptionsConfigCategory(CardOptions cardOptions, CardOptionsConfig configFile) {
            CardOptions = cardOptions;
            ConfigFile = configFile;

            foreach (var property in cardOptions.Properties.Values) {
                CardPropertyConfigEntry entry = property.CreateCardPropertyConfigEntry(configFile);
                entry.OnValueChanged += () => OnEntryChanged.Invoke(entry);
                ConfigEntries.Add(entry);
            }
        }


        public List<CardPropertyConfigEntry> GetNonDefaultEntries() {
            return ConfigEntries
                .Where(entry => !entry.IsDefaultValue)
                .ToList();
        }


        public void SyncWithConfig() {
            Profiler.Start("CardOptionsConfigCategory.SyncWithConfig");
            foreach (var entry in ConfigEntries) {
                entry.SyncWithConfig();
            }
            Profiler.End("CardOptionsConfigCategory.SyncWithConfig");
        }


        public byte[] SerializeCategory() {
            Profiler.Start("CardOptionsConfigCategory.SerializeCategory");

            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream)) {
                var entries = GetNonDefaultEntries();

                writer.Write((byte)entries.Count);
                foreach (var entry in entries) {
                    writer.Write(entry.Name);

                    byte[] data = entry.SerializeValue();

                    writer.Write(data.Length);
                    writer.Write(data);
                }

                Profiler.End("CardOptionsConfigCategory.SerializeCategory");
                return stream.ToArray();
            }

        }


        public void DeserializeCategory(byte[] data) {
            Profiler.Start("CardOptionsConfigCategory.DeserializeCategory");

            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream)) {
                byte entryCount = reader.ReadByte();
                if (entryCount < 0) {
                    LoggerUtils.Log(LogLevel.Error, "Invalid config entry count.");
                    return;
                }

                for (int i = 0; i < entryCount; i++) {
                    string name = reader.ReadString();
                    int length = reader.ReadInt32();

                    if (length < 0 || length > stream.Length - stream.Position) {
                        LoggerUtils.Log(LogLevel.Warning, $"Invalid data length {length} for config entry '{name}'. Skipping entry.");
                        continue;
                    }

                    byte[] entryData = reader.ReadBytes(length);

                    if (entryData.Length != length) {
                        LoggerUtils.Log(LogLevel.Warning, $"Unexpected end of data while reading config entry '{name}'. Skipping entry.");
                        continue;
                    }

                    CardPropertyConfigEntry entry = GetEntry(name);
                    if (entry == null) {
                        LoggerUtils.Log(LogLevel.Warning, $"Unknow property name '{name}', skipping property.");
                        continue;
                    }

                    entry.DeserializeValue(entryData);
                }
            }

            Profiler.End("CardOptionsConfigCategory.DeserializeCategory");
        }


        public CardPropertyConfigEntry GetEntry(string name) {
            return ConfigEntries.FirstOrDefault(entry => entry.Name == name);
        }

        public CardPropertyConfigEntry<T> GetEntry<T>() {
            return (CardPropertyConfigEntry<T>)ConfigEntries.FirstOrDefault(entry => entry is CardPropertyConfigEntry<T>);
        }

        public override string ToString() {
            return $"CardOptionsConfigCategory: {CardOptions.Card.name} with {ConfigEntries.Count} entries";
        }
    }
}
