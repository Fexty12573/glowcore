# Inventory System Architecture

> Reference document for the GlowCore inventory UI system.
> Last updated: 2026-04-17 (IGlowCoreObject interface introduced; GlowCoreObject: dispatch-by-level, config-driven expansion via ExpandIfConfigured, Upgrade() separated from FeedMaterial, auto-close moved from UI to GlowCoreObject.Update(); Fire: VFX-only; GlowCoreLevelConfig: ExpansionItem/ExpansionCostPerTile/TileCount/InitialActiveLogs; GlowCoreUpgradeUI: targets IGlowCoreObject, upgrade button, inventory dragging enabled; FeedMaterialRowUI: IGlowCoreObject, Add All button, accumulated+pending/required label format; WorldGrid: multi-tile PlaceNodeAt overload)

---

## Overview

The inventory system has three layers: a **data layer** (pure C#, no UI), a **crafting layer**
(pure C#, no UI), and a **UI layer** (uGUI). All layers are decoupled through interfaces
(`IInventoryService`, `ICraftingService`, `IGlowCoreObject`) — UI logic depends only on
these interfaces, never on concrete MonoBehaviour types.

```
┌─────────────────────────────────────────────────────────────┐
│                        DATA LAYER                           │
│                                                             │
│  Item (SO) ←── ItemStack (class) ←── Inventory (C#)        │
│                                         ↑                   │
│                                  PlayerInventory (MB)       │
│                                   implements                │
│                                  IInventoryService          │
│                                         ↓                   │
│                                   PlayerHand (MB)           │
│                                                             │
│  GlowCoreLevelConfig (SO) ──► GlowCoreObject (MB)          │
│                                   implements                │
│                               IGlowCoreObject               │
└───────────────────────┬─────────────────────────────────────┘
                        │
    IInventoryService + ICraftingService + IGlowCoreObject
                        │
┌───────────────────────▼─────────────────────────────────────┐
│                     CRAFTING LAYER                           │
│                                                             │
│  CraftingSystem (plain C#) ── implements ICraftingService   │
│  Owns recipe validation + execution logic                   │
│  Receives IInventoryService in constructor                  │
└───────────────────────┬─────────────────────────────────────┘
                        │
     IInventoryService + ICraftingService + IGlowCoreObject
                        │
┌───────────────────────▼─────────────────────────────────────┐
│                        UI LAYER                             │
│                                                             │
│  InventoryUI ──► ItemSlotUI (×32)    (uses IInventoryService)│
│                  TooltipUI                                   │
│  CraftingUI ──► RecipeRowUI (×N)    (uses ICraftingService) │
│  HotbarUI ────► HotbarSlotUI (×8)   (uses IInventoryService)│
│  GlowCoreUpgradeUI ──► FeedMaterialRowUI (uses IGlowCoreObject)│
│                  ItemIconHelper (static)                     │
│                  UIColors (static)                           │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## Interfaces

### `IInventoryService` — (`Assets/Scripts/IInventoryService.cs`)

The single contract between the data layer and all consumers (UI, crafting, etc.).

| Member | Kind | Purpose |
|---|---|---|
| `SlotCount` | Property | Total slots (32) |
| `HotbarSlotCount` | Property | Hotbar size (8) |
| `SelectedHotbarIndex` | Property | Currently selected hotbar slot |
| `IsOpen` | Property | Whether inventory panel is open |
| `IsCraftingTableOpen` | Property | Whether the crafting table panel is open |
| `IsGlowCoreUIOpen` | Property | Whether the GlowCore upgrade panel is open |
| `GetSlotData(int)` | Query | Returns `SlotData` snapshot for a flat index |
| `CountItem(Item)` | Query | Total count of an item across all slots |
| `CanAcceptItem(Item, int)` | Query | Can inventory hold this many of item? |
| `IsHotbarSlot(int)` | Query | Is this flat index a hotbar slot? |
| `Swap(int, int)` | Command | Swap two slots |
| `TryMerge(int, int)` | Command | Merge src into dst if same item |
| `DropItem(int, Vector3)` | Command | Drop slot contents to world |
| `SelectHotbarSlot(int)` | Command | Set active hotbar slot |
| `ToggleInventory()` | Command | Toggle panel open/close |
| `SetInventoryOpen(bool)` | Command | Set panel state explicitly |
| `SetCraftingTableOpen(bool)` | Command | Set crafting table open state, fires `OnCraftingTableToggled` |
| `SetGlowCoreUIOpen(bool)` | Command | Set GlowCore UI open state, fires `OnGlowCoreUIToggled` |
| `RemoveItems(Item, int)` | Command | Remove items across slots, returns amount removed |
| `AddItem(Item, int)` | Command | Add items to inventory |
| `OnSlotChanged` | Event | `Action<SlotChangedEvent>` — fires with slot index + data snapshot |
| `OnHotbarSelectionChanged` | Event | `Action<int>` — fires with new hotbar index |
| `OnInventoryToggled` | Event | `Action<bool>` — fires with open state |
| `OnCraftingToggled` | Event | `Action` — fires when C key pressed while inventory is open |
| `OnCraftingTableToggled` | Event | `Action<bool>` — fires when crafting table is opened/closed |
| `OnGlowCoreUIToggled` | Event | `Action<bool>` — fires when the GlowCore upgrade panel is opened/closed |
| `OnCloseUIRequested` | Event | `Action` — fires on Escape; closes any open UI (inventory, crafting table, or GlowCore UI) |

### `ICraftingService` — (`Assets/Scripts/ICraftingService.cs`)

The contract between the crafting layer and the UI.

| Member | Kind | Purpose |
|---|---|---|
| `Recipes` | Property | `IReadOnlyList<Recipe>` of all recipes |
| `CanCraft(Recipe)` | Query | Are ingredients available + space for result? |
| `GetItemCount(Item)` | Query | Convenience: count of item in inventory |
| `Craft(Recipe)` | Command | Execute crafting (remove ingredients, add result) |
| `OnRecipesRefreshed` | Event | `Action` — fires when craftability may have changed |

### `IGlowCoreObject` — (`Assets/Scripts/IGlowCoreObject.cs`)

The contract between `GlowCoreObject` (data layer) and all UI consumers. UI never depends on the concrete `GlowCoreObject` class.

| Member | Kind | Purpose |
|---|---|---|
| `LevelConfig` | Property | Current level's `GlowCoreLevelConfig` SO |
| `NextLevelConfig` | Property | Config of `m_nextLevelPrefab` (null if max level) |
| `Level` | Property | Current level number |
| `TotalProgress01` | Property | Normalized `0..1` progress across all required materials |
| `IsReadyToUpgrade` | Property | True when all required materials are fully accumulated |
| `HasNextLevel` | Property | True when `m_nextLevelPrefab` is not null (i.e. not max level) |
| `AccumulatedFor(Item)` | Query | How much of an item has been fed so far |
| `FeedMaterial(Item, int)` | Command | Clamps to available/needed; removes from inventory; fires events |
| `Upgrade()` | Command | Fires `OnLevelUp`, calls `UpgradePhysical()` + `SpawnNextLevel()`. Only call when `IsReadyToUpgrade` |
| `OnProgressChanged` | Event | `Action` — fires after any successful feed |
| `OnLevelUp` | Event | `Action` — fires when `Upgrade()` is called, before the prefab is swapped |

### `SlotData` — (`Assets/Scripts/SlotData.cs`)

Immutable value type snapshot of a slot. UI never holds references to live `ItemStack` objects.

| Field | Type | Purpose |
|---|---|---|
| `Item` | `Item` | The item type (null if empty) |
| `Amount` | `int` | Stack count |
| `IsValid` | `bool` | True if Item != null and Amount > 0 |
| `Icon` | `Sprite` | Convenience: Item?.Icon |

### `SlotChangedEvent` — (`Assets/Scripts/SlotData.cs`)

Rich event payload so listeners don't need to query state after receiving the event.

| Field | Type | Purpose |
|---|---|---|
| `SlotIndex` | `int` | Which slot changed |
| `Data` | `SlotData` | New state of that slot |

---

## Data Layer

### `Item` — ScriptableObject (`Assets/Scripts/ScriptableObjects/Item.cs`)

The definition of an item type. Created as `.asset` files in the Unity Editor.

| Field | Type | Purpose |
|---|---|---|
| `Name` | string | Display name shown in tooltip |
| `Description` | string | Tooltip description (TextArea, can be empty) |
| `Prefab` | GameObject | 3D model used for world drops and in-hand visual |
| `Icon` | Sprite | Icon used in UI slots and drag cursor |
| `DropScale` | float | Scale multiplier for world-drop visuals |
| `InHandScale` | float | Scale multiplier when item is held in hand |
| `MaxStack` | int | Maximum items per stack (default 99, min 0) |

### `ItemStack` — Serializable class (`Assets/Scripts/ScriptableObjects/ItemStack.cs`)

A runtime instance representing "X amount of item Y" in a single slot.

| Member | Purpose |
|---|---|
| `Item` | Which item type this stack holds (null = empty slot) |
| `Amount` | How many |
| `IsValid` | True if Item != null and Amount != 0 |
| `IsFull` | True if Amount >= MaxStack |
| `Add(int)` | Add amount, clamped to MaxStack. Returns amount added |
| `Add(ItemStack)` | Merge another stack of the same item into this one |
| `Set(ItemStack)` | Copy another stack's data, zeroing the source's amount |
| `Set(Item, int)` | Directly set item and amount |
| `Clear()` | Set to null/0 |

### `Recipe` — ScriptableObject (`Assets/Scripts/ScriptableObjects/Recipe.cs`)

Defines a crafting recipe.

| Field | Type | Purpose |
|---|---|---|
| `Name` | string | Display name shown in the recipe row |
| `Ingredients` | Ingredient[] | Array of required items and amounts |
| `ResultItem` | Item | The item produced by crafting |
| `ResultAmount` | int | How many of the result item (min 1) |
| `RequiresCraftingTable` | bool | If true, recipe is excluded from hand-crafting (`CraftingUI` filters it out) |

**`Ingredient`** (nested serializable struct): `Item` + `Amount` (min 1).

### `Inventory` — Serializable class (`Assets/Scripts/Inventory.cs`)

A flat array of `ItemStack` slots organized as a 2D grid (width × height).
This is **not** a MonoBehaviour — it's a plain C# class owned by `PlayerInventory`.

| Member | Purpose |
|---|---|
| `Width`, `Height`, `Size` | Grid dimensions and total slot count |
| `OnSlotChanged` (event) | `Action<int>` — fires with the flat index (internal, wrapped by PlayerInventory) |
| `GetSlot(int)` | Access by flat index |
| `AddItems(stack, emptySlotStart)` | Stack onto existing, then first empty slot |
| `Swap(a, b)` | Swap two slots |
| `TryMerge(src, dst)` | Merge if same item type |
| `CountItem(Item)` | Sum total amount across all slots |
| `RemoveItems(Item, int)` | Remove items, fires OnSlotChanged per slot |
| `CanAccept(Item, int)` | Check if space exists |
| `ClearSlot(int)` | Clears slot data and fires `OnSlotChanged` |

### `PlayerInventory` — MonoBehaviour, implements `IInventoryService` (`Assets/Scripts/PlayerInventory.cs`)

Lives on the Player GameObject. Owns the `Inventory` instance and delegates to it for all
`IInventoryService` operations. Handles input via Input System action callbacks.

**Input Actions (event-driven, no Update polling):**
- `OnInventory(InputValue)` → Tab key → `ToggleInventory()`
- `OnCrafting(InputValue)` → C key → fires `OnCraftingToggled` (only when inventory is open)
- `OnCloseUI(InputValue)` → Escape key → closes inventory if open; always fires `OnCloseUIRequested` (used by `CraftingTableInteractable` to close the bench)
- `OnHotbarSlot1..8(InputValue)` → keys 1-8 → `SelectHotbarSlot()`
- `OnNext(InputValue)` / `OnPrevious(InputValue)` → scroll wheel navigation

**Inspector fields:** `m_playerHand` — reference to the `PlayerHand` component.

**Extra public methods (not on `IInventoryService`):**
- `ConsumeHandItem(int amount)` — removes `amount` items from the currently selected hotbar slot. Used by `BlockBehaviour` after a successful block placement.
- `SetCraftingTableOpen(bool)` — sets `m_isCraftingTableOpen` and fires `OnCraftingTableToggled`. Called by `CraftingTableInteractable`.

### `IHandItem` — Interface (`Assets/Scripts/IHandItem.cs`)

Contract for components attached to hand item prefabs that respond to the Use action (left click).

| Member | Purpose |
|---|---|
| `Use(InputValue)` | Called by `PlayerHand.OnUse` when the player uses the held item |

**Note:** Components implementing `IHandItem` are stored **disabled** (`m_Enabled: 0`) in their prefab. `PlayerHand.UpdateHandVisual()` explicitly enables the MonoBehaviour after instantiation so Unity lifecycle (`OnEnable`, `Start`) runs correctly.

### `IPlayerInventoryAware` — Interface (`Assets/Scripts/IPlayerInventoryAware.cs`)

Optional contract for hand item components that need access to `PlayerInventory` (e.g. to consume the item after use).

| Member | Purpose |
|---|---|
| `SetInventory(PlayerInventory)` | Called by `PlayerHand.UpdateHandVisual()` after instantiating the hand item |

### `PlayerHand` — MonoBehaviour (`Assets/Scripts/PlayerHand.cs`)

Manages the item physically held in the player's hand. Singleton (`s_instance`).

**Inspector field:** `m_playerInventory` — must be wired to the `PlayerInventory` component on the Player GameObject.

**`UpdateHandVisual()` sequence:**
1. Destroys the previous hand item GameObject
2. Instantiates `Item.Prefab` as a child of the hand transform
3. Removes `Rigidbody` and `Outline` components
4. If the root has an `IHandItem` MonoBehaviour: **enables it** (prefabs store it disabled)
5. If the root implements `IPlayerInventoryAware`: calls `SetInventory(m_playerInventory)`

### `ItemStackDrop` — MonoBehaviour (`Assets/Scripts/ItemStackDrop.cs`)

Represents a dropped `ItemStack` in the world. Called by `PlayerInventory.DropItem()`.

### `GlowCoreLevelConfig` — ScriptableObject (`Assets/Scripts/ScriptableObjects/GlowCoreLevelConfig.cs`)

Per-level data for a `GlowCoreObject` prefab variant. Referenced by each GlowCore level prefab via a serialized field. Reuses `Recipe.Ingredient` for the required-materials list.

| Field | Type | Purpose |
|---|---|---|
| `LevelName` | string | Display name for the level |
| `Level` | int | Level number (1, 2, 3, ...) |
| `LevelIcon` | Sprite | Icon shown in the upgrade UI header |
| `RequiredMaterials` | `IReadOnlyList<Recipe.Ingredient>` | Items + amounts needed to reach the **next** level |
| `TilesOnLevelUp` | int | Tiles the world grid expands by when this level's upgrade completes |
| `TileCount` | int | Tiles this GlowCore occupies in the world grid (1=1×1, 4=2×2, 9=3×3, 16=4×4) |
| `ExpansionItem` | Item | Which item triggers incremental map expansion when fed. Leave null to disable. |
| `ExpansionCostPerTile` | int | How many of `ExpansionItem` are needed to grow the map by one tile |
| `InitialActiveLogs` | int | Level 1 only — how many log GameObjects are active when the GlowCore first spawns |
| `FormatPerk()` | method | Returns the perk description template with `{tiles}` substituted |

### `GlowCoreObject` — MonoBehaviour, `IInteractable`, `IGlowCoreObject` (`Assets/Scripts/GlowCore.cs`)

The world-placed GlowCore node. Implements `IGlowCoreObject`. Consumes materials toward a `GlowCoreLevelConfig` and spawns the next-level prefab once the player manually triggers an upgrade.

**Inspector fields:**
- `m_levelConfig` — the `GlowCoreLevelConfig` SO for this prefab's level
- `m_nextLevelPrefab` — the next-level GlowCore prefab (null = max level)
- `m_logs[]` — Level 1 log GameObjects activated incrementally as Wood is fed
- `m_fire` — optional Fire component; when `Wood` is fed, `Fire.FeedWood(amount)` is called for VFX
- `m_closeDistance` — auto-close distance in XZ (default 5f)

**Runtime:** `m_playerInventory` is found via `FindFirstObjectByType<PlayerInventory>()` in `Awake()` — **not a serialized field**. This makes every GlowCore level prefab self-contained.

**Key API:**
| Member | Purpose |
|---|---|
| `LevelConfig` | Current level's config SO |
| `NextLevelConfig` | Config on `m_nextLevelPrefab` (null if no next level) |
| `Level`, `HasNextLevel`, `IsReadyToUpgrade` | Convenience properties (all on `IGlowCoreObject`) |
| `TotalProgress01` | Normalized `0..1` progress across all required materials |
| `AccumulatedFor(Item)` | How much of an item has been fed so far |
| `RequiredFor(Item)` | Target amount for an item at this level |
| `FeedMaterial(Item, int)` | Clamps to `min(amount, stillNeeded, available)`; removes from inventory; stores accumulated; calls `ExpandIfConfigured`; fires `OnProgressChanged`. Does **not** auto-upgrade. |
| `Upgrade()` | Fires `OnLevelUp`, calls `UpgradePhysical()`, then `SpawnNextLevel()`. Only valid when `IsReadyToUpgrade`. |
| `Interact()` | Caches `GlowCoreUpgradeUI` via `FindFirstObjectByType`; calls `m_ui.Show(this)` |
| `ActivateLogs(int)` | Activates the next N log GameObjects (Level 1 visual) |
| `OnProgressChanged` | Event — fires after any successful feed |
| `OnLevelUp` | Event — fires when `Upgrade()` is called, before the prefab is swapped |

**Level dispatch pattern:** `CreatePhysical(int level)` and `FeedPhysical(int level, Item, int)` switch on the level integer and delegate to private `*Level1`, `*Level2`, … helpers. Adding behavior for a new level requires only a new `case` and a new private method — no inheritance, no extra components.

**Incremental expansion:** `ExpandIfConfigured(Item, int)` checks whether the fed item matches `m_levelConfig.ExpansionItem`. If so, it accumulates a running bank and calls `WorldGrid.Instance.Expand(1)` for each full `ExpansionCostPerTile` threshold crossed. Fully config-driven per level.

**Level-up flow:** `Upgrade()` runs:
1. Fires `OnLevelUp` (UI hides).
2. `UpgradePhysical()` → `WorldGrid.Instance.Expand(m_levelConfig.TilesOnLevelUp, isLevelUp: true)`.
3. `SpawnNextLevel()` → clears all tiles in `oldNode.TilesUsed`, instantiates `m_nextLevelPrefab` as a sibling under `transform.parent`, reads `nextConfig.TileCount`, calls `WorldGrid.Instance.PlaceNodeAt(newNode, x, z, tileCount)`, then `Destroy(gameObject)`.

**Auto-close:** `Update()` runs while `m_ui.IsVisible`; if the player drifts beyond `m_closeDistance` on the XZ plane, calls `m_ui.Hide()`. This mirrors the `CraftingTableInteractable.Update()` pattern and keeps the UI clean of distance logic.

### `Fire` — MonoBehaviour (`Assets/Scripts/Fire.cs`)

Handles **fire VFX scaling only**. `FeedWood(int amount)` is called from `GlowCoreObject.FeedPhysicalLevel1` when `Wood` is fed — it updates `TotalWoodReceived` and scales the fire particle system. **All world expansion logic has been removed from Fire.** Incremental tile expansion is handled by `GlowCoreObject.ExpandIfConfigured`; level-up expansion is handled by `GlowCoreObject.UpgradePhysical`.

### `WorldGrid` — MonoBehaviour (`Assets/Scripts/WorldGrid.cs`)

Manages the world tile grid. Key method relevant to GlowCore:

- `PlaceNodeAt(Node node, int x, int z, int tileCount)` — computes a centered square region of side `√tileCount` and registers all tiles to the node. Used by `SpawnNextLevel()` so multi-tile GlowCore variants (e.g. Level 2 with `TileCount = 4` occupying 2×2) are registered correctly.
- `ClearNodeAt(Vector2Int tile)` — used by `SpawnNextLevel()` to free all tiles in `oldNode.TilesUsed` before spawning the replacement.

---

## Crafting Layer

### `CraftingSystem` — Plain C# class, implements `ICraftingService` (`Assets/Scripts/CraftingSystem.cs`)

Pure logic class. No MonoBehaviour, no UI. Created independently by both `CraftingUI` and `CraftingTableUI` in their respective `Start()` methods — each instance owns its own filtered recipe slice.

| Member | Purpose |
|---|---|
| Constructor | Receives `IInventoryService` + `Recipe[]` |
| `CanCraft(Recipe)` | Checks ingredient counts + space for result |
| `Craft(Recipe)` | Removes ingredients via `IInventoryService.RemoveItems`, adds result via `AddItem` |
| `GetItemCount(Item)` | Delegates to `IInventoryService.CountItem` |
| `OnRecipesRefreshed` | Fires when any `OnSlotChanged` event occurs |
| `Dispose()` | Unsubscribes from events |

---

## UI Layer

All UI scripts live in `Assets/Scripts/UI/Inventory/` under the `GlowCore.UI.Inventory` namespace.

### `InventoryUI` — MonoBehaviour (`InventoryUI.cs`)

**The central controller.** Manages the inventory panel: builds the slot grid, handles
drag-and-drop, tracks hover state, and controls the cursor icon.

**Key change:** Uses `IInventoryService` (assigned from `[SerializeField] PlayerInventory`).
Never accesses `Inventory` or `PlayerInventory` directly for data operations.

**Pointer flow:**
- `ItemSlotUI` owns drag lifecycle (`OnBeginDrag`, `OnDrag`, `OnEndDrag`)
- `InventoryUI` receives forwarded calls (`OnDragUpdate`, `OnSlotReleased`)

**Drag-and-drop flow:**
1. `OnSlotPressed(index)` → called by ItemSlotUI on pointer down
2. `PickUpItem(index, SlotData)` → ghosts source slot, shows cursor icon
3. `ItemSlotUI.OnDrag` → `InventoryUI.OnDragUpdate` updates cursor and tooltip position
4. `ItemSlotUI.OnEndDrag` → `InventoryUI.OnSlotReleased` → `DropHeldItem()`
5. Merge/Swap/Drop via `m_service.TryMerge()`, `m_service.Swap()`, `m_service.DropItem()`
6. `CancelHeldItem()` → cleanup

### `ItemSlotUI` — MonoBehaviour (`ItemSlotUI.cs`)

**Reusable slot component.** Receives `SlotData` via `Refresh(SlotData)` — never queries
the data layer. Initialized with `Initialize(InventoryUI owner, int slotIndex, SlotData)`.

### `CraftingUI` — MonoBehaviour (`CraftingUI.cs`)

**Crafting panel controller.** Creates a `CraftingSystem` in `Start()` and passes it to rows.

- Reads recipes from the global `RecipeList` asset, **filtering to `RequiresCraftingTable == false`** (hand-craftable only)
- Opens via `OnCraftingToggled` event from `IInventoryService` (C key, only when inventory is open)
- Closes when inventory closes (`OnInventoryToggled`)
- No `Update()` — fully event-driven
- Subscribes to `ICraftingService.OnRecipesRefreshed` for auto-refresh

### `CraftingTableUI` — MonoBehaviour (`CraftingTableUI.cs`)

**Crafting bench panel controller.** Opens when the player interacts with a crafting table node (E key).
Combines a recipe list (left) with a read-only inventory display (right) in a single panel.

- Reads **all** recipes from the global `RecipeList` asset — **no filtering** (the global list is expected to contain all recipes; table-only recipes are naturally only accessible here)
- Creates its own `CraftingSystem` with the full recipe list
- Right-side inventory display: 32 `ItemSlotUI` instances with `null` owner (read-only, no drag/click)
- Inventory display stays in sync via `OnSlotChanged` (always subscribed, cheap no-op when hidden)
- Opened/closed by `CraftingTableInteractable` via `SetVisible(bool)` — no direct key binding
- Also auto-closes when: player walks out of range, inventory opens (Tab), or Escape is pressed

### `RecipeRowUI` — MonoBehaviour (`RecipeRowUI.cs`)

**Single recipe row.** Initialized with `ICraftingService` — calls `CanCraft()`, `GetItemCount()`,
and `Craft()` through the interface. No direct inventory access.

### `HotbarUI` — MonoBehaviour (`HotbarUI.cs`)

**Always-visible hotbar.** Uses `IInventoryService` for data and events.
No `Update()` — number key input is handled by `PlayerInventory` via Input Actions.
Dims (alpha 0.35, non-interactive) when the inventory, crafting table, **or GlowCore upgrade panel** is open — subscribes to `OnInventoryToggled`, `OnCraftingTableToggled`, and `OnGlowCoreUIToggled`.

### `HotbarSlotUI` — MonoBehaviour (`HotbarSlotUI.cs`)

**Single standalone hotbar slot.** Initialized with `IInventoryService`.
Receives `SlotData` via `Refresh(SlotData)`.

### `TooltipUI` — MonoBehaviour (`TooltipUI.cs`)

**Mouse-following tooltip.** No `Update()` — position is updated by
`InventoryUI.OnDragUpdate` calling `TooltipUI.UpdatePosition()`.

### `ItemIconHelper` — Static class (`ItemIconHelper.cs`)

Thin helper for applying item icons to uGUI `Image` components. Since `Item.Icon` is now a `Sprite`, no conversion or caching is needed — `GetSprite(item)` returns `item?.Icon` directly.

### `UIColors` — Static class (`Assets/Scripts/UI/UIColors.cs`)

All color tokens. Never hardcode colors.

---

## GlowCore Upgrade UI

Scripts live in `Assets/Scripts/UI/GlowCore/` under the `GlowCore.UI.Upgrade` namespace. The layout mirrors `CraftingTableUI`: feed-material panel on the left, read-only inventory on the right.

### `GlowCoreUpgradeUI` — MonoBehaviour (`GlowCoreUpgradeUI.cs`)

Central controller. Opens via `GlowCoreObject.Interact()` → `Show(IGlowCoreObject target)`. Depends only on `IGlowCoreObject` — never on the concrete `GlowCoreObject`.

**Inspector fields:** `m_panelCanvasGroup`, `m_backdropCanvasGroup`, `m_closeButton`, `m_currentLevelIcon` + `m_nextLevelIcon` + `m_levelText`, `m_progressBar` (`ProgressBarUI`), `m_perkInfo` (TMP), `m_feedRowContentParent` + `m_feedRowPrefab`, `m_feedButton` + `m_feedButtonLabel` (TMP), `m_inventoryUpperGridParent` + `m_inventoryHotbarRowParent` + `m_slotPrefab`.

**No `m_closeDistance` or `m_player` field.** Auto-close lives in `GlowCoreObject.Update()`, not here. This UI has **no `Update()` method** — fully event-driven.

**Lifecycle & subscriptions:**
- `Start()` resolves `PlayerInventory` + `InventoryUI`, builds the right-side inventory (32 `ItemSlotUI` with **`m_inventoryUI` as owner** — drag is enabled, matching the behavior of the standard inventory panel), subscribes to `OnSlotChanged`, `OnInventoryToggled`, and `OnCloseUIRequested`.
- `Show(IGlowCoreObject)` stores the target, subscribes to `target.OnProgressChanged` and `target.OnLevelUp`, spawns `FeedMaterialRowUI` rows from `target.LevelConfig.RequiredMaterials`, calls `m_inventoryService.SetGlowCoreUIOpen(true)`, shows the panel.
- `SetVisible(false)`/`Hide()` unsubscribes from the target, clears target, cancels any held drag via `InventoryUI.CancelHeldItem()`, calls `SetGlowCoreUIOpen(false)`.

**Feed/Upgrade flow:** `OnFeedButtonClicked` checks `m_target.IsReadyToUpgrade` first:
- If `true` → calls `m_target.Upgrade()` directly (no row iteration needed).
- If `false` → iterates rows, calls `target.FeedMaterial(row.Material, row.SelectedAmount)` for each row with a positive stepper value, then resets all steppers to 0.

**Button state:** evaluated on every `OnSelectionChanged` and on `OnTargetProgressChanged`:
- When `IsReadyToUpgrade`: button label = `"UPGRADE TO LEVEL {next.Level}"`, always `interactable = true`.
- Otherwise: button label = `"FEED MATERIALS"`, `interactable` = any row has `SelectedAmount > 0`.

**Header:** `m_nextLevelIcon.color = IsReadyToUpgrade ? Color.white : UIColors.WoodBorderLight`. Level text format: `"3 => 4"`.

**Level-up:** on `target.OnLevelUp`, the UI hides. `GlowCoreObject.SpawnNextLevel` then destroys the old object and instantiates the replacement.

### `FeedMaterialRowUI` — MonoBehaviour (`FeedMaterialRowUI.cs`)

Single row: icon, name, count label, stepper, and an "Add All" button.

- `Initialize(IInventoryService, IGlowCoreObject target, Item, int required)` — takes `IGlowCoreObject`, not the concrete class. Wires icon via `ItemIconHelper.GetSprite`, binds the stepper to `[0, min(stillNeeded, have)]`.
- `Refresh()` — calls `SetBounds(0, GetMaxSelectable())` first, **then** reads `m_stepper.Value` as `pending` (avoids stale-read after the stepper clamps). Count label format: `"{accumulated + pending}/{required}"`. Color: `UIColors.Green` when `accumulated + pending >= required`, otherwise `UIColors.MissingMat`. Border turns green when `accumulated >= required` (fully met in the data layer, not just preview).
- `ResetSelection()` — zeroes the stepper (called after a feed).
- `OnAddAllClicked()` — calls `m_stepper.SetValue(GetMaxSelectable())` to pre-fill the maximum feedable amount.
- Exposes `Material`, `SelectedAmount`, and `OnSelectionChanged` for the controller.

### `AmountStepperUI` — MonoBehaviour (`AmountStepperUI.cs`)

Reusable `[−] value [+]` widget. `Bind(min, max, initial)` seeds it; `SetBounds(min, max)` can be called at runtime to clamp. Disables the `−` button at min and the `+` button at max. Fires `OnValueChanged(int)` on user clicks (not on programmatic `SetValue(..., fireEvent: false)`).

### `ProgressBarUI` — MonoBehaviour (`ProgressBarUI.cs`)

Thin wrapper around `Image.fillAmount` + percent `TextMeshProUGUI`. Single method `SetProgress(float)` clamps to `[0,1]` and updates the label to `"{0}%"`.

---

## Unity Hierarchy

```
InventoryCanvas (Canvas — Screen Space Overlay, CanvasScaler 854×480)
  │
  ├── InventoryPanel (CanvasGroup + VerticalLayoutGroup)
  │     ├── UpperGrid (GridLayoutGroup: 40×40 cells, 4px spacing, 4 columns)
  │     │     └── [24 ItemSlotUI spawned at runtime — indices 8–31]
  │     │
  │     ├── Spacer (empty RectTransform, ~12px height)
  │     │
  │     └── HotbarRow (HorizontalLayoutGroup: 4px spacing)
  │           └── [8 ItemSlotUI spawned at runtime, hotbar-styled — indices 0–7]
  │
  ├── CraftTabButton (Button — onClick → CraftingUI.Toggle())
  │
  ├── CraftingPanel (CraftingUI + CanvasGroup)
  │     ├── Title (TMP — "CRAFTING")
  │     └── ScrollView → Viewport → Content
  │           └── [RecipeRowUI instances spawned at runtime]
  │
  ├── HotbarPanel (CanvasGroup, anchored bottom-center)
  │     └── HotbarGrid (HorizontalLayoutGroup)
  │           └── [8 HotbarSlotUI spawned at runtime]
  │
  ├── CraftingBenchPanel (CraftingTableUI + CanvasGroup — starts hidden)
  │     ├── RecipeSection (VerticalLayoutGroup, 245px wide)
  │     │     ├── TitleLabel (TMP — "CRAFTING BENCH", Cinzel Bold 11px, AccentDim)
  │     │     ├── SubtitleLabel (TMP — "Advanced Recipes", Nunito 9px, WhiteFaint)
  │     │     └── RecipeScrollView → Viewport → Content
  │     │           └── [RecipeRowUI instances spawned at runtime — bench recipes only]
  │     └── InventorySection (VerticalLayoutGroup, 210px wide)
  │           ├── TitleLabel (TMP — "INVENTORY", Cinzel Bold 11px, AccentDim)
  │           └── InventoryGrid (GridLayoutGroup: 36×36 cells, 3px spacing, 4 columns)
  │                 └── [32 ItemSlotUI spawned at runtime — read-only, null owner]
  │
  ├── GlowCoreUpgradePanel (GlowCoreUpgradeUI + CanvasGroup — starts hidden)
  │     ├── LeftSection (VerticalLayoutGroup, ~245px wide)
  │     │     ├── TitleLabel (TMP — "GLOWCORE", Cinzel Bold 11px, AccentDim)
  │     │     ├── LevelHeader (HorizontalLayoutGroup)
  │     │     │     ├── CurrentLevelIcon (Image)
  │     │     │     ├── LevelText (TMP — e.g. "3 ⇒ 4")
  │     │     │     └── NextLevelIcon (Image, tinted WoodBorderLight until reached)
  │     │     ├── ProgressBar (ProgressBarUI — Image.fillAmount + % label)
  │     │     ├── PerkInfo (TMP — e.g. "Next level expands the glow radius by +15 tiles")
  │     │     ├── FeedMaterialsLabel (TMP — "FEED THE GLOWCORE")
  │     │     ├── FeedScrollView → Viewport → Content (VerticalLayoutGroup)
  │     │     │     └── [FeedMaterialRowUI instances spawned at runtime]
  │     │     └── FeedMaterialsButton (Button + TMP label — "FEED MATERIALS" or "UPGRADE TO LEVEL N"; disabled when no stepper > 0 and not ready to upgrade)
  │     └── RightSection (VerticalLayoutGroup, 210px wide)
  │           ├── TitleLabel (TMP — "INVENTORY", Cinzel Bold 11px, AccentDim)
  │           └── InventoryGrid (GridLayoutGroup)
  │                 └── [32 ItemSlotUI spawned at runtime — read-only, null owner]
  │
  ├── CursorIcon (RawImage — 40×40, raycast OFF)
  │
  └── Tooltip (CanvasGroup — raycast OFF, interactable OFF)
        ├── TooltipName (TMP)
        └── TooltipDesc (TMP)
```

---

## Data Flow

### Adding an item (pickup)
```
ItemStackDrop.OnTriggerEnter()
  → PlayerInventory.Add(stack)
      → Inventory.AddItems(stack, HotbarStartIndex=0)
        → fires Inventory.OnSlotChanged(flatIndex)
          → PlayerInventory wraps as SlotChangedEvent(index, SlotData)
            → IInventoryService.OnSlotChanged fires
              → InventoryUI receives event → ItemSlotUI.Refresh(data)
              → HotbarUI receives event → HotbarSlotUI.Refresh(data)
              → CraftingSystem receives event → ICraftingService.OnRecipesRefreshed fires
                → CraftingUI refreshes rows
```

### Drag-and-drop
```
ItemSlotUI.OnPointerDown → InventoryUI.OnSlotPressed(index)
  → PickUpItem(index, SlotData): ghost slot, show cursor

ItemSlotUI.OnDrag → InventoryUI.OnDragUpdate → UpdateCursorPosition()

InventoryUI.OnPointerUp → DropHeldItem()
  → same item type: m_service.TryMerge(src, dst)
  → different item: m_service.Swap(src, dst)
  → no target slot: m_service.DropItem(index, position)
  → CancelHeldItem(): unghost, hide cursor
```

### Crafting (hand — C key)
```
RecipeRowUI click → ICraftingService.Craft(recipe)
  → CraftingSystem.Craft(recipe)
    → IInventoryService.RemoveItems() per ingredient
    → IInventoryService.AddItem() for result
    → OnSlotChanged fires per affected slot → UI auto-refreshes
```

### Crafting table interaction
```
Player presses E near crafting table
  → NodeActionSystem.OnInteract() [guard: IsOpen==false AND IsCraftingTableOpen==false]
    → Node.Interact() → CraftingTableInteractable.Interact()
      → SetCraftingTableOpen(!m_isOpen)
        → PlayerInventory.SetCraftingTableOpen(bool) → OnCraftingTableToggled fires
        → CraftingTableUI.SetVisible(true/false)
        → HotbarUI dims/restores (via OnCraftingTableToggled)

Tab while crafting table is open
  → PlayerInventory.ToggleInventory() → OnInventoryToggled(true)
    → CraftingTableInteractable.OnInventoryToggled(true)
      → SetCraftingTableOpen(false) → bench closes, inventory opens normally

Escape while crafting table is open
  → PlayerInventory.OnCloseUI() → OnCloseUIRequested fires
    → CraftingTableInteractable.OnCloseUIRequested()
      → SetCraftingTableOpen(false)

Player walks out of interaction range
  → CraftingTableInteractable.Update() detects distance > m_closeDistance
    → SetCraftingTableOpen(false)
```

### GlowCore upgrade interaction
```
Player presses E near a GlowCore
  → NodeActionSystem.OnInteract() [guard: !IsOpen AND !IsCraftingTableOpen AND !IsGlowCoreUIOpen]
    → Node.Interact() → GlowCoreObject.Interact()
      → GlowCoreUpgradeUI.Show(this)  [target typed as IGlowCoreObject]
        → subscribes to target.OnProgressChanged + target.OnLevelUp
        → builds FeedMaterialRowUI rows from target.LevelConfig.RequiredMaterials
        → PlayerInventory.SetGlowCoreUIOpen(true) → OnGlowCoreUIToggled(true)
          → HotbarUI dims

Player adjusts a stepper on a row
  → AmountStepperUI.OnValueChanged(newValue)
    → FeedMaterialRowUI.Refresh()
        → SetBounds(0, GetMaxSelectable()) [clamps stepper first]
        → reads pending = m_stepper.Value [after clamping — avoids stale read]
        → repaints count label: "{accumulated + pending}/{required}"
    → FeedMaterialRowUI.OnSelectionChanged fires
      → GlowCoreUpgradeUI.RefreshFeedButton() re-evaluates button interactable + label

Player clicks FEED MATERIALS [button label = "FEED MATERIALS", not yet ready to upgrade]
  → GlowCoreUpgradeUI.OnFeedButtonClicked
    → for each row with SelectedAmount > 0: target.FeedMaterial(item, amount)
      → GlowCoreObject.FeedMaterial:
          → m_playerInventory.RemoveItems(item, consume)
          → m_accumulated[item] += consume
          → if item is Wood: Fire.FeedWood(consume) — fire VFX only (no expansion)
          → ExpandIfConfigured(item, consume):
              if item == ExpansionItem: bank amount, Expand(1) per ExpansionCostPerTile threshold
          → OnProgressChanged fires
            → UI refreshes header + progress bar + rows + button
    → all steppers reset to 0

All materials met: IsReadyToUpgrade = true
  → button label changes to "UPGRADE TO LEVEL N", button always enabled
  → NextLevelIcon switches to Color.white (was WoodBorderLight)

Player clicks UPGRADE TO LEVEL N
  → GlowCoreUpgradeUI.OnFeedButtonClicked
    → m_target.IsReadyToUpgrade == true → calls m_target.Upgrade()
      → GlowCoreObject.Upgrade():
          → OnLevelUp fires → GlowCoreUpgradeUI.Hide()
          → UpgradePhysical(): WorldGrid.Expand(m_levelConfig.TilesOnLevelUp, isLevelUp: true)
          → SpawnNextLevel():
              → clears all tiles in oldNode.TilesUsed
              → Instantiates m_nextLevelPrefab at same position, under transform.parent
              → reads nextConfig.TileCount, calls WorldGrid.PlaceNodeAt(newNode, x, z, tileCount)
              → Destroy(gameObject)

Player walks > m_closeDistance away [GlowCoreObject.Update()]
  → GlowCoreObject detects distance > m_closeDistance → m_ui.Hide()

Tab / Escape / close button
  → UI hides → PlayerInventory.SetGlowCoreUIOpen(false) → HotbarUI restores
```

### Opening/closing inventory
```
PlayerInventory.OnInventory(InputValue) → ToggleInventory()
  → IInventoryService.OnInventoryToggled(bool)
    → InventoryUI: show/hide panel, cancel held item, hide crafting
    → HotbarUI: dim/restore
```

### Changing hotbar slot
```
PlayerInventory.OnHotbarSlot1..8(InputValue) → SelectHotbarSlot(i)
  → IInventoryService.OnHotbarSelectionChanged(i)
    → HotbarUI.UpdateSelection() → HotbarSlotUI.SetSelected()
  → PlayerHand.SetItemInHand()
```

---

## Input Actions

All input is event-driven via Unity Input System action callbacks on `PlayerInventory`.
**No `Update()` polling exists in any inventory or UI script.**

| Action | Key | Receiver | Callback |
|---|---|---|---|
| Inventory | Tab | PlayerInventory | `OnInventory` → `ToggleInventory()` |
| Crafting | C | PlayerInventory | `OnCrafting` → fires `OnCraftingToggled` (only while inventory open) |
| CloseUI | Escape | PlayerInventory | `OnCloseUI` → closes inventory if open; always fires `OnCloseUIRequested` |
| HotbarSlot1–8 | 1–8 | PlayerInventory | `OnHotbarSlot1..8` → `SelectHotbarSlot()` |
| Next | Scroll down | PlayerInventory | `OnNext` → decrement slot |
| Previous | Scroll up | PlayerInventory | `OnPrevious` → increment slot |

**Note:** Crouch was rebound from C to Left Ctrl to free C for crafting.

---

## File Index

| File | Layer | Type | Purpose |
|---|---|---|---|
| `IInventoryService.cs` | Interface | Interface | Data layer facade |
| `ICraftingService.cs` | Interface | Interface | Crafting layer facade |
| `IGlowCoreObject.cs` | Interface | Interface | GlowCore data layer facade — used by all GlowCore UI |
| `SlotData.cs` | Shared | Struct | Immutable slot snapshot + event payload |
| `ScriptableObjects/Item.cs` | Data | ScriptableObject | Item type definition |
| `ScriptableObjects/ItemStack.cs` | Data | Serializable class | Runtime stack instance |
| `ScriptableObjects/Recipe.cs` | Data | ScriptableObject | Crafting recipe definition (`RequiresCraftingTable` flag) |
| `ScriptableObjects/RecipeList.cs` | Data | ScriptableObject | Global recipe registry — single asset shared by all crafting UIs |
| `Inventory.cs` | Data | Plain C# class | Slot grid logic |
| `PlayerInventory.cs` | Data | MonoBehaviour | Implements IInventoryService, owns Inventory |
| `IHandItem.cs` | Data | Interface | Contract for usable held items |
| `IPlayerInventoryAware.cs` | Data | Interface | Contract for held items needing inventory access |
| `PlayerHand.cs` | Data | MonoBehaviour | In-hand item visual, enables IHandItem, wires SetInventory |
| `ItemStackDrop.cs` | Data | MonoBehaviour | World-dropped item |
| `CraftingSystem.cs` | Crafting | Plain C# class | Implements ICraftingService |
| `CraftingTableInteractable.cs` | Data | MonoBehaviour | IInteractable — opens/closes CraftingTableUI on E key; auto-closes on Tab, Escape, or walking away |
| `BlockBehaviour.cs` | Data | MonoBehaviour | IHandItem + IPlayerInventoryAware — block placement |
| `UI/Inventory/InventoryUI.cs` | UI | MonoBehaviour | Panel controller + drag-and-drop |
| `UI/Inventory/CraftingUI.cs` | UI | MonoBehaviour | Hand-crafting panel (C key), reads from RecipeList |
| `UI/Inventory/CraftingTableUI.cs` | UI | MonoBehaviour | Bench crafting panel + read-only inventory display |
| `UI/Inventory/RecipeRowUI.cs` | UI | MonoBehaviour | Single recipe row |
| `UI/Inventory/ItemSlotUI.cs` | UI | MonoBehaviour | Single slot (grid + hotbar row) |
| `UI/Inventory/HotbarUI.cs` | UI | MonoBehaviour | Standalone hotbar controller |
| `UI/Inventory/HotbarSlotUI.cs` | UI | MonoBehaviour | Single standalone hotbar slot |
| `UI/Inventory/TooltipUI.cs` | UI | MonoBehaviour | Mouse-following tooltip |
| `UI/Inventory/ItemIconHelper.cs` | UI | Static utility | Applies Sprite icons to Image components |
| `UI/UIColors.cs` | UI | Static constants | Color tokens |
| `ScriptableObjects/GlowCoreLevelConfig.cs` | Data | ScriptableObject | Per-level GlowCore materials, tile expansion, ExpansionItem, TileCount |
| `GlowCore.cs` | Data | MonoBehaviour | `GlowCoreObject` — IInteractable + IGlowCoreObject; dispatch-by-level; FeedMaterial + Upgrade |
| `Fire.cs` | Data | MonoBehaviour | Fire VFX scaling only — no world expansion logic |
| `UI/GlowCore/GlowCoreUpgradeUI.cs` | UI | MonoBehaviour | GlowCore upgrade panel controller (left) + read-only inventory (right) |
| `UI/GlowCore/FeedMaterialRowUI.cs` | UI | MonoBehaviour | Single feed-material row with stepper |
| `UI/GlowCore/AmountStepperUI.cs` | UI | MonoBehaviour | Reusable −/+ numeric stepper |
| `UI/GlowCore/ProgressBarUI.cs` | UI | MonoBehaviour | Reusable progress bar (Image.fillAmount + % label) |

---

## Key Dependencies & Must-Knows

- **UI depends only on interfaces** (`IInventoryService`, `ICraftingService`, `IGlowCoreObject`), never on `PlayerInventory`, `Inventory`, `CraftingSystem`, or `GlowCoreObject` directly.
- **`SlotData` is immutable.** UI receives snapshots via events and `Refresh(SlotData)` — never holds references to live `ItemStack` objects.
- **No `Update()` in any UI script.** All input is event-driven via Input Actions; pointer events use EventSystem handlers. The one `Update()` in the GlowCore layer lives on `GlowCoreObject` (distance auto-close), not on the UI panel.
- **`CraftingSystem` is a plain C# class**, not a MonoBehaviour. Created by `CraftingUI` and `CraftingTableUI`, each owning their own instance. Both are disposed in `OnDestroy()`.
- **`PlayerInventory` is the `IInventoryService` implementor.** It wraps `Inventory.OnSlotChanged(int)` into rich `SlotChangedEvent` payloads.
- **Crouch is now Left Ctrl**, C is Crafting toggle.
- **`Item.Icon` is a `Sprite`.** Import textures with Texture Type = Sprite (2D and UI). Read/Write is **not** required. The `RawImage` drag cursor accesses `sprite.texture` directly — this is intentional.
- **`PlayerHand` is a singleton** (`PlayerHand.Instance`). Its `m_playerInventory` field must be wired in the Inspector.
- **Hand item prefabs store `IHandItem` components disabled.** `PlayerHand.UpdateHandVisual()` enables them. Do not enable them in the prefab — that breaks the lifecycle ordering.
- **`IPlayerInventoryAware` is called automatically** by `PlayerHand.UpdateHandVisual()` — no manual wiring needed beyond prefab setup.
- **`Add()` fills hotbar first.** `PlayerInventory.Add()` passes `emptySlotStart = HotbarStartIndex`.
- **`NodeActionSystem` blocks interaction while any UI is open.** It guards on `IsOpen`, `IsCraftingTableOpen`, **and `IsGlowCoreUIOpen`** — hovering and interacting with world nodes is disabled while any UI is open.
- **`CraftingTableUI` reads all global recipes** (no filter). `CraftingUI` is the one that filters to `RequiresCraftingTable == false`. If a recipe should only appear at the bench, set `RequiresCraftingTable = true` — it will be excluded from `CraftingUI` automatically.
- **`CraftingTableInteractable` owns the open/close state** of the bench. It calls `PlayerInventory.SetCraftingTableOpen(bool)` — never set `IsCraftingTableOpen` from anywhere else.
- **Escape fires `OnCloseUIRequested` unconditionally**, even when inventory is not open. `CraftingTableInteractable` uses this to close the bench UI without needing an inventory open state check.
- **`GlowCoreObject.FeedMaterial` is the only way to feed the GlowCore.** The old wood-only `Interact()` path is gone. `FeedMaterial` does **not** auto-upgrade — the player must explicitly click the upgrade button, which calls `GlowCoreObject.Upgrade()`.
- **`Fire.FeedWood` is VFX-only.** It is called from `FeedPhysicalLevel1` when Wood is fed. It no longer drives world expansion. Incremental expansion is config-driven via `ExpandIfConfigured`; level-up expansion lives in `UpgradePhysical`.
- **Each GlowCore level is a separate prefab** referenced through `m_nextLevelPrefab`. The per-level cost, icon, perk, `TileCount`, and `ExpansionItem` live on a `GlowCoreLevelConfig` SO assigned to each prefab via `m_levelConfig`. `NextLevelConfig` reads the next prefab's config without instantiating it.
- **`m_playerInventory` in `GlowCoreObject` is found at runtime** via `FindFirstObjectByType<PlayerInventory>()` in `Awake()`. This is intentional — it keeps every GlowCore level prefab self-contained with no manual Inspector wiring for the inventory reference.
- **Auto-close distance check lives in `GlowCoreObject.Update()`**, not in `GlowCoreUpgradeUI`. This mirrors the `CraftingTableInteractable` pattern. `GlowCoreUpgradeUI` has no `Update()`.
- **Multi-tile GlowCore levels** use `TileCount` on the SO. `SpawnNextLevel()` clears all tiles tracked in `oldNode.TilesUsed` before registering the new node with `WorldGrid.PlaceNodeAt(node, x, z, tileCount)`, which computes a centered square of side `√tileCount`.

---

## How to Extend

### Add a new UI panel (equipment, chest, shop)
1. Create a new MonoBehaviour that takes `IInventoryService` (via `[SerializeField] PlayerInventory`)
2. Subscribe to `OnSlotChanged` for reactive updates
3. Call service commands (`Swap`, `TryMerge`, etc.) — never access `Inventory` directly

### Add a new item
1. Create → Scriptable Objects → Item, fill fields (Icon must be a Sprite asset — Texture Type: Sprite (2D and UI))
2. Done — system picks it up automatically

### Add a new crafting recipe
1. Create → Scriptable Objects → Recipe, fill fields
2. Add to the global `RecipeList` asset (`Assets/ScriptableObjects/Recipes/GlobalRecipeList.asset`) — both `CraftingUI` and `CraftingTableUI` read from it automatically
3. Set `RequiresCraftingTable = true` if the recipe should only be available at a crafting table — `CraftingUI` will exclude it; `CraftingTableUI` will still show it (it shows all recipes)
4. Leave `RequiresCraftingTable = false` for hand-craftable recipes — they appear in both UIs

### Add right-click actions
Add `IPointerClickHandler` to `ItemSlotUI`, check for `InputButton.Right`,
call a new method on `InventoryUI`.

### Make a usable held item
1. Attach a component implementing `IHandItem` to the item's Prefab root.
2. **Disable the component** in the prefab (`m_Enabled: 0`) — `PlayerHand.UpdateHandVisual()` enables it at runtime.
3. If the item needs to consume itself from the inventory, also implement `IPlayerInventoryAware` — `SetInventory` will be called automatically.
