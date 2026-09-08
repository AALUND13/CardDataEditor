# Card Data Editor (v1.0.0)
## Description
**Card Data Editor** is a mod that allows you to edit **card properties**, including:
- **Rarity**
- **Theme**
- **Card Amount**
- **Detach Class** - available for certain class cards
These properties can be modified through the **Card Data Editor** menu.
### Detaching Class Cards
By default, players can only detach class cards that have the `ClassDetachable` card category.

If you want to detach **ANY** class card, you can enable **Danger Mode** in the **Card Data Editor** settings. This allows any class card to be detached from its class, regardless of whether it has the `ClassDetachable` category.

> **Warning:** Danger Mode allows you to detach class cards that were **not designed to be detached**. Doing so may cause cards to behave incorrectly, break card functionality, or cause other unexpected issues. **Only enable Danger Mode if you understand and accept the risks.**

## Migrations
If you previously used mods such as [Rarity Toggle](https://thunderstore.io/c/rounds/p/AALUND13/RarityToggle/) or [Theme Toggle](https://thunderstore.io/c/rounds/p/Root/ThemeToggle/), **Card Data Editor** will automatically migrate their configurations to its own config system.

This allows you to easily replace those mods with **Card Data Editor** without having to manually recreate your settings.
## For Developers
If you are developing a mod and want to integrate it with **Card Data Editor**, there are two ways you can extend its functionality.

### Making a Class Card Detachable
If your class card can safely be detached from its class without breaking anything, add the `ClassDetachable` card category to your card.

Players will then be able to detach the card from its class without needing to enable **Danger Mode**.

> **Warning:** Only mark a card as `ClassDetachable` if detaching it is safe. Doing so may cause unexpected behavior or break your card if it relies on its class.

### Adding Custom Card Properties
You can add your own custom card properties by creating a class that inherits from `CardProperty<T>`:
```csharp
public class MyCardProperty : CardProperty<MyValueType> {
    // Your property implementation
}
```

You can then register your property using:
```csharp
PropertyRegistry.RegisterProperty<MyCardProperty>();
```

If you only want the property to be available for a specific card, you can use:
```csharp
PropertyRegistry.RegisterPropertyForCard<MyCardProperty>(MyCard);
```

For more examples and implementation details, take a look at the **Card Data Editor** source code.