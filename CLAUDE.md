# CLAUDE.md — AI Agent Guidelines for GlowCore

## Project Overview

GlowCore is a **Unity 6 (6000.3.9f1)** game built with URP (Universal Render Pipeline). It is a university Software Engineering project (Semester 4) developed by a team of four.

- **Engine:** Unity 6000.3.9f1
- **Language:** C# 9.0
- **Render Pipeline:** URP 17.3.0
- **Input:** Unity Input System 1.18.0
- **Testing:** NUnit via Unity Test Framework 1.6.0

## Coding Standards

**Follow [CODING-GUIDELINES.md](CODING-GUIDELINES.md) strictly.** Key rules:

### Naming

| Element              | Convention                | Example                          |
|----------------------|---------------------------|----------------------------------|
| Classes / Interfaces | PascalCase (`I` prefix)   | `PlayerController`, `ICollectible` |
| Methods              | PascalCase                | `MovePlayer()`, `TakeDamage()`   |
| Bool methods         | Question prefix           | `IsAlive()`, `CanCollect()`      |
| Parameters / Locals  | camelCase                 | `playerSpeed`, `itemCount`       |
| Private fields       | `m_` prefix + camelCase   | `m_playerHealth`                 |
| Public fields        | PascalCase (avoid in classes) | `PlayerHealth`               |
| Properties           | PascalCase                | `Health`, `Score`                |
| Constants (`const`)  | `k` prefix + PascalCase   | `kMaxHealth`, `kGravity`         |
| Static fields        | `s_` prefix + camelCase   | `s_instanceCount`                |
| Events               | `On` prefix + PascalCase  | `OnPlayerDeath`                  |
| Enums / Values       | PascalCase                | `PlayerState.Idle`               |
| Namespaces           | PascalCase, project-based | `GlowCore.Player`               |

### Class Member Ordering

1. Constants
2. Static Fields
3. Instance Fields
4. Constructors
5. Properties
6. Public Methods
7. Private Methods

### Style Rules

- Always use explicit access modifiers — never rely on defaults.
- No `public` fields in classes — use properties instead. Public fields in structs are fine.
- Prefer expression-bodied members for simple properties: `public int Health => m_health;`
- Avoid `in` parameters (defensive copy performance trap).
- Use `[SerializeField]` to expose private fields to the Unity Inspector.

## Project Structure

```
Assets/
├── Animations/          # Animation clips and controllers
├── Audio/               # Sound effects and music
├── Fonts/               # Custom fonts
├── Materials/           # Shader materials
├── Particles/           # Particle systems
├── Prefabs/             # Reusable game objects (GlowCore/, Items/, Nodes/, Player.prefab)
├── Scenes/              # MainWorldScene + per-developer scenes
├── Scripts/             # All C# code
│   ├── TestingExample/  # Example test subject classes
│   └── Tests/           # Unit, Integration, Mocks
├── ScriptableObjects/   # Data containers
├── Settings/            # URP pipeline settings
├── Sprites/             # 2D graphics
└── Palettes/            # Color palettes
```

## Writing C# Code

- Scripts go under `Assets/Scripts/` in a folder matching their namespace (e.g., `GlowCore.Player` → `Assets/Scripts/Player/`).
- Use `MonoBehaviour` for components attached to GameObjects. Use plain C# classes for logic that doesn't need Unity lifecycle.
- Use the new Input System callbacks (`OnMove`, `OnLook`, etc.) — not the legacy `Input.GetKey` API.
- Separate concerns: input handling, movement, camera, and game logic should be in separate components.
- Use `[SerializeField] private` instead of `public` for Inspector-exposed fields.

## Testing

- **Unit tests** go in `Assets/Scripts/Tests/UnitTests/`.
- **Integration tests** go in `Assets/Scripts/Tests/IntegrationTests/`.
- **Mock objects** go in `Assets/Scripts/Tests/Mocks/`.
- Use NUnit attributes (`[Test]`, `[SetUp]`, `[TearDown]`).
- Use `[UnityTest]` with coroutines for tests that need physics frames (`yield return new WaitForFixedUpdate()`).
- Integration tests can load prefabs via `AssetDatabase.LoadAssetAtPath<GameObject>()`.
- Use a `kEpsilon` constant (e.g., `0.001f`) for float comparisons in tests.

## Git & Branching

- **Main branches:** `main` (production), `dev` (integration)
- **Branch naming:** `<type>/gc-<jira-id>-description`
  - Types: `feature`, `bug`, `docs`, `task`
- PRs target `dev` from feature branches. Reference the Jira issue in PR descriptions.
- Do not commit Unity-generated folders: `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/`, `Build/`.

## CI/CD

- WebGL builds run on every push to `main` and `dev`.
- Windows/Linux builds run on version tags (`v*`).
- Documentation (Typst) builds run on changes to `doc/**` or doc tags (`d*.*`).
- Builds deploy to GitHub Pages. The pipeline caches the `Library/` folder.

## Documentation

- Project documentation lives in `doc/` and uses **Typst** (not Markdown).
- Do not modify `doc/lib.typ` (shared template library) unless specifically asked.
- Meeting protocols go in `doc/protocols/`.

## Things to Avoid

- Do not modify files under `Library/`, `Temp/`, `ProjectSettings/`, or `Packages/` unless explicitly asked.
- Do not add `.meta` files manually — Unity generates these automatically.
- Do not use `GameObject.Find()` at runtime — it is slow and fragile. Use references via `[SerializeField]` or dependency injection.
- Do not use `public` fields in MonoBehaviour classes — use `[SerializeField] private` + a property if external access is needed.
- Do not use legacy Input (`Input.GetKey`, `Input.GetAxis`) — use the Input System package.
- Do not put game logic in `Update()` if it can be event-driven.
