using CardDataEditor.DataEditting.Properties;
using System;
using System.Collections.Generic;

namespace CardDataEditor.DataEditting.Registries {
    public static class PropertyRegistry {
        private static readonly List<Type> registeredProperties = new List<Type>();
        private static readonly Dictionary<CardInfo, List<Type>> registeredCardProperties = new Dictionary<CardInfo, List<Type>>();


        internal static void RegisterDefaultProperties() {
            RegisterProperty<RarityCardProperty>();
            RegisterProperty<ThemeCardProperty>();
            RegisterProperty<DetachClassProperty>();
            RegisterProperty<AmountCardProperty>();
        }


        public static void RegisterProperty<T>() where T : CardProperty {
            Type propertyType = typeof(T);

            if (registeredProperties.Contains(propertyType)) {
                throw new InvalidOperationException(
                    $"The card property '{propertyType.Name}' is already registered globally."
                );
            }

            registeredProperties.Add(propertyType);
        }

        public static void RegisterPropertyForCard<T>(CardInfo card) where T : CardProperty {
            if (card == null) {
                throw new ArgumentNullException(nameof(card));
            }

            Type propertyType = typeof(T);

            if (registeredProperties.Contains(propertyType)) {
                throw new InvalidOperationException(
                    $"The card property '{propertyType.Name}' is already registered globally " +
                    $"and cannot be registered specifically for card '{card.name}'."
                );
            }

            if (!registeredCardProperties.TryGetValue(card, out List<Type> properties)) {
                properties = new List<Type>();
                registeredCardProperties.Add(card, properties);
            }

            if (properties.Contains(propertyType)) {
                throw new InvalidOperationException(
                    $"The card property '{propertyType.Name}' is already registered for card '{card.name}'."
                );
            }

            properties.Add(propertyType);
        }


        public static void AddProperties(CardOptions cardOptions) {
            foreach (Type propertyType in registeredProperties) {
                cardOptions.AddProperty(propertyType);
            }

            if (registeredCardProperties.TryGetValue(cardOptions.Card, out List<Type> properties)) {
                foreach (Type propertyType in properties) {
                    cardOptions.AddProperty(propertyType);
                }
            }
        }
    }
}