# Inventory System Architecture

> Reference document for the GlowCore inventory UI system.
> Last updated: 2026-04-09 (CraftingTableInteractable rename fix; IInventoryService extended with IsCraftingTableOpen/OnCraftingTableToggled/OnCloseUIRequested; CraftingTableUI shows all global recipes; HotbarUI dims on crafting table open; NodeActionSystem guards on IsCraftingTableOpen; Escape closes UI)

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
| `IsCraftingTableOpen` | Property | Whether the crafting table panel is open |
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
| `RemoveItems(Item, int)` | Command | Remove items across slots, returns amount removed |
| `AddItem(Item, int)` | Command | Add items to inventory |
| `OnSlotChanged` | Event | `Action<SlotChangedEvent>` — fires with slot index + data snapshot |
| `OnHotbarSelectionChanged` | Event | `Action<int>` — fires with new hotbar index |
| `OnInventoryToggled` | Event | `Action<bool>` — fires with open state |
| `OnCraftingToggled` | Event | `Action` — fires when C key pressed while inventory is open |
| `OnCraftingTableToggled` | Event | `Action<bool>` — fires when crafting table is opened/closed |
| `OnCloseUIRequested` | Event | `Action` — fires on Escape; closes any open UI (inventory or crafting table) |

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
Dims (alpha 0.35, non-interactive) when either the inventory **or the crafting table** is open — subscribes to both `OnInventoryToggled` and `OnCraftingTableToggled`.

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

---

## Key Dependencies & Must-Knows

- **UI depends only on interfaces** (`IInventoryService`, `ICraftingService`), never on `PlayerInventory`, `Inventory`, or `CraftingSystem` directly.
- **`SlotData` is immutable.** UI receives snapshots via events and `Refresh(SlotData)` — never holds references to live `ItemStack` objects.
- **No `Update()` in any inventory/UI script.** All input is event-driven via Input Actions; pointer events use EventSystem handlers.
- **`CraftingSystem` is a plain C# class**, not a MonoBehaviour. Created by `CraftingUI` and `CraftingTableUI`, each owning their own instance. Both are disposed in `OnDestroy()`.
- **`PlayerInventory` is the `IInventoryService` implementor.** It wraps `Inventory.OnSlotChanged(int)` into rich `SlotChangedEvent` payloads.
- **Crouch is now Left Ctrl**, C is Crafting toggle.
- **`Item.Icon` is a `Sprite`.** Import textures with Texture Type = Sprite (2D and UI). Read/Write is **not** required. The `RawImage` drag cursor accesses `sprite.texture` directly — this is intentional.
- **`PlayerHand` is a singleton** (`PlayerHand.Instance`). Its `m_playerInventory` field must be wired in the Inspector.
- **Hand item prefabs store `IHandItem` components disabled.** `PlayerHand.UpdateHandVisual()` enables them. Do not enable them in the prefab — that breaks the lifecycle ordering.
- **`IPlayerInventoryAware` is called automatically** by `PlayerHand.UpdateHandVisual()` — no manual wiring needed beyond prefab setup.
- **`Add()` fills hotbar first.** `PlayerInventory.Add()` passes `emptySlotStart = HotbarStartIndex`.
- **`NodeActionSystem` blocks interaction while crafting table is open.** It guards on both `IsOpen` and `IsCraftingTableOpen` — hovering and interacting with world nodes is disabled while any UI is open.
- **`CraftingTableUI` reads all global recipes** (no filter). `CraftingUI` is the one that filters to `RequiresCraftingTable == false`. If a recipe should only appear at the bench, set `RequiresCraftingTable = true` — it will be excluded from `CraftingUI` automatically.
- **`CraftingTableInteractable` owns the open/close state** of the bench. It calls `PlayerInventory.SetCraftingTableOpen(bool)` — never set `IsCraftingTableOpen` from anywhere else.
- **Escape fires `OnCloseUIRequested` unconditionally**, even when inventory is not open. `CraftingTableInteractable` uses this to close the bench UI without needing an inventory open state check.

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
