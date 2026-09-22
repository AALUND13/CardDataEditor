using CardDataEditor.DataEditting.Properties;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CardDataEditor.DataEditting.Config {
    public abstract class CardPropertyConfigEntry {
        public readonly string Name;
        public readonly CardProperty CardOptionProperty;
        public readonly CardDataConfigFile ConfigFile;

        public event Action OnValueChanged;

        public object DefaultValue { get; }
        public bool IsDefaultValue => CompareObjects(CardOptionProperty.GetProperty(), DefaultValue);

        private object value;
        public object Value {
            get => value;
            set {
                this.value = value;
                CardOptionProperty.ApplyProperty(value);
                OnValueChanged?.Invoke();
            }
        }

        protected CardPropertyConfigEntry(string name, CardProperty cardOptionProperty, CardDataConfigFile configFile) {
            if (name == null) {
                throw new ArgumentNullException(
                    nameof(name)
                );
            } else if (cardOptionProperty == null) {
                throw new ArgumentNullException(
                    nameof(cardOptionProperty)
                );
            } else if (configFile == null) {
                throw new ArgumentNullException(
                    nameof(configFile)
                );
            } else if (cardOptionProperty.ConfigEntry != null) {
                throw new InvalidOperationException(
                    $"Card property ${cardOptionProperty.GetPropertyName()} is already assigned to a config entry\n" +
                    $"You cannot assigned multiple config entry to a single card property."
                );
            }

            Name = name;
            CardOptionProperty = cardOptionProperty;
            ConfigFile = configFile;

            DefaultValue = cardOptionProperty.GetProperty();
            value = DefaultValue;

            cardOptionProperty.ConfigEntry = this;
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

        private bool CompareObjects(object a, object b) {
            if(a is IList listA && b is IList listB) {
                if (listA.Count != listB.Count) {
                    return false;
                }

                for(int i = 0; i < listA.Count; i++) {
                    if (!Equals(listA[i], listB[i])) {
                        return false;
                    }
                }

                return true;
            }

            return Equals(a, b);
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

        public CardPropertyConfigEntry(string name, CardProperty<T> cardOptionProperty, CardDataConfigFile configFile) : base(name, cardOptionProperty, configFile) {
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
