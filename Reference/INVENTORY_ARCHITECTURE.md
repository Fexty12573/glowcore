# Inventory System Architecture

> Reference document for the GlowCore inventory UI system.
> Last updated: 2026-03-31 (Synced to current interfaces and drag event flow)

---

## Overview

The inventory system has three layers: a **data layer** (pure C#, no UI), a **crafting layer**
(pure C#, no UI), and a **UI layer** (uGUI). The data and crafting layers expose interfaces
(`IInventoryService`, `ICraftingService`) used by UI logic, while scene wiring currently uses
serialized `PlayerInventory` references in UI controllers.

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
└───────────────────────┬─────────────────────────────────────┘
                        │
             IInventoryService (interface)
                        │
┌───────────────────────▼─────────────────────────────────────┐
│                     CRAFTING LAYER                           │
│                                                             │
│  CraftingSystem (plain C#) ── implements ICraftingService   │
│  Owns recipe validation + execution logic                   │
│  Receives IInventoryService in constructor                  │
└───────────────────────┬─────────────────────────────────────┘
                        │
         IInventoryService + ICraftingService
                        │
┌───────────────────────▼─────────────────────────────────────┐
│                        UI LAYER                             │
│                                                             │
│  InventoryUI ──► ItemSlotUI (×32)    (uses IInventoryService)│
│                  TooltipUI                                   │
│  CraftingUI ──► RecipeRowUI (×N)    (uses ICraftingService) │
│  HotbarUI ────► HotbarSlotUI (×8)   (uses IInventoryService)│
│                  ItemIconHelper (static)                     │
│                  UIColors (static)                           │
│                                            │
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
| `RemoveItems(Item, int)` | Command | Remove items across slots, returns amount removed |
| `AddItem(Item, int)` | Command | Add items to inventory |
| `OnSlotChanged` | Event | `Action<SlotChangedEvent>` — fires with slot index + data snapshot |
| `OnHotbarSelectionChanged` | Event | `Action<int>` — fires with new hotbar index |
| `OnInventoryToggled` | Event | `Action<bool>` — fires with open state |
| `OnCraftingToggled` | Event | `Action` — fires when C key pressed while open |

### `ICraftingService` — (`Assets/Scripts/ICraftingService.cs`)

The contract between the crafting layer and the UI.

| Member | Kind | Purpose |
|---|---|---|
| `Recipes` | Property | `IReadOnlyList<Recipe>` of all recipes |
| `CanCraft(Recipe)` | Query | Are ingredients available + space for result? |
| `GetItemCount(Item)` | Query | Convenience: count of item in inventory |
| `Craft(Recipe)` | Command | Execute crafting (remove ingredients, add result) |
| `OnRecipesRefreshed` | Event | `Action` — fires when craftability may have changed |

### `SlotData` — (`Assets/Scripts/SlotData.cs`)

Immutable value type snapshot of a slot. UI never holds references to live `ItemStack` objects.

| Field | Type | Purpose |
|---|---|---|
| `Item` | `Item` | The item type (null if empty) |
| `Amount` | `int` | Stack count |
| `IsValid` | `bool` | True if Item != null and Amount > 0 |
| `Icon` | `Texture2D` | Convenience: Item?.Icon |

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
| `Icon` | Texture2D | Icon used in UI slots and drag cursor |
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
- `OnCrafting(InputValue)` → C key → fires `OnCraftingToggled` (only when open)
- `OnHotbarSlot1..8(InputValue)` → keys 1-8 → `SelectHotbarSlot()`
- `OnNext(InputValue)` / `OnPrevious(InputValue)` → scroll wheel navigation

**Inspector fields:** `m_playerHand` — reference to the `PlayerHand` component.

### `PlayerHand` — MonoBehaviour (`Assets/Scripts/PlayerHand.cs`)

Manages the item physically held in the player's hand. Singleton (`s_instance`).

### `ItemStackDrop` — MonoBehaviour (`Assets/Scripts/ItemStackDrop.cs`)

Represents a dropped `ItemStack` in the world. Called by `PlayerInventory.DropItem()`.

---

## Crafting Layer

### `CraftingSystem` — Plain C# class, implements `ICraftingService` (`Assets/Scripts/CraftingSystem.cs`)

Pure logic class. No MonoBehaviour, no UI. Created by `CraftingUI` in `Start()`.

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

- Opens via `OnCraftingToggled` event from `IInventoryService` (C key)
- No `Update()` — fully event-driven
- Subscribes to `ICraftingService.OnRecipesRefreshed` for auto-refresh

### `RecipeRowUI` — MonoBehaviour (`RecipeRowUI.cs`)

**Single recipe row.** Initialized with `ICraftingService` — calls `CanCraft()`, `GetItemCount()`,
and `Craft()` through the interface. No direct inventory access.

### `HotbarUI` — MonoBehaviour (`HotbarUI.cs`)

**Always-visible hotbar.** Uses `IInventoryService` for data and events.
No `Update()` — number key input is handled by `PlayerInventory` via Input Actions.

### `HotbarSlotUI` — MonoBehaviour (`HotbarSlotUI.cs`)

**Single standalone hotbar slot.** Initialized with `IInventoryService`.
Receives `SlotData` via `Refresh(SlotData)`.

### `TooltipUI` — MonoBehaviour (`TooltipUI.cs`)

**Mouse-following tooltip.** No `Update()` — position is updated by
`InventoryUI.OnDragUpdate` calling `TooltipUI.UpdatePosition()`.

### `ItemIconHelper` — Static class (`ItemIconHelper.cs`)

Converts `Texture2D` → `Sprite` for uGUI `Image` components. Caches results.

### `UIColors` — Static class (`Assets/Scripts/UI/UIColors.cs`)

All color tokens. Never hardcode colors.

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

### Crafting
```
RecipeRowUI click → ICraftingService.Craft(recipe)
  → CraftingSystem.Craft(recipe)
    → IInventoryService.RemoveItems() per ingredient
    → IInventoryService.AddItem() for result
    → OnSlotChanged fires per affected slot → UI auto-refreshes
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
| Crafting | C | PlayerInventory | `OnCrafting` → fires `OnCraftingToggled` |
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
| `SlotData.cs` | Shared | Struct | Immutable slot snapshot + event payload |
| `ScriptableObjects/Item.cs` | Data | ScriptableObject | Item type definition |
| `ScriptableObjects/ItemStack.cs` | Data | Serializable class | Runtime stack instance |
| `ScriptableObjects/Recipe.cs` | Data | ScriptableObject | Crafting recipe definition |
| `Inventory.cs` | Data | Plain C# class | Slot grid logic |
| `PlayerInventory.cs` | Data | MonoBehaviour | Implements IInventoryService, owns Inventory |
| `PlayerHand.cs` | Data | MonoBehaviour | In-hand item visual |
| `ItemStackDrop.cs` | Data | MonoBehaviour | World-dropped item |
| `CraftingSystem.cs` | Crafting | Plain C# class | Implements ICraftingService |
| `UI/Inventory/InventoryUI.cs` | UI | MonoBehaviour | Panel controller + drag-and-drop |
| `UI/Inventory/CraftingUI.cs` | UI | MonoBehaviour | Crafting panel, creates CraftingSystem |
| `UI/Inventory/RecipeRowUI.cs` | UI | MonoBehaviour | Single recipe row |
| `UI/Inventory/ItemSlotUI.cs` | UI | MonoBehaviour | Single slot (grid + hotbar row) |
| `UI/Inventory/HotbarUI.cs` | UI | MonoBehaviour | Standalone hotbar controller |
| `UI/Inventory/HotbarSlotUI.cs` | UI | MonoBehaviour | Single standalone hotbar slot |
| `UI/Inventory/TooltipUI.cs` | UI | MonoBehaviour | Mouse-following tooltip |
| `UI/Inventory/ItemIconHelper.cs` | UI | Static utility | Texture2D → Sprite cache |
| `UI/UIColors.cs` | UI | Static constants | Color tokens |

---

## Key Dependencies & Must-Knows

- **UI depends only on interfaces** (`IInventoryService`, `ICraftingService`), never on `PlayerInventory`, `Inventory`, or `CraftingSystem` directly.
- **`SlotData` is immutable.** UI receives snapshots via events and `Refresh(SlotData)` — never holds references to live `ItemStack` objects.
- **No `Update()` in any inventory/UI script.** All input is event-driven via Input Actions; pointer events use EventSystem handlers.
- **`CraftingSystem` is a plain C# class**, not a MonoBehaviour. Created by `CraftingUI` and disposed in `OnDestroy()`.
- **`PlayerInventory` is the `IInventoryService` implementor.** It wraps `Inventory.OnSlotChanged(int)` into rich `SlotChangedEvent` payloads.
- **Crouch is now Left Ctrl**, C is Crafting toggle.
- **`Item.Icon` must have Read/Write Enabled** in texture import settings.
- **`PlayerHand` is a singleton** (`PlayerHand.Instance`).
- **`Add()` fills hotbar first.** `PlayerInventory.Add()` passes `emptySlotStart = HotbarStartIndex`.

---

## How to Extend

### Add a new UI panel (equipment, chest, shop)
1. Create a new MonoBehaviour that takes `IInventoryService` (via `[SerializeField] PlayerInventory`)
2. Subscribe to `OnSlotChanged` for reactive updates
3. Call service commands (`Swap`, `TryMerge`, etc.) — never access `Inventory` directly

### Add a new item
1. Create → Scriptable Objects → Item, fill fields (Icon must have Read/Write ON)
2. Done — system picks it up automatically

### Add a new crafting recipe
1. Create → Scriptable Objects → Recipe, fill fields
2. Add to `CraftingUI.m_recipes` array in the InventoryCanvas prefab

### Add right-click actions
Add `IPointerClickHandler` to `ItemSlotUI`, check for `InputButton.Right`,
call a new method on `InventoryUI`.

### Make a usable held item
Attach a component implementing `IHandItem` to the item's Prefab root.
