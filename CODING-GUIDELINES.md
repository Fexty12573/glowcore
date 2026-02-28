# Coding Guidelines
This document refers to guidelines specific to any Unity C# code written for the project. These guidelines are based on the existing Unity C# coding standards and best practices, found [here](https://unity.com/how-to/naming-and-code-style-tips-c-scripting-unity).

## Naming Conventions
- **Classes and Interfaces**: Use PascalCase. For interfaces, prefix with 'I'. Example: `PlayerController`, `ICollectible`.
- **Methods**: Use PascalCase. Example: `MovePlayer()`, `CollectItem()`.
  - For methods that return `bool`, consider prefixing with a question word. Example: `IsAlive()`, `CanCollectItem()`.
- **Parameters and Local Variables**: Use camelCase. Example: `playerSpeed`, `itemCount`.
  - Avoid overly short names; be descriptive where possible. Exceptions are common loop variables like `i`, `j`, `k` or variables in math expressions.
- **Constants**: Use PascalCase and prefix with 'k'. Example: `kMaxHealth`, `kGravity`.
  - This refers specifically to `const` identifiers, not `readonly` fields, which should follow the same convention as other fields.
- **Private Fields**: Use camelCase and prefix with `m_`. Example: `m_playerHealth`, `m_enemyCount`.
- **Public Fields**: Use PascalCase. Example: `PlayerHealth`, `EnemyCount`.
- **Properties**: Use PascalCase. Example: `Health`, `Score`.
- **Events**: Use PascalCase and prefix with 'On'. Example: `OnPlayerDeath`, `OnItemCollected`.
- **Namespaces**: Use PascalCase and reflect the project structure. Example: `GlowCore.Player`, `GlowCore.Enemies`.
- **Enums**: Use PascalCase. Example: `PlayerState`, `EnemyType`.
  - For enum values, also use PascalCase. Example: `Idle`, `Running`, `Jumping`.

## General Guidelines
### Properties
For `get` only properties, prefer expression-bodied members for simplicity. Example:
```cs
public int Health => m_playerHealth;
```
This can also be applied to `get/set` properties when the logic is simple. Example:
```cs
public int Score
{
    get => m_score;
    set => m_score = value;
}
```

### Method Parameters
Try not to use `in` parameters, as they can be a performance trap due to the compiler potentially creating defensive copies.

### Class Members
- Always be explicit with access modifiers. Don't rely on defaults.
- Avoid `public` fields in classes. Prefer using properties.
  - Public fields in structs are fine.

### Class Member Ordering
1. Constants
2. Static Fields
3. Instance Fields
4. Constructors
5. Properties
6. Public Methods
7. Private Methods

Example:
```cs
public class ExampleClass : MonoBehavior
{
    // Constants
    private const int kMaxHealth = 100;

    // Static Fields
    private static int s_instanceCount;

    // Instance Fields
    private int m_currentHealth;

    // Constructors
    public ExampleClass()
    {
        m_currentHealth = kMaxHealth;
        s_instanceCount++;
    }

    // Properties
    public int Health => m_currentHealth;

    // Public Methods
    public void TakeDamage(int damage)
    {
        m_currentHealth -= damage;
        if (m_currentHealth < 0)
            m_currentHealth = 0;
    }

    // Private Methods
    private void ResetHealth()
    {
        m_currentHealth = kMaxHealth;
    }
}
```
