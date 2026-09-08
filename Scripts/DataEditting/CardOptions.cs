using CardDataEditor.DataEditting.Properties;
using CardDataEditor.Utils.Debug;
using System;
using System.Collections.Generic;
using System.IO;

namespace CardDataEditor.DataEditting {
    public class CardOptions {
        public readonly CardInfo Card;
        public readonly Dictionary<Type, CardProperty> Properties = new Dictionary<Type, CardProperty>();

        public CardOptions(CardInfo cardInfo) {
            Profiler.Start("CardOptions.CardOptions");
            Card = cardInfo;
            Profiler.End("CardOptions.CardOptions");
        }

        public void AddProperty(Type type) {
            Profiler.Start("CardOptions.AddProperty");
            if (typeof(CardProperty).IsAssignableFrom(type)) {
                var propertyInstance = (CardProperty)Activator.CreateInstance(type, new object[] { Card });
                Properties[type] = propertyInstance;
            }
            Profiler.End("CardOptions.AddProperty");
        }

        public void AddProperty<T>() where T : CardProperty, new() {
            Profiler.Start("CardOptions.AddProperty<T>");
            T property = new T();
            if (property.CanShowProperty()) {
                Properties[typeof(T)] = property;
            }
            Profiler.End("CardOptions.AddProperty<T>");
        }

        public T GetProperty<T>() where T : CardProperty {
            if (Properties.TryGetValue(typeof(T), out var property)) {
                var result = property as T;
                return result;
            }

            return null;
        }

        public byte[] Serialize() {
            Profiler.Start("CardOptions.Serialize");
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream)) {
                foreach (var property in Properties.Values) {
                    byte[] data = property.SerializeValue(property.GetProperty());

                    writer.Write(data.Length);
                    writer.Write(data);
                }

                var result = stream.ToArray();
                Profiler.End("CardOptions.Serialize");
                return result;
            }
        }

        public void Deserialize(byte[] data) {
            Profiler.Start("CardOptions.Deserialize");
            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream)) {
                foreach (var property in Properties.Values) {
                    int length = reader.ReadInt32();
                    byte[] propertyData = reader.ReadBytes(length);

                    property.ApplyProperty(property.DeserializeValue(propertyData));
                }
            }
            Profiler.End("CardOptions.Deserialize");
        }

        public override string ToString() {
            return $"CardOptions for {Card.name} with {Properties.Count} properties.";
        }
    }
}