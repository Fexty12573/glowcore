# Inventory System Architecture

> Reference document for the GlowCore inventory UI system.
> Last updated: 2026-05-26 (GC-224: Right-click stack splitting added. `ItemSlotUI` now implements `IPointerClickHandler` and forwards right-clicks to `InventoryUI.OnSlotRightClicked`. `InventoryUI` gained a parallel "floating-hold" state (`m_floatingItem`, `m_floatingAmount`, `m_floatingSource`) that coexists with the existing left-click whole-slot ghost-hold. Right-click halves an idle stack; right-click while floating-holding places one item onto a compatible slot. A polled `Update()` follows the cursor only while floating-holding. Closing any panel returns floating items to the source container via `AddStack`. Works across all `IItemContainer`s — no per-container code.)

---

## Overview

The inventory system has three layers: a **data layer** (pure C#, no UI), a **crafting layer**
(pure C#, no UI), and a **UI layer** (uGUI). All layers are decoupled through interfaces
(`IInventoryService`, `ICraftingService`, `IGlowCoreObject`, `IItemContainer`) — UI logic depends only on
these interfaces, never on concrete MonoBehaviour types.

`IItemContainer` is the **smallest** of the inventory contracts: any slot-based container
(player inventory, chest, future barrel, etc.) implements it. The drag-and-drop UI is
container-agnostic — it operates on `IItemContainer` and works seamlessly across panels.

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

### `IItemContainer` — (`Assets/Scripts/IItemContainer.cs`)

The minimal contract for any slot-based container. Implemented by `PlayerInventory` and
`Chest`. Used by `ItemSlotUI`, `InventoryUI` (drag/drop), and `ContainerOps` (bulk moves).

| Member | Kind | Purpose |
|---|---|---|
| `SlotCount` | Property | Total slots |
| `GetSlotData(int)` | Query | `SlotData` snapshot for a flat index |
| `CountItem(Item)` | Query | Total of an item across all slots |
| `CanAcceptItem(Item, int)` | Query | Is there room? |
| `Swap(int, int)` | Command | Swap two slots within this container |
| `TryMerge(int, int)` | Command | Merge two slots if same item |
| `SetSlot(int, Item, int)` | Command | Direct slot write (used for cross-container moves) |
| `AddStack(Item, int)` | Command | Bulk-add; returns amount actually added |
| `RemoveItems(Item, int)` | Command | Bulk-remove; returns amount actually removed |
| `OnSlotChanged` | Event | `Action<SlotChangedEvent>` |

### `IInventoryService` — (`Assets/Scripts/IInventoryService.cs`)

The single contract between the data layer and all consumers (UI, crafting, etc.).

| Member | Kind | Purpose |
|---|---|---|
| `SlotCount` | Property | Total slots (32) |
| `HotbarSlotCount` | Property | Hotbar size (8) |
| `SelectedHotbarIndex` | Property | Currently selected hotbar slot |
| `IsOpen` | Property | Whether inventory panel is open |
| `IsCraftingStationOpen` | Property | Whether the crafting table panel is open |
| `IsGlowCoreUIOpen` | Property | Whether the GlowCore upgrade panel is open |
| `IsChestOpen` | Property | Whether a chest panel is open |
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
| `SetCraftingStationOpen(bool)` | Command | Set crafting table open state, fires `OnCraftingStationToggled` |
| `SetGlowCoreUIOpen(bool)` | Command | Set GlowCore UI open state, fires `OnGlowCoreUIToggled` |
| `SetChestOpen(bool)` | Command | Set chest open state, fires `OnChestToggled` |
| `RemoveItems(Item, int)` | Command | Remove items across slots, returns amount removed |
| `AddItem(Item, int)` | Command | Add items to inventory |
| `OnSlotChanged` | Event | `Action<SlotChangedEvent>` — fires with slot index + data snapshot |
| `OnHotbarSelectionChanged` | Event | `Action<int>` — fires with new hotbar index |
| `OnInventoryToggled` | Event | `Action<bool>` — fires with open state |
| `OnCraftingToggled` | Event | `Action` — fires when C key pressed while inventory is open |
| `OnCraftingStationToggled` | Event | `Action<bool>` — fires when crafting table is opened/closed |
| `OnGlowCoreUIToggled` | Event | `Action<bool>` — fires when the GlowCore upgrade panel is opened/closed |
| `OnChestToggled` | Event | `Action<bool>` — fires when a chest panel is opened/closed |
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
- `OnCloseUI(InputValue)` → Escape key → closes inventory if open; always fires `OnCloseUIRequested` (used by `CraftingStationInteractable` to close the bench)
- `OnHotbarSlot1..8(InputValue)` → keys 1-8 → `SelectHotbarSlot()`
- `OnNext(InputValue)` / `OnPrevious(InputValue)` → scroll wheel navigation

**Inspector fields:** `m_playerHand` — reference to the `PlayerHand` component.

**Extra public methods (not on `IInventoryService`):**
- `ConsumeHandItem(int amount)` — removes `amount` items from the currently selected hotbar slot. Used by `BlockBehaviour` after a successful block placement.
- `SetCraftingStationOpen(bool)` — sets `m_isCraftingTableOpen` and fires `OnCraftingStationToggled`. Called by `CraftingStationInteractable`.

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

**Auto-close:** `Update()` runs while `m_ui.IsVisible`; if the player drifts beyond `m_closeDistance` on the XZ plane, calls `m_ui.Hide()`. This mirrors the `CraftingStationInteractable.Update()` pattern and keeps the UI clean of distance logic.

### `Chest` — MonoBehaviour, `IItemContainer` (`Assets/Scripts/Chest.cs`)

The world-placed chest's data layer. Owns its own `Inventory(m_width, m_height)` — `m_width` and `m_height` are `[SerializeField, Min(1)]` fields (defaults `8` × `2`, i.e. 16 slots) so each chest prefab/instance can declare its own 2D size in the Inspector. Implements `IItemContainer` by delegating to that inventory, and translates the inventory's `Action<int>` slot-changed event into the rich `Action<SlotChangedEvent>` payload that the UI consumes.

`Chest` carries no UI logic and no interaction logic — all of that lives in `ChestInteractable`.

**Note:** `ChestUI` builds slots dynamically from `chest.SlotCount`. The visual column wrap is controlled by the `GridLayoutGroup` constraint on the chest panel prefab (currently fixed at 8 columns) — change that constraint if you need a different wrap.

### `ChestInteractable` — MonoBehaviour, `IInteractable` (`Assets/Scripts/ChestInteractable.cs`)

`[RequireComponent(typeof(Chest))]`. Mirrors `CraftingStationInteractable`. On `Interact()` it flips its open state and:

- Calls `PlayerInventory.SetChestOpen(open)` — fires `OnChestToggled`.
- Calls `ChestUI.Show(chest)` / `ChestUI.Hide()`.

Auto-closes on Tab (inventory open), Escape (`OnCloseUIRequested`), or walking beyond `m_closeDistance` (default 5f, checked in `Update`).

### `ContainerOps` — Static helper (`Assets/Scripts/ContainerOps.cs`)

Container-agnostic bulk operations. Used by `ChestUI` for TAKE ALL / INSERT ALL and by `InventoryUI` for cross-container drag releases.

| Member | Purpose |
|---|---|
| `TransferAll(IItemContainer source, IItemContainer destination)` | Iterates source slots, calls `destination.AddStack` then `source.SetSlot(..., null, 0)` for the consumed amount. |
| `MoveStack(IItemContainer src, int srcIdx, SlotData srcData, IItemContainer dst, int dstIdx, SlotData dstData)` | Single-slot cross-container move: empty target → full move; same item → merge up to MaxStack; different item → swap. Pure `SetSlot` operations under the hood. |

### `Fire` — MonoBehaviour (`Assets/Scripts/Fire.cs`)

Handles **fire VFX scaling only**. `FeedWood(int amount)` is called from `GlowCoreObject.FeedPhysicalLevel1` when `Wood` is fed — it updates `TotalWoodReceived` and scales the fire particle system. **All world expansion logic has been removed from Fire.** Incremental tile expansion is handled by `GlowCoreObject.ExpandIfConfigured`; level-up expansion is handled by `GlowCoreObject.UpgradePhysical`.

### `WorldGrid` — MonoBehaviour (`Assets/Scripts/WorldGrid.cs`)

Manages the world tile grid. Key method relevant to GlowCore:

- `PlaceNodeAt(Node node, int x, int z, int tileCount)` — computes a centered square region of side `√tileCount` and registers all tiles to the node. Used by `SpawnNextLevel()` so multi-tile GlowCore variants (e.g. Level 2 with `TileCount = 4` occupying 2×2) are registered correctly.
- `ClearNodeAt(Vector2Int tile)` — used by `SpawnNextLevel()` to free all tiles in `oldNode.TilesUsed` before spawning the replacement.

---

## Crafting Layer

### `CraftingSystem` — Plain C# class, implements `ICraftingService` (`Assets/Scripts/CraftingSystem.cs`)

Pure logic class. No MonoBehaviour, no UI. Created independently by both `CraftingUI` and `CraftingStationUI` in their respective `Start()` methods — each instance owns its own filtered recipe slice.

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
- `InventoryUI` receives forwarded calls — they all pass `ItemSlotUI` (the slot itself), not just an index, so the controller can read the slot's owning `IItemContainer`.

**Held-item state — two parallel modes:**

| Mode | Trigger | Fields | Source slot | Cancel behavior |
|---|---|---|---|---|
| **Ghost-hold** | Left-click drag | `m_heldSlot`, `m_heldContainer`, `m_heldSlotIndex`, `m_isHolding` | Ghosted, data unchanged | Un-ghost; data never moved |
| **Floating-hold** | Right-click halve | `m_floatingItem`, `m_floatingAmount`, `m_floatingSource` | Mutated (split already applied) | `AddStack` remainder back to `m_floatingSource` |

Only one mode is active at a time. Both share the same `m_cursorIcon` for rendering. `IsHoldingItem` returns `m_isHolding || m_floatingAmount > 0`. This lets a single drag controller handle drops on slots in *any* panel for both interaction styles.

**Right-click handler — `OnSlotRightClicked(ItemSlotUI slot)`** (single entry point):
- **Idle:** `TryHalveSlot` → if `data.Amount > 1`, takes `pickup = data.Amount / 2`, mutates source slot to `N − pickup`, begins floating-hold. Slots with a single item (incl. unstackable items like the axe) are skipped — left-drag is the right tool for moving them.
- **Floating-hold:** `TryPlaceOneOnSlot` → places 1 onto target if empty or same-item-with-room; decrements `m_floatingAmount`; ends floating-hold when it hits 0.
- **Ghost-hold:** no-op (right-click during a left-drag is reserved for future use).

**Off-slot click while floating — `HandleOffSlotClick()` (polled from `Update`)**:
Any mouse click whose target is not a slot is treated as a world-drop intent. The check is `m_hoveredSlot == null` at click time — slot clicks are dispatched by `IPointerDownHandler` before `Update` runs, so the hover state is authoritative. Rules:
- Source = `PlayerInventory` and GlowCore UI closed: drops to world. **Left-click drops all** floating, **right-click drops one**, mirroring the slot place-all/place-one rule.
- Source = chest, or GlowCore UI open: cancels (returns floating to source). Chest items never drop to world — same rule as the left-drag release-outside path.
- No backdrop GameObject or Inspector wiring needed; works wherever the inventory is open.

**Left-click handler while floating — `TryDumpFloatingOnSlot(ItemSlotUI slot)`**:
- Empty target → places the whole floating stack and ends floating-hold.
- Same item → tops the target up to `MaxStack`, keeps the remainder floating.
- Different item → Minecraft-style swap: target's old contents become the new floating stack and the floating stack is placed into the target. (Use this to cancel a halve by left-clicking the original source slot.)

**Cursor visuals:** `UpdateCursorIcon(Item)` / `UpdateCursorCount(int)` / `HideCursor()` are shared by both hold modes. The icon is the `RawImage`; the count is `m_cursorCountText` (TextMeshProUGUI, optional — leave unwired if not desired). Count is hidden when amount ≤ 1, matching slot-count rendering in `ItemIconHelper`.

**Cursor follow during floating-hold:** right-click doesn't fire `IDragHandler`, so there's no pointer-move event to ride. `InventoryUI.Update()` polls `Mouse.current.position` only while `m_floatingAmount > 0` — does nothing when idle. This is the only `Update()` in the entire UI layer.

**Drag-and-drop flow (left-click):**
1. `OnSlotPressed(ItemSlotUI slot)` → reads `slot.Container`, `slot.SlotIndex`, calls `PickUpItem(slot, data)`
2. `PickUpItem(slot, data)` → ghosts source slot, shows cursor icon, stores held container + index
3. `ItemSlotUI.OnDrag` → `InventoryUI.OnDragUpdate` updates cursor and tooltip position
4. `ItemSlotUI.OnEndDrag` → `InventoryUI.OnSlotReleased` → `DropHeldItem()`
5. **Drop logic** (in `DropHeldItem`):
   - Same container as source: existing `Swap`/`TryMerge` on the container
   - **Different container:** `ContainerOps.MoveStack(heldContainer, heldIdx, heldData, hoveredContainer, hoveredIdx, hoveredData)`
   - Released outside any slot: world-drop, but **only if held came from `PlayerInventory`** (chest items don't drop to world)
6. `CancelHeldItem()` → cleanup (also calls `ReturnFloatingToSource` + `EndFloatingHold`, no-ops when not floating)

**Right-click flow (split / place-one):**
1. `ItemSlotUI.OnPointerClick` filters for `InputButton.Right` → `InventoryUI.OnSlotRightClicked(slot)`
2. Idle → halve (only when source `Amount > 1`): `SetSlot(idx, item, N − pickup)` on the slot's container, begin floating-hold with `(item, pickup)`. Single-item slots are skipped.
3. Floating-hold → place one: `SetSlot(idx, item, hoverData.Amount + 1)` (or `1` if empty); decrement floating amount; end floating-hold at 0

**Left-click flow while floating (dump / swap):**
1. `ItemSlotUI.OnPointerDown` (Left) → `InventoryUI.OnSlotPressed(slot)`. While floating, routes to `TryDumpFloatingOnSlot`.
2. Empty target → place all, end floating.
3. Same item → top up to `MaxStack`, keep remainder floating.
4. Different item → swap: target contents become the new floating stack.

**Close-while-holding cleanup:** UI close path (`OnInventoryToggled(false)`, `ChestUI.Hide`, `GlowCoreUpgradeUI.Hide`) all route through `CancelHeldItem` → `ReturnFloatingToSource` → `m_floatingSource.AddStack(item, remaining)`. Ghost-hold un-ghosts the source slot (data never moved).

### `ItemSlotUI` — MonoBehaviour (`ItemSlotUI.cs`)

**Reusable slot component, container-aware.** Receives `SlotData` via `Refresh(SlotData)` — never queries the data layer. Initialized with `Initialize(InventoryUI owner, IItemContainer container, int slotIndex, SlotData)`.

The slot exposes `Container`, `SlotIndex`, and `CurrentData` properties so the owner (`InventoryUI`) can route drag operations to the correct container without caring whether the slot belongs to the player inventory, a chest, or a future container type.

**Pointer interfaces:** `IPointerDownHandler` + `IPointerUpHandler` (left-click drag lifecycle), `IPointerClickHandler` (forwards right-click via `OnSlotRightClicked`), `IBeginDragHandler` + `IDragHandler` + `IEndDragHandler` (drag), `IPointerEnterHandler` + `IPointerExitHandler` (hover). All logic lives in `InventoryUI`; the slot only forwards.

### `CraftingUI` — MonoBehaviour (`CraftingUI.cs`)

**Crafting panel controller.** Creates a `CraftingSystem` in `Start()` and passes it to rows.

- Reads recipes from the global `RecipeList` asset, **filtering to `RequiresCraftingTable == false`** (hand-craftable only)
- Opens via `OnCraftingToggled` event from `IInventoryService` (C key, only when inventory is open)
- Closes when inventory closes (`OnInventoryToggled`)
- No `Update()` — fully event-driven
- Subscribes to `ICraftingService.OnRecipesRefreshed` for auto-refresh

### `CraftingStationUI` — MonoBehaviour (`CraftingStationUI.cs`)

**Crafting bench panel controller.** Opens when the player interacts with a crafting table node (E key).
Combines a recipe list (left) with a read-only inventory display (right) in a single panel.

- Reads **all** recipes from the global `RecipeList` asset — **no filtering** (the global list is expected to contain all recipes; table-only recipes are naturally only accessible here)
- Creates its own `CraftingSystem` with the full recipe list
- Right-side inventory display: 32 `ItemSlotUI` instances with `null` owner (read-only, no drag/click)
- Inventory display stays in sync via `OnSlotChanged` (always subscribed, cheap no-op when hidden)
- Opened/closed by `CraftingStationInteractable` via `SetVisible(bool)` — no direct key binding
- Also auto-closes when: player walks out of range, inventory opens (Tab), or Escape is pressed

### `RecipeRowUI` — MonoBehaviour (`RecipeRowUI.cs`)

**Single recipe row.** Initialized with `ICraftingService` — calls `CanCraft()`, `GetItemCount()`,
and `Craft()` through the interface. No direct inventory access.

`Refresh()` recomputes craftability: per-ingredient labels colored `UIColors.Green`/`UIColors.MissingMat`,
craft button `interactable` set, and two sprite swaps driven by `CanCraft()` — the row background `Image`
(`m_defaultBackground` `recipe_row` vs `m_craftableBackground` `recipe_row_craftable_preview`) and the
craft button `Image` (`m_craftButtonInactive` vs `m_craftButtonActive`). Background/button state is
sprite-based, not color-tinted.

**Per-station theming:** `Initialize(..., RecipeRowTheme theme = null)` accepts an optional
`RecipeRowTheme` ScriptableObject. `CraftingStationUI` passes `CraftingStation.RecipeRowTheme`;
`CraftingUI` (hand-crafting) passes nothing. `ApplyTheme()` overrides only the sprites the theme
actually sets — a null sprite on the theme leaves the prefab default, so a station can re-skin just
the parts it wants. To give a station custom rows/buttons: create a `RecipeRowTheme` asset
(Create → Scriptable Objects → RecipeRowTheme), fill the sprites, assign it to the station's
`RecipeRowTheme` field. No prefab variants needed.

### `ChestUI` — MonoBehaviour (`ChestUI.cs`)

**Two-panel chest controller.** Mirrors `CraftingStationUI` structure:

- Left: chest grid (slot count from `Chest.m_width × Chest.m_height`, default 8 × 2 = 16; visual wrap from the panel's `GridLayoutGroup`, currently 8 columns)
- Center: TAKE ALL / INSERT ALL buttons
- Right: read/write player inventory display (32 slots, full drag)

Subscribes to both the chest's `OnSlotChanged` (via `BindChest`) and the player's `OnSlotChanged` (in `Start`). Slots on **both** sides are owned by `InventoryUI` so cross-panel drag works through the shared cursor and held-item state.

**Lifecycle:**
- `Start()` calls `EnsureInitialized()` then `SetVisible(false)`.
- `EnsureInitialized()` is idempotent: finds `PlayerInventory` and `InventoryUI`, wires button listeners, and calls `BuildInventoryDisplay()` (which clears existing children first to avoid double-population). Subscribes to `OnSlotChanged`.
- `Show(Chest)` calls `EnsureInitialized()`, `BindChest(chest)`, `SetVisible(true)`. Lazy-init makes `Show` safe to call before the first frame.
- `Hide()` / `SetVisible(false)` cancels any held drag (`InventoryUI.CancelHeldItem`) and unbinds the chest.

Buttons → `ContainerOps.TransferAll(chest, player)` and `ContainerOps.TransferAll(player, chest)`.

### `HotbarUI` — MonoBehaviour (`HotbarUI.cs`)

**Always-visible hotbar.** Uses `IInventoryService` for data and events.
No `Update()` — number key input is handled by `PlayerInventory` via Input Actions.
Dims (alpha 0.35, non-interactive) when the inventory, crafting table, **or GlowCore upgrade panel** is open — subscribes to `OnInventoryToggled`, `OnCraftingStationToggled`, and `OnGlowCoreUIToggled`.

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

Scripts live in `Assets/Scripts/UI/GlowCore/` under the `GlowCore.UI.Upgrade` namespace. The layout mirrors `CraftingStationUI`: feed-material panel on the left, read-only inventory on the right.

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

Single row: icon, name, count label, and a background image.

- `Initialize(IInventoryService, IGlowCoreObject target, Item, int required)` — takes `IGlowCoreObject`, not the concrete class. Sets the name label, wires the icon via `ItemIconHelper.GetSprite`, then calls `Refresh()`.
- `Refresh()` — `accumulated = target.AccumulatedFor(item)`; `enough = accumulated >= required`. Count label format: `"{accumulated}/{required}"`, colored `UIColors.Green` when `enough`, otherwise `UIColors.MissingMat`. The background `Image` swaps between `m_defaultBackground` and `m_completeBackground` sprites on the same `enough` flag (`material_row` → `material_row_complete`).
- Exposes `Material` and `MaxFeedable` (= `min(stillNeeded, have)`) for the controller; `GlowCoreUpgradeUI` reads `MaxFeedable` to decide what to feed and whether the feed button is enabled.

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
  ├── CraftingBenchPanel (CraftingStationUI + CanvasGroup — starts hidden)
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
  ├── ChestPanel (ChestUI + CanvasGroup — starts hidden, panel 780×240)
  │     └── ChestInventoryArea (HorizontalLayoutGroup)
  │           ├── ChestSection (~380px wide)
  │           │     ├── TitleLabel (TMP — "CHEST", Cinzel Bold 11px, AccentDim)
  │           │     └── ChestGrid (GridLayoutGroup: 40×40 cells, 4px spacing, 8 columns)
  │           │           └── [16 ItemSlotUI spawned at runtime — owner = InventoryUI, container = Chest]
  │           ├── Border (separator)
  │           ├── TransferButtons (VerticalLayoutGroup, 70px wide)
  │           │     ├── TakeAllButton (Button — chest → player via ContainerOps.TransferAll)
  │           │     └── InsertAllButton (Button — player → chest via ContainerOps.TransferAll)
  │           └── InventorySection (~280px wide)
  │                 ├── Title (TMP — "INVENTORY", Cinzel Bold 11px, AccentDim)
  │                 ├── UpperGrid (GridLayoutGroup)
  │                 │     └── [24 ItemSlotUI spawned at runtime — owner = InventoryUI, container = PlayerInventory]
  │                 └── HotbarRow (HorizontalLayoutGroup)
  │                       └── [8 ItemSlotUI spawned at runtime, hotbar-styled — slots 0–7]
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
  → NodeActionSystem.OnInteract() [guard: IsOpen==false AND IsCraftingStationOpen==false]
    → Node.Interact() → CraftingStationInteractable.Interact()
      → SetCraftingStationOpen(!m_isOpen)
        → PlayerInventory.SetCraftingStationOpen(bool) → OnCraftingStationToggled fires
        → CraftingStationUI.SetVisible(true/false)
        → HotbarUI dims/restores (via OnCraftingStationToggled)

Tab while crafting table is open
  → PlayerInventory.ToggleInventory() → OnInventoryToggled(true)
    → CraftingStationInteractable.OnInventoryToggled(true)
      → SetCraftingStationOpen(false) → bench closes, inventory opens normally

Escape while crafting table is open
  → PlayerInventory.OnCloseUI() → OnCloseUIRequested fires
    → CraftingStationInteractable.OnCloseUIRequested()
      → SetCraftingStationOpen(false)

Player walks out of interaction range
  → CraftingStationInteractable.Update() detects distance > m_closeDistance
    → SetCraftingStationOpen(false)
```

### GlowCore upgrade interaction
```
Player presses E near a GlowCore
  → NodeActionSystem.OnInteract() [guard: !IsOpen AND !IsCraftingStationOpen AND !IsGlowCoreUIOpen]
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
| `IInventoryService.cs` | Interface | Interface | Data layer facade (player inventory + UI panel state) |
| `IItemContainer.cs` | Interface | Interface | Minimal slot-container facade (player, chest, future containers) |
| `ICraftingService.cs` | Interface | Interface | Crafting layer facade |
| `IGlowCoreObject.cs` | Interface | Interface | GlowCore data layer facade — used by all GlowCore UI |
| `SlotData.cs` | Shared | Struct | Immutable slot snapshot + event payload |
| `ScriptableObjects/Item.cs` | Data | ScriptableObject | Item type definition |
| `ScriptableObjects/ItemStack.cs` | Data | Serializable class | Runtime stack instance |
| `ScriptableObjects/Recipe.cs` | Data | ScriptableObject | Crafting recipe definition (`RequiresCraftingTable` flag) |
| `ScriptableObjects/RecipeList.cs` | Data | ScriptableObject | Global recipe registry — single asset shared by all crafting UIs |
| `Inventory.cs` | Data | Plain C# class | Slot grid logic; `SetSlot(int, Item, int)` overload + `AddStack` for cross-container ops |
| `PlayerInventory.cs` | Data | MonoBehaviour | Implements IInventoryService **and IItemContainer**, owns Inventory |
| `Chest.cs` | Data | MonoBehaviour | Implements IItemContainer; owns its own Inventory sized by serialized `m_width × m_height` (default 8 × 2 = 16) |
| `ChestInteractable.cs` | Data | MonoBehaviour | IInteractable on chest GameObject; opens/closes ChestUI |
| `ContainerOps.cs` | Data | Static helper | `TransferAll` + `MoveStack` between any two `IItemContainer`s |
| `IHandItem.cs` | Data | Interface | Contract for usable held items |
| `IPlayerInventoryAware.cs` | Data | Interface | Contract for held items needing inventory access |
| `PlayerHand.cs` | Data | MonoBehaviour | In-hand item visual, enables IHandItem, wires SetInventory |
| `ItemStackDrop.cs` | Data | MonoBehaviour | World-dropped item |
| `CraftingSystem.cs` | Crafting | Plain C# class | Implements ICraftingService |
| `CraftingStationInteractable.cs` | Data | MonoBehaviour | IInteractable — opens/closes CraftingStationUI on E key; auto-closes on Tab, Escape, or walking away |
| `BlockBehaviour.cs` | Data | MonoBehaviour | IHandItem + IPlayerInventoryAware — block placement |
| `UI/Inventory/InventoryUI.cs` | UI | MonoBehaviour | Panel controller + drag-and-drop |
| `UI/Inventory/CraftingUI.cs` | UI | MonoBehaviour | Hand-crafting panel (C key), reads from RecipeList |
| `UI/Inventory/CraftingStationUI.cs` | UI | MonoBehaviour | Bench crafting panel + read-only inventory display |
| `UI/Inventory/ChestUI.cs` | UI | MonoBehaviour | Chest panel (left grid) + player inventory (right) + TAKE ALL/INSERT ALL |
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
| `UI/GlowCore/FeedMaterialRowUI.cs` | UI | MonoBehaviour | Single feed-material row — icon, name, count label, background swaps to "complete" sprite when met |
| `UI/GlowCore/AmountStepperUI.cs` | UI | MonoBehaviour | Reusable −/+ numeric stepper |
| `UI/GlowCore/ProgressBarUI.cs` | UI | MonoBehaviour | Reusable progress bar (Image.fillAmount + % label) |

---

## Key Dependencies & Must-Knows

- **UI depends only on interfaces** (`IInventoryService`, `ICraftingService`, `IGlowCoreObject`, `IItemContainer`), never on `PlayerInventory`, `Inventory`, `CraftingSystem`, `GlowCoreObject`, or `Chest` directly.
- **`IItemContainer` is the OCP seam.** Drag-and-drop, TAKE ALL / INSERT ALL, and `ItemSlotUI` all operate on `IItemContainer`. Adding a new container type (barrel, fridge, lockbox) means: implement `IItemContainer`, give it a UI panel like `ChestUI`, and the existing drag system Just Works across panels.
- **`InventoryUI` is the single drag controller.** Even when chest slots are visible, they are still owned by `InventoryUI` — that is what enables cross-panel drag with one cursor and one held-item state. `ChestUI` only manages panel visibility and the chest-side slot list.
- **`SlotData` is immutable.** UI receives snapshots via events and `Refresh(SlotData)` — never holds references to live `ItemStack` objects.
- **No `Update()` in any UI script.** All input is event-driven via Input Actions; pointer events use EventSystem handlers. The one `Update()` in the GlowCore layer lives on `GlowCoreObject` (distance auto-close), not on the UI panel.
- **`CraftingSystem` is a plain C# class**, not a MonoBehaviour. Created by `CraftingUI` and `CraftingStationUI`, each owning their own instance. Both are disposed in `OnDestroy()`.
- **`PlayerInventory` is the `IInventoryService` implementor.** It wraps `Inventory.OnSlotChanged(int)` into rich `SlotChangedEvent` payloads.
- **Crouch is now Left Ctrl**, C is Crafting toggle.
- **`Item.Icon` is a `Sprite`.** Import textures with Texture Type = Sprite (2D and UI). Read/Write is **not** required. The `RawImage` drag cursor accesses `sprite.texture` directly — this is intentional.
- **`PlayerHand` is a singleton** (`PlayerHand.Instance`). Its `m_playerInventory` field must be wired in the Inspector.
- **Hand item prefabs store `IHandItem` components disabled.** `PlayerHand.UpdateHandVisual()` enables them. Do not enable them in the prefab — that breaks the lifecycle ordering.
- **`IPlayerInventoryAware` is called automatically** by `PlayerHand.UpdateHandVisual()` — no manual wiring needed beyond prefab setup.
- **`Add()` fills hotbar first.** `PlayerInventory.Add()` passes `emptySlotStart = HotbarStartIndex`.
- **`NodeActionSystem` blocks interaction while any UI is open.** It guards on `IsOpen`, `IsCraftingStationOpen`, **and `IsGlowCoreUIOpen`** — hovering and interacting with world nodes is disabled while any UI is open.
- **`CraftingStationUI` reads all global recipes** (no filter). `CraftingUI` is the one that filters to `RequiresCraftingTable == false`. If a recipe should only appear at the bench, set `RequiresCraftingTable = true` — it will be excluded from `CraftingUI` automatically.
- **`CraftingStationInteractable` owns the open/close state** of the bench. It calls `PlayerInventory.SetCraftingStationOpen(bool)` — never set `IsCraftingStationOpen` from anywhere else.
- **Escape fires `OnCloseUIRequested` unconditionally**, even when inventory is not open. `CraftingStationInteractable` uses this to close the bench UI without needing an inventory open state check.
- **`GlowCoreObject.FeedMaterial` is the only way to feed the GlowCore.** The old wood-only `Interact()` path is gone. `FeedMaterial` does **not** auto-upgrade — the player must explicitly click the upgrade button, which calls `GlowCoreObject.Upgrade()`.
- **`Fire.FeedWood` is VFX-only.** It is called from `FeedPhysicalLevel1` when Wood is fed. It no longer drives world expansion. Incremental expansion is config-driven via `ExpandIfConfigured`; level-up expansion lives in `UpgradePhysical`.
- **Each GlowCore level is a separate prefab** referenced through `m_nextLevelPrefab`. The per-level cost, icon, perk, `TileCount`, and `ExpansionItem` live on a `GlowCoreLevelConfig` SO assigned to each prefab via `m_levelConfig`. `NextLevelConfig` reads the next prefab's config without instantiating it.
- **`m_playerInventory` in `GlowCoreObject` is found at runtime** via `FindFirstObjectByType<PlayerInventory>()` in `Awake()`. This is intentional — it keeps every GlowCore level prefab self-contained with no manual Inspector wiring for the inventory reference.
- **Auto-close distance check lives in `GlowCoreObject.Update()`**, not in `GlowCoreUpgradeUI`. This mirrors the `CraftingStationInteractable` pattern. `GlowCoreUpgradeUI` has no `Update()`.
- **Multi-tile GlowCore levels** use `TileCount` on the SO. `SpawnNextLevel()` clears all tiles tracked in `oldNode.TilesUsed` before registering the new node with `WorldGrid.PlaceNodeAt(node, x, z, tileCount)`, which computes a centered square of side `√tileCount`.

---

## How to Extend

### Add a new container type (barrel, fridge, lockbox)
1. Implement `IItemContainer` on a MonoBehaviour. Easiest path is to wrap your own `Inventory(width, height)` (see `Chest.cs` for the template).
2. Add a sibling MonoBehaviour that implements `IInteractable` to open/close the panel — see `ChestInteractable.cs`.
3. Add an `IsXxxOpen` / `SetXxxOpen` / `OnXxxToggled` triple to `IInventoryService` and `PlayerInventory`. Update the `NodeActionSystem` interaction guard and the `HotbarUI` dim-trigger.
4. Build a UI panel similar to `ChestUI`: bind `m_inventoryUI` as the slot owner so cross-panel drag works for free; wire any TAKE ALL / INSERT ALL buttons through `ContainerOps.TransferAll`.

### Add a new general UI panel (equipment, shop, ...)
1. Create a new MonoBehaviour that takes `IInventoryService` (via `[SerializeField] PlayerInventory`)
2. Subscribe to `OnSlotChanged` for reactive updates
3. Call service commands (`Swap`, `TryMerge`, etc.) — never access `Inventory` directly

### Add a new item
1. Create → Scriptable Objects → Item, fill fields (Icon must be a Sprite asset — Texture Type: Sprite (2D and UI))
2. Done — system picks it up automatically

### Add a new crafting recipe
1. Create → Scriptable Objects → Recipe, fill fields
2. Add to the global `RecipeList` asset (`Assets/ScriptableObjects/Recipes/GlobalRecipeList.asset`) — both `CraftingUI` and `CraftingStationUI` read from it automatically
3. Set `RequiresCraftingTable = true` if the recipe should only be available at a crafting table — `CraftingUI` will exclude it; `CraftingStationUI` will still show it (it shows all recipes)
4. Leave `RequiresCraftingTable = false` for hand-craftable recipes — they appear in both UIs

### Add right-click actions
The seam is already in place: `ItemSlotUI` forwards right-clicks to
`InventoryUI.OnSlotRightClicked(ItemSlotUI slot)`. Extend that single method with a new
branch (or refactor to a strategy if more than 3 modes accumulate). Existing branches:
**idle → halve**, **floating-hold → place 1**, **ghost-hold → no-op**.

### Make a usable held item
1. Attach a component implementing `IHandItem` to the item's Prefab root.
2. **Disable the component** in the prefab (`m_Enabled: 0`) — `PlayerHand.UpdateHandVisual()` enables it at runtime.
3. If the item needs to consume itself from the inventory, also implement `IPlayerInventoryAware` — `SetInventory` will be called automatically.
