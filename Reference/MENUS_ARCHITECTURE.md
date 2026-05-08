# Menu System Architecture

> Reference document for the GlowCore menu system: title, new game, pause, settings, and the
> scene-transition flow.
> Last updated: 2026-05-08 (GC-76 initial implementation).

---

## Overview

The menu system has three layers, mirroring the inventory architecture: a **service layer**
(plain C# unless engine APIs are required), a **presentation layer** (uGUI / TextMeshPro
MonoBehaviours), and a **persistence layer** (PlayerPrefs, SaveData, AudioMixer, UnityEngine.Screen).
Presentation depends only on interfaces — concrete services are wired by per-scene bootstrappers.

```
┌─────────────────────────────────────────────────────────────┐
│   PRESENTATION LAYER (MonoBehaviours, GlowCore.UI.Menus)    │
│                                                             │
│   TitleScreen / NewGameScreen / ConfirmDialogScreen         │
│   PauseScreen / PauseButton / PauseInputHandler             │
│   SettingsScreen + ISettingControl impls                    │
│   SceneTransitionOverlay (DontDestroyOnLoad)                │
└──────────────┬──────────────────────────────────────────────┘
               │ IMenuManager · IMenuScreen ·
               │ ISceneTransition · IGameStateController ·
               │ INewGameService · IGameLaunchContext ·
               │ ISettingsRepository · IAudioBridge ·
               │ IDisplayService · ISettingsCategory ·
               │ ISettingControl · IEscapeConsumer
┌──────────────▼──────────────────────────────────────────────┐
│   SERVICE LAYER (plain C# unless engine API needed)         │
│                                                             │
│   MenuManager + MenuScreenLocator                           │
│   SceneTransitionService · GameStateController              │
│   NewGameService · GameLaunchContext (DontDestroyOnLoad MB) │
│   PlayerPrefsSettingsRepository (over IPlayerPrefsBackend)  │
│   AudioBridge · AudioMixerVolumeApplier (MB)                │
│   DisplayService · SettingsRowFactory                       │
│   AudioSettingsCategory · DisplaySettingsCategory           │
│   EscapeRouter + InventoryEscapeConsumer +                  │
│   PauseEscapeConsumer                                       │
│   SaveDataGateway (wraps static SaveData calls)             │
└──────────────┬──────────────────────────────────────────────┘
               │
┌──────────────▼──────────────────────────────────────────────┐
│   PERSISTENCE / ENGINE                                      │
│   PlayerPrefs · SaveData · UnityEngine.Screen · AudioMixer  │
└─────────────────────────────────────────────────────────────┘
```

The bootstrappers (`MenuBootstrapper` on `TitleScene`, `GameBootstrapper` on `MainWorldScene`)
compose the service graph in `Awake()` and inject services into screens via `Initialize(...)`.
This mirrors the existing `AutosaveService.Initialize(...)` pattern.

---

## Namespace and assembly

| Scope | Namespace | Assembly |
|---|---|---|
| All menu code | `GlowCore.UI.Menus` | `Gameplay.asmdef` |
| Tests | `Tests.UnitTests.Menus` | `UnitTests.asmdef` |
| Test fakes | `Tests.Mocks.Menus` | `Gameplay.asmdef` (mirrors `MockSaveService` placement) |

`Gameplay.asmdef` references TextMeshPro and Unity.InputSystem. `UnityEngine.Audio` and
`UnityEngine.UI` are part of the runtime and resolve without extra references.

---

## Interfaces

### `IMenuScreen` — `Assets/Scripts/UI/Menus/Core/IMenuScreen.cs`

| Member | Kind | Purpose |
|---|---|---|
| `Id` | Property | `MenuScreenId` — Title, NewGame, Pause, Settings, ConfirmDialog |
| `BlocksGameplay` | Property | True when this screen requires `timeScale=0` |
| `Show()` | Command | Make screen visible (set CanvasGroup alpha=1) |
| `Hide()` | Command | Make screen invisible |

`IMenuArgsReceiver<TArgs>` — implemented by screens that need an argument payload (e.g.
`ConfirmDialogScreen` receives `ConfirmDialogArgs { Message, OnConfirm }` via
`IMenuManager.OpenWithArgs`).

### `IMenuManager` — `Assets/Scripts/UI/Menus/Core/IMenuManager.cs`

| Member | Kind | Purpose |
|---|---|---|
| `Current` | Property | Currently visible screen, or `null` |
| `Open(MenuScreenId)` | Command | Show a screen, push the previous onto the back-stack |
| `OpenWithArgs<TArgs>(MenuScreenId, TArgs)` | Command | Open and forward a payload to the screen if it implements `IMenuArgsReceiver<TArgs>` |
| `Back()` | Command | Pop the back-stack; if empty, calls `CloseAll()` |
| `CloseAll()` | Command | Hide current and clear history |
| `OnScreenChanged` | Event | `Action<IMenuScreen>` — fires on Open/Back/CloseAll |

`IMenuScreenLocator` — registry the manager queries (`Get(MenuScreenId)`); implementations
register/unregister screens. The default implementation is a `Dictionary<MenuScreenId, IMenuScreen>`.

### `ISceneTransition` — `Assets/Scripts/UI/Menus/Services/ISceneTransition.cs`

| Member | Kind | Purpose |
|---|---|---|
| `LoadTitleScene()` | Command | Routes through `SceneTransitionOverlay` with subtitle "Returning home..." |
| `LoadMainWorldScene()` | Command | Routes through overlay with "Awakening..." |
| `QuitApplication()` | Command | `Application.Quit()` in build, stops play in editor |

### `IGameStateController` — `Assets/Scripts/UI/Menus/Services/IGameStateController.cs`

| Member | Kind | Purpose |
|---|---|---|
| `IsPaused` | Property | True when `Time.timeScale == 0` was set by Pause |
| `Pause()` / `Resume()` | Command | Idempotent; flip `Time.timeScale` |
| `SaveNow()` | Command | Delegates to `ISaveService.Save` |
| `ReturnToMainMenu()` | Command | Save, resume timescale, load title scene |

### `INewGameService` — `Assets/Scripts/UI/Menus/Services/INewGameService.cs`

| Member | Kind | Purpose |
|---|---|---|
| `SaveExists` | Property | Delegates to `ISaveDataGateway.Exists()` |
| `StartNewGame(string worldName)` | Command | `gateway.Delete()` → `launchContext.SetNewGame(name)` → `sceneTransition.LoadMainWorldScene()` |

### `IGameLaunchContext` — `Assets/Scripts/UI/Menus/Services/IGameLaunchContext.cs`

DontDestroyOnLoad MonoBehaviour singleton that carries the world name across the scene change.
`WorldGrid.Start()` reads it: if `Mode == NewGame`, the existing save was already deleted by
`NewGameService` and the world name is seeded onto the next save; the context then resets to
`Continue` so subsequent reloads use the normal load path.

### `ISettingsRepository` — `Assets/Scripts/UI/Menus/Services/ISettingsRepository.cs`

`GetFloat/Int/Bool` and `SetFloat/Int/Bool` plus `Save()`. Implemented by
`PlayerPrefsSettingsRepository`, which is constructed over `IPlayerPrefsBackend` so unit tests
can inject a `Dictionary`-based backend (see `FakePlayerPrefsBackend`).

### `IAudioBridge` — `Assets/Scripts/UI/Menus/Services/IAudioBridge.cs`

Pure-event interface (`OnMasterVolumeChanged`, `OnMusicVolumeChanged`, `OnSfxVolumeChanged`)
plus `RaiseMaster/Music/Sfx`. The slider's `Write` callback both stores the value via the
repository and raises the event. `AudioMixerVolumeApplier` (MonoBehaviour) subscribes to the
events and writes to a serialized `AudioMixer` via `SetFloat(parameter, LinearToDb(v))`. If no
mixer is wired the slider still saves, just doesn't change volume.

### `IDisplayService` — `Assets/Scripts/UI/Menus/Services/IDisplayService.cs`

| Member | Purpose |
|---|---|
| `AvailableResolutions` | Deduped by (width, height), highest refresh-rate per pair, sorted ascending |
| `MouseSensitivity` | Current slider value (0..1) |
| `SetFullscreen/SetResolution/SetMouseSensitivity` | Apply via `UnityEngine.Screen` and raise `OnMouseSensitivityChanged` |

`PlayerCamera` calls `Initialize(displayService)` from `GameBootstrapper`, subscribes to
`OnMouseSensitivityChanged`, and scales rotation by `DisplayService.SliderToMultiplier(slider)`
(slider 0..1 → multiplier 0.1..3.0).

### `ISettingsCategory` and `SettingDescriptor`

A category is a label + an `IReadOnlyList<SettingDescriptor>`. A descriptor is a small data
class with `Key`, `Label`, `Kind` (Slider / Toggle / Dropdown), value range or option list, and
`Read()` / `Write(object)` closures. Static factories build them:

- `SettingDescriptor.Slider(key, label, min, max, Func<float>, Action<float>)`
- `SettingDescriptor.Toggle(key, label, Func<bool>, Action<bool>)`
- `SettingDescriptor.Dropdown(key, label, options, Func<int>, Action<int>)`

Adding a setting = one new descriptor in a category — no edits to `SettingsScreen`.

### `ISettingControl`

`Bind(SettingDescriptor)` and `Unbind()`. Implemented by `SliderSettingControl`,
`ToggleSettingControl`, `DropdownSettingControl` (all MonoBehaviours on the row prefabs at
`Assets/Prefabs/UI/Menus/SettingRow_*.prefab`). Each control subscribes to its UI widget on
`Bind`, unsubscribes on `Unbind`, and writes through `descriptor.Write(...)` on user input.

`SettingsRowFactory` is a plain-C# registry — bootstrappers register a builder
(`Func<Transform, ISettingControl>`) per `SettingControlKind`. Adding a new kind (e.g. color
picker, key rebind) is one new control prefab + one register call, with `SettingsScreen`
untouched.

### `IEscapeConsumer` — `Assets/Scripts/UI/Menus/Pause/IEscapeConsumer.cs`

| Member | Purpose |
|---|---|
| `Priority` | Higher consumers run first |
| `TryConsumeEscape()` | Return `true` if Esc was handled; the router stops dispatch |

`EscapeRouter` keeps a priority-sorted list and dispatches Esc on each `Update` of
`PauseInputHandler`. Adding a new screen that should swallow Esc means a new
`IEscapeConsumer` and one register call in `GameBootstrapper`.

Bundled consumers:
- `InventoryEscapeConsumer` (priority 100): closes crafting → glowcore → inventory in that
  order via `IInventoryService.RequestCloseUI()`. Returns `false` if nothing is open so the
  router falls through.
- `PauseEscapeConsumer` (priority 0): opens pause when `MenuManager.Current == null`,
  resumes + closes-all on Pause, swallows Esc on `ConfirmDialog`, and falls back to
  `MenuManager.Back()` for Settings / NewGame / etc.

### `ISaveDataGateway`

Thin wrapper around `SaveData.Exists()` / `SaveData.Delete()` so `NewGameService` is
testable with `FakeSaveDataGateway`.

---

## OCP seams

| Extension | What you do | What you don't touch |
|---|---|---|
| New menu screen | Implement `IMenuScreen`, register prefab in the bootstrapper's `MenuScreenLocator`. Optionally implement `IMenuArgsReceiver<T>` for payloads. | Existing screens, `MenuManager`. |
| New settings category | Implement `ISettingsCategory`, build `SettingDescriptor`s, add the category to the bootstrapper's category list. `SettingsScreen` iterates whatever it gets — no `switch` on type. | `SettingsScreen`, existing categories. |
| New control type (color, key-rebind, …) | Implement `ISettingControl`, ship a prefab, register a builder with `SettingsRowFactory` in the bootstrapper. | Existing controls, the descriptor system. |
| New Esc behavior (e.g. dialog close) | Implement `IEscapeConsumer`, register on the router with appropriate priority. | `PauseInputHandler`, existing consumers. |
| Different settings storage (JSON, cloud) | Implement `ISettingsRepository` directly (or `IPlayerPrefsBackend` to keep using PlayerPrefs serialisation but redirect storage). Wire in the bootstrapper. | Categories, descriptors, screen. |

---

## Bootstrappers

### `MenuBootstrapper` — TitleScene root

Composes the service graph in `Awake()`:

1. `GameLaunchContext.Instance` (lazily creates the DontDestroyOnLoad MB).
2. `SaveDataGateway`, `SceneTransitionService`, `NewGameService`.
3. `UnityPlayerPrefsBackend` → `PlayerPrefsSettingsRepository`, `AudioBridge`, `DisplayService`.
4. `m_audioApplier?.Initialize(audioBridge)` — wires the mixer subscriber.
5. `ApplyPersistedSettings(...)` — reads master/music/sfx and mouse-sensitivity from the
   repository and raises the bridge events / `SetMouseSensitivity` so initial values are
   correct before any UI shows.
6. `MenuScreenLocator` + `MenuManager`.
7. Initialize and register `TitleScreen`, `NewGameScreen`, `ConfirmDialogScreen`,
   `SettingsScreen` (with row factory + categories).
8. `Start()`: `MenuManager.Open(Title)`.

### `GameBootstrapper` — MainWorldScene root

Same service graph as `MenuBootstrapper`, plus:

- A `GameStateController(saveService, sceneTransition)`.
- A scene-local `MenuManager` registering `PauseScreen`, in-game `SettingsScreen`, in-game
  `ConfirmDialogScreen`.
- `m_pauseButton?.Initialize(gameState, menuManager)`.
- `EscapeRouter` with `InventoryEscapeConsumer` (if `PlayerInventory` is found in the scene)
  and `PauseEscapeConsumer`. A `PauseInputHandler` is added at runtime and given the router.
- `m_worldGrid?.SetLaunchContext(launchContext)` — so the world picks up the new-game world
  name.
- `m_playerCamera?.Initialize(displayService)` — so the camera respects mouse-sensitivity.

The two bootstrappers do not share screen instances; each scene owns its own pause-family /
settings panels (intentional — keeps scenes self-contained).

---

## Scene transitions

`SceneTransitionService.LoadMainWorldScene()` and `LoadTitleScene()` route through
`SceneTransitionOverlay.Instance.LoadScene(name, subtitle)`.

`SceneTransitionOverlay` is a `DontDestroyOnLoad` singleton built lazily on first access:

- Procedurally-generated vertical-gradient background (dark indigo → black, gamma-eased).
- Pulsing `GLOWCORE` wordmark (alpha 0.55–1.0 at ~2 Hz, driven by `Time.unscaledTime` so it
  works while `timeScale = 0`).
- Small gold divider beneath the wordmark and 9 deterministic ember dots scattered above it.
- Subtitle text (`Awakening...` / `Returning home...`).

Timing: ~1 s total — 0.25 s fade-in, ≥0.55 s minimum hold while `LoadSceneAsync` runs (the
overlay only allows scene activation once both `progress >= 0.9` and the minimum hold are
reached), 0.05 s post-load hold, 0.25 s fade-out.

---

## Per-scene UI structure

### `TitleScene.unity`

```
Bootstrapper                    MenuBootstrapper, AudioMixerVolumeApplier
EventSystem                     InputSystemUIInputModule (NOT StandaloneInputModule)
[TitleWorld]                    Posed in-engine world backdrop
  Main Camera                   Camera + TitleCameraDrift + AudioListener
  DirectionalLight              (instanced from Assets/Prefabs/DirectionalLight.prefab)
  GlobalVolume                  URP post-processing
  Ground                        Plane primitive
  Tree (×8), Stone (×2), GlowCoreLevel5
Canvas (Screen Space Overlay, 854×480, Match 0.5)
  Background                    Full-stretch 45% black dim — applies behind every panel so
                                the world doesn't disappear when switching panels
  TitleScreenPanel              CanvasGroup, contains the title + buttons
  NewGamePanel                  CanvasGroup
  SettingsPanel                 CanvasGroup
  ConfirmDialogPanel            CanvasGroup
  VersionLabel                  v{Application.version}
```

### `MainWorldScene.unity` additions (additive only — existing world untouched)

```
Bootstrapper                    GameBootstrapper, AudioMixerVolumeApplier (root)
PauseCanvas (Screen Space Overlay, sortOrder 100 over the existing InventoryCanvas)
  PauseButton                   Top-right gear; calls Pause when not already paused
  PausePanel
    DimOverlay                  Full-stretch black 0.55
    PausedLabel                 Top-right "PAUSED"
    ButtonsCard                 Centered rounded card (RoundedRect.png + RoundedRectBorder.png)
      Buttons                   RESUME / SAVE / SETTINGS / MAIN MENU (vertical layout)
  SettingsPanel                 Same rounded-card backdrop, in-game scene-local instance
  ConfirmDialogPanel            Dim overlay + dialog box + message + CONFIRM / CANCEL
```

---

## Prefabs

| Prefab | Path |
|---|---|
| Slider settings row | `Assets/Prefabs/UI/Menus/SettingRow_Slider.prefab` |
| Toggle settings row | `Assets/Prefabs/UI/Menus/SettingRow_Toggle.prefab` |
| Dropdown settings row | `Assets/Prefabs/UI/Menus/SettingRow_Dropdown.prefab` |

Each row carries its respective `ISettingControl` MonoBehaviour with internal layout
pre-wired; bootstrappers register them with `SettingsRowFactory`. Instantiation happens at
runtime when `SettingsScreen.ActivateCategory` fills the content container.

---

## Sprites

| Sprite | Use |
|---|---|
| `Assets/Sprites/UI/RoundedRect.png` | Filled rounded card backdrop, 9-sliced |
| `Assets/Sprites/UI/RoundedRectBorder.png` | Hollow rounded ring, 9-sliced |

Both are 128×128 procedural with corner radius 24 and 2-px stroke. Both use sprite borders of
28 px on each side so they slice cleanly at any size. Pulled in by Pause card and Settings
card via `Image.type = Sliced`.

---

## Settings keys

PlayerPrefs keys are namespaced `menu.<category>.<setting>`:

- `menu.audio.master` (float, default 0.8)
- `menu.audio.music` (float, default 0.8)
- `menu.audio.sfx` (float, default 0.8)
- `menu.display.fullscreen` (int 0/1)
- `menu.display.resolution` (int — index into `IDisplayService.AvailableResolutions`)
- `menu.display.mouseSensitivity` (float, default 0.31 ≈ 1.0× multiplier)

---

## Touches to existing code (minimal)

- `SaveData.Delete()` — used by `NewGameService` via `ISaveDataGateway`.
- `WorldGrid.Start()` — reads `IGameLaunchContext`. `NewGame` mode skips load + seeds world
  name, `Continue` mode runs the existing load path. Falls back to the singleton context when
  no bootstrapper-set context is present.
- `WorldGrid.LoadSaveData()` — captures `Player.Name` for re-emission on save.
- `WorldGrid.BuildSaveData()` — reads the captured name.
- `WorldGrid.SetLaunchContext(IGameLaunchContext)` — bootstrapper-side hook.
- `PlayerCamera` — added `Initialize(IDisplayService)` and a sensitivity multiplier scaling
  the existing `m_cameraSpeed`. Default behaviour unchanged when nothing wires it.
- `PlayerInventory.RequestCloseUI()` (new) — closes inventory in place + raises
  `OnCloseUIRequested`. `OnInventory` input callback now bails when `Time.timeScale == 0`
  to block the inventory key while paused.
- `IInventoryService.RequestCloseUI()` — added to the interface.
- `NodeActionSystem.UpdateOutlineHover()` — clears hover when `Time.timeScale == 0`.
- `Assets/InputSystem_Actions.inputactions` — Esc binding removed from the legacy `CloseUI`
  action so the escape router is the sole Esc handler. The `CloseUI` action itself remains
  for any future binding that wants to invoke `RequestCloseUI` directly.

---

## Testing

NUnit under `Assets/Scripts/Tests/UnitTests/Menus/`:

- `MenuManagerTests` — open/back/close, screen change events, multi-arg open, history stack.
- `SettingsRepositoryTests` — round-trip via `FakePlayerPrefsBackend`.
- `NewGameServiceTests` — uses `FakeSaveDataGateway`, `FakeGameLaunchContext`,
  `FakeSceneTransition`; asserts `Delete` called, launch mode set, scene loaded.
- `AudioBridgeTests` — events fire on Raise calls; subscribers receive correct values;
  unsubscribed handlers don't fire.
- `DescriptorRoundTripTests` — descriptor `Read`/`Write` closures move data through the
  repository and trigger side-effects like the audio bridge.

Fakes mirror the existing `MockSaveService` placement at `Assets/Scripts/Tests/Mocks/Menus/`
(in the `Gameplay` assembly so the test assembly can reach them through its Gameplay
reference).

---

## Build / lint

- `dotnet format Tools/Lint/GlowCore.Lint.csproj --verify-no-changes --severity error` —
  scoped to the menu folders, passes.
- All new code follows `CODING-GUIDELINES.md`: Allman braces, `m_` private fields,
  `[SerializeField] private` for inspector exposure, explicit access modifiers, `k`-prefixed
  constants, `s_`-prefixed statics.

---

## Things to revisit

- **Title scene lighting** — the freshly-created TitleScene needs its scene-level lighting
  data baked / configured. The `[TitleWorld]` is in place but the user finalised lighting
  manually in editor; future rebuilds should preserve those settings.
- **AudioMixer asset** — `AudioMixerVolumeApplier` is wired but no mixer asset is assigned by
  default. Audio sliders save and fire events; volume only changes once a mixer is dropped on
  the bootstrapper's `m_audioApplier` field with the three exposed parameters
  (`MasterVolume`, `MusicVolume`, `SfxVolume`).
- **Fakes location** — test fakes currently live in the Gameplay assembly (matching
  `MockSaveService`). If the team later splits a TestSupport assembly, the fakes should move
  with it.
