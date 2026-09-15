using CardDataEditor.DataEditting.Config;
using UnityEngine;

namespace CardDataEditor.DataEditting.Properties {
    public abstract class CardProperty {
        public CardInfo Card { get; internal set; }
        public CardOptions CardOptions { get; internal set; }
        public CardPropertyConfigEntry ConfigEntry { get; internal set; }

        public virtual void OnInit() { }
        public virtual void ApplyPropertyToPreviewCard(GameObject cardObject, CardInfo cardInfo) { }

        public virtual bool CanShowProperty() => true;
        public virtual GameObject CreateUIProperty(CardPropertyConfigEntry entry) => null;

        public abstract CardPropertyConfigEntry CreateCardPropertyConfigEntry(CardDataConfigFile configFile);

        public abstract string GetCategoryName();
        public abstract string GetPropertyName();

        public virtual string GetSerializeName() => GetPropertyName();

        public abstract void ApplyProperty(object value);
        public abstract object GetProperty();

        public abstract byte[] SerializeValue(object value);
        public abstract object DeserializeValue(byte[] data);
    }

    public abstract class CardProperty<T> : CardProperty {
        public abstract void ApplyProperty(T value);
        public abstract T GetPropertyTyped();

        public abstract byte[] SerializeValueTyped(T value);
        public abstract T DeserializeValueTyped(byte[] data);


        public override CardPropertyConfigEntry CreateCardPropertyConfigEntry(CardDataConfigFile configFile) =>
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