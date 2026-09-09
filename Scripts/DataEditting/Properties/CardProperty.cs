using BepInEx.Configuration;
using CardDataEditor.DataEditting.Config;
using UnityEngine;

namespace CardDataEditor.DataEditting.Properties {
    public abstract class CardProperty {
        public CardInfo Card { get; private set; }

        public CardProperty(CardInfo card) {
            Card = card;
        }

        public virtual bool CanShowProperty() => true;
        public virtual GameObject CreateUIProperty(CardPropertyConfigEntry entry) => null;

        public virtual void ApplyPropertyToPreviewCard(GameObject cardObject, CardInfo cardInfo) { }

        public abstract CardPropertyConfigEntry CreateCardPropertyConfigEntry(CardOptionsConfig configFile);

        public abstract string GetCategoryName();
        public abstract string GetPropertyName();
        public virtual string GetDescription() => null;
        public virtual string GetSerializeName() => GetPropertyName();

        public abstract void ApplyProperty(object value);
        public abstract object GetProperty();

        public abstract byte[] SerializeValue(object value);
        public abstract object DeserializeValue(byte[] data);
    }

    public abstract class CardProperty<T> : CardProperty {
        public CardProperty(CardInfo card) : base(card) { }

        public abstract void ApplyProperty(T value);
        public abstract T GetPropertyTyped();

        public abstract byte[] SerializeValueTyped(T value);
        public abstract T DeserializeValueTyped(byte[] data);


        public override CardPropertyConfigEntry CreateCardPropertyConfigEntry(CardOptionsConfig configFile) =>
            new CardPropertyConfigEntry<T>(GetSerializeName(), this, configFile);

        public override void ApplyProperty(object value) =>
            ApplyProperty((T)value);
        public override object GetProperty() =>
            GetPropertyTyped();

        public override byte[] SerializeValue(object value) =>
            SerializeValueTyped((T)value);
        public override object DeserializeValue(byte[] data) =>
            DeserializeValueTyped(data);
    }
}