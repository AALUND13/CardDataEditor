using CardDataEditor.DataEditting.Properties;
using System;
using System.Collections.Generic;

namespace CardDataEditor.DataEditting.Config {
    public abstract class CardPropertyConfigEntry {
        public readonly string Name;
        public readonly CardProperty CardOptionProperty;
        public readonly CardOptionsConfig ConfigFile;

        public event Action OnValueChanged;

        public object DefaultValue { get; }
        public bool IsDefaultValue => Equals(CardOptionProperty.GetProperty(), DefaultValue);

        private object value;
        public object Value {
            get => value;
            set {
                this.value = value;
                CardOptionProperty.ApplyProperty(value);
                OnValueChanged?.Invoke();
            }
        }

        protected CardPropertyConfigEntry(string name, CardProperty cardOptionProperty, CardOptionsConfig configFile) {
            Name = name;
            CardOptionProperty = cardOptionProperty;
            ConfigFile = configFile;

            DefaultValue = cardOptionProperty.GetProperty();
            value = DefaultValue;
        }

        public void SyncWithConfig() {
            CardOptionProperty.ApplyProperty(Value);
        }

        public byte[] SerializeValue() {
            return CardOptionProperty.SerializeValue(Value);
        }

        public void DeserializeValue(byte[] data) {
            Value = CardOptionProperty.DeserializeValue(data);
        }

    }


    public class CardPropertyConfigEntry<T> : CardPropertyConfigEntry {
        public readonly CardProperty<T> CardOptionPropertyTyped;

        public readonly T DefaultValueTyped;
        public bool IsDefaultValueTyped =>
            EqualityComparer<T>.Default.Equals(
                CardOptionPropertyTyped.GetPropertyTyped(),
                DefaultValueTyped);

        public T ValueTyped {
            get => (T)Value;
            set => Value = value;
        }

        public CardPropertyConfigEntry(string name, CardProperty<T> cardOptionProperty, CardOptionsConfig configFile) : base(name, cardOptionProperty, configFile) {
            CardOptionPropertyTyped = cardOptionProperty;
            DefaultValueTyped = cardOptionProperty.GetPropertyTyped();
        }

        public byte[] SerializeValueTyped(T value) {
            return CardOptionPropertyTyped.SerializeValueTyped(value);
        }

        public T DeserializeValueTyped(byte[] data) {
            return CardOptionPropertyTyped.DeserializeValueTyped(data);
        }
    }
}
