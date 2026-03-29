# Inventory System Architecture

> Reference document for the GlowCore inventory UI system.
> Last updated: 2026-03-28 (crafting panel: Recipe SO, CraftingUI, RecipeRowUI; Inventory.CountItem/RemoveItems/CanAccept)

---

## Overview

The inventory system has two layers: a **data layer** (pure C#, no UI) and a **UI layer** (uGUI).
The data layer manages items, stacks, and slots. The UI layer reads from the data layer and reacts
to change events — it never modifies item data directly, only calls data-layer methods.

```
┌─────────────────────────────────────────────────────────────┐
│                        DATA LAYER                           │
│                                                             │
│  Item (SO) ←── ItemStack (SO) ←── Inventory (C#)           │
│                                         ↑                   │
│                                  PlayerInventory (MB)       │
│                                         ↓                   │
│                                   PlayerHand (MB)           │
└───────────────────────┬─────────────────────────────────────┘
                        │ events: OnSlotChanged,
                        │         OnInventoryToggled,
                        │         OnHotbarSelectionChanged
┌───────────────────────▼─────────────────────────────────────┐
│                        UI LAYER                             │
│                                                             │
│  InventoryUI ──► ItemSlotUI (×32)                           │
│                  TooltipUI                                   │
│                  CraftingUI ──► RecipeRowUI (×N)            │
│  HotbarUI ────► HotbarSlotUI (×8)                           │
│                  ItemIconHelper (static)                     │
│                  UIColors (static)                           │
│                  UITiming (static)                           │
└─────────────────────────────────────────────────────────────┘
```

---

## Data Layer

### `Item` — ScriptableObject (`Assets/Scripts/ScriptableObjects/Item.cs`)

The definition of an item type. Created as `.asset` files in the Unity Editor.

| Field          | Type        | Purpose                                         |
|----------------|-------------|-------------------------------------------------|
| `Name`         | string      | Display name shown in tooltip                    |
| `Description`  | string      | Tooltip description (TextArea, can be empty)     |
| `Prefab`       | GameObject  | 3D model used for world drops and in-hand visual |
| `Icon`         | Texture2D   | Icon used in UI slots and drag cursor            |
| `DropScale`    | float       | Scale multiplier for world-drop visuals (×Prefab)|
| `InHandScale`  | float       | Scale multiplier when item is held in hand       |
| `MaxStack`     | int         | Maximum items per stack (default 99, min 0)      |

### `ItemStack` — ScriptableObject (`Assets/Scripts/ScriptableObjects/ItemStack.cs`)

A runtime instance representing "X amount of item Y" in a single slot. Created via
`ItemStack.Create(item, amount)` or `ScriptableObject.CreateInstance<ItemStack>()` — never saved as assets.

| Member          | Purpose                                                  |
|-----------------|----------------------------------------------------------|
| `Item`          | Which item type this stack holds (null = empty slot)     |
| `Amount`        | How many                                                 |
| `Valid`         | True if Item != null and Amount != 0                     |
| `Create(Item, int)` | Static factory: creates and returns a new instance   |
| `Add(int)`      | Add amount, clamped to MaxStack. Returns amount added    |
| `Add(ItemStack)`| Merge another stack of the same item into this one       |
| `Set(ItemStack)`| Copy another stack's data, zeroing the source's amount   |
| `Set(Item, int)`| Directly set item and amount                             |

### `Recipe` — ScriptableObject (`Assets/Scripts/ScriptableObjects/Recipe.cs`)

Defines a crafting recipe. Created as `.asset` files in the Unity Editor.

| Field         | Type            | Purpose                              |
|---------------|-----------------|--------------------------------------|
| `Name`        | string          | Display name shown in the recipe row |
| `Ingredients` | Ingredient[]    | Array of required items and amounts  |
| `ResultItem`  | Item            | The item produced by crafting        |
| `ResultAmount`| int             | How many of the result item (min 1)  |

**`Ingredient`** (nested serializable struct): `Item` + `Amount` (min 1).

### `Inventory` — Serializable class (`Assets/Scripts/Inventory.cs`)

A flat array of `ItemStack` slots organized as a 2D grid (width × height).
This is **not** a MonoBehaviour — it's a plain C# class owned by `PlayerInventory`.

| Member                      | Purpose                                                           |
|-----------------------------|-------------------------------------------------------------------|
| `Width`, `Height`, `Size`   | Grid dimensions and total slot count                              |
| `OnSlotChanged` (event)     | Fires with the flat index whenever a slot's data changes          |
| `this[x, y]`                | 2D indexer for grid access                                        |
| `GetSlot(int)`              | Access by flat index (0 to Size-1)                                |
| `AddItems(stack, emptySlotStart=0)` | Stack onto existing same-item slots first, then first empty slot at or after `emptySlotStart` (wraps around). Returns false if no space. |
| `Swap(a, b)`                | Swap contents of two slots by flat index                          |
| `TryMerge(src, dst)`        | Stack src onto dst if same item type. Returns true if fully merged|
| `GetSlotWithItem(Item)`     | Returns `(ItemStack slot, int index)` tuple — first match, or `(null, -1)` |
| `CountItem(Item)`           | Sums total amount of a given item across all slots (pure query)       |
| `RemoveItems(Item, int)`    | Consumes items across slots, fires `OnSlotChanged` per affected slot. Returns amount removed. Clears slot via `Set(null, 0)` when depleted. |
| `CanAccept(Item, int)`      | Checks if inventory has enough space (partial stacks + empty slots) for the given item and amount |
| `GetFirstEmptySlot()`       | Returns `Vector2Int?` of first empty slot in row-major order, or null |
| `NotifySlotChanged(int)`    | Fires `OnSlotChanged` for the given flat index. Use when slot data is mutated outside of `Inventory`'s own methods. |

**Flat index layout** (4 columns × 8 rows = 32 slots):
```
Row 0:  [ 0][ 1][ 2][ 3]     ← upper inventory
Row 1:  [ 4][ 5][ 6][ 7]
Row 2:  [ 8][ 9][10][11]
Row 3:  [12][13][14][15]
Row 4:  [16][17][18][19]
Row 5:  [20][21][22][23]
Row 6:  [24][25][26][27]     ← hotbar (last 8 slots, shown as HotbarRow in panel)
Row 7:  [28][29][30][31]     ←
```

### `PlayerInventory` — MonoBehaviour (`Assets/Scripts/PlayerInventory.cs`)

Lives on the Player GameObject. Owns the `Inventory` instance and handles input (TAB to toggle,
scroll wheel to change hotbar selection). Also holds a reference to `PlayerHand` to update the
held-item visual whenever the hotbar selection changes.

| Member                      | Purpose                                                    |
|-----------------------------|------------------------------------------------------------|
| `kColumns` (4)              | Grid width                                                  |
| `kTotalRows` (8)            | Grid height                                                 |
| `kHotbarSlots` (8)          | Number of hotbar slots                                      |
| `Inventory`                 | The Inventory instance (created in Awake)                   |
| `HotbarStartIndex`          | Flat index where hotbar begins (Size - kHotbarSlots = 24)  |
| `UpperSlotCount`            | Number of non-hotbar slots (24)                            |
| `IsOpen`                    | Whether the inventory panel is currently visible            |
| `SelectedHotbarIndex`       | Currently active hotbar slot (0–7)                          |
| `OnSlotChanged` (via Inv)   | Propagated from Inventory                                  |
| `OnInventoryToggled`        | Fires when inventory opens/closes                          |
| `OnHotbarSelectionChanged`  | Fires when active hotbar slot changes                      |
| `Add(ItemStack)`            | Calls `Inventory.AddItems(stack, HotbarStartIndex)` — fills hotbar slots first, overflows to upper grid |
| `ConsumeHandItem()`         | Clears the currently selected hotbar slot (`Set(null, 0)`), rebuilds the hand visual, and fires `NotifySlotChanged` so all UI listeners update. Use this instead of mutating the `ItemStack` directly. |
| `SelectHotbarSlot(int)`     | Sets selected slot, fires event, and calls `PlayerHand.SetItemInHand()` |
| `GetHotbarSlot(i)`          | Get ItemStack at hotbar position i                         |
| `HotbarToInventoryIndex(i)` | Convert hotbar index → flat inventory index                |
| `InventoryToHotbarIndex(i)` | Convert flat index → hotbar index (-1 if not hotbar slot)  |
| `IsHotbarSlot(int)`         | Returns true if the flat index is in the hotbar range      |
| `ToggleInventory()`         | Toggle open/close                                          |
| `SetInventoryOpen(bool)`    | Set open state explicitly (no-op if already in that state) |

**Input (new Input System):**
- `Keyboard.current.tabKey.wasPressedThisFrame` in `Update()` → toggle inventory
- `OnNext(InputValue)` / `OnPrevious(InputValue)` → scroll wheel hotbar navigation
  - `OnNext` scrolls to the **previous** slot index (decrement), `OnPrevious` scrolls to the **next** (increment). Both wrap around.

**Inspector fields:** `m_playerHand` — reference to the `PlayerHand` component.

**Important:** External systems that consume items (e.g. `GlowCoreObject`) must call `ConsumeHandItem()` rather than mutating the slot's `ItemStack` directly — direct mutation bypasses `OnSlotChanged` and leaves the hotbar UI stale.

### `PlayerHand` — MonoBehaviour (`Assets/Scripts/PlayerHand.cs`)

Manages the item physically held in the player's hand. Singleton (`s_instance`). Updated
automatically by `PlayerInventory.SelectHotbarSlot()`.

| Member                | Purpose                                                           |
|-----------------------|-------------------------------------------------------------------|
| `Instance`            | Static singleton accessor                                         |
| `ItemsInHand`         | The current ItemStack being held (mirrors selected hotbar slot)   |
| `SetItemInHand(stack)`| Destroys old visual, instantiates new one scaled by `InHandScale`; removes Rigidbody and Outline from it |
| `UpdateHandVisual()`  | Rebuilds the in-hand model from `m_itemsInHand`                   |
| `IHandItem`           | Interface (`Use(InputValue)`) that in-hand prefabs may implement. `OnUse` input callback triggers it. |

**Important:** `SetItemInHand` destroys the existing Rigidbody and Outline on the instantiated prefab so the world-pickup model works safely in-hand.

### `ItemStackDrop` — MonoBehaviour (`Assets/Scripts/ItemStackDrop.cs`)

Represents a dropped `ItemStack` in the world. Requires `SphereCollider` (trigger, radius 0.5).
Spawns up to 3 copies of the item's Prefab as child visuals, randomly spread within 0.1 units.
All colliders/rigidbodies on the visual children are disabled/kinematic.

| Member                          | Purpose                                                    |
|---------------------------------|------------------------------------------------------------|
| `Stack`                         | The ItemStack this drop represents (read-only)             |
| `Spawn(ItemDrop, Vector3)`      | Static factory: rolls random amount between Min/Max, creates GameObject + component |
| `Spawn(ItemStack, Vector3)`     | Overload: spawns with a fixed amount from an existing stack |
| `Initialize(Item, int)`         | Sets up Stack and builds visuals                           |

**World behavior:**
- Bobs up and down via `FixedUpdate()` (sine wave, random phase offset so drops don't sync).
- `OnTriggerEnter`: walks up the collider hierarchy to find a `PlayerInventory`, calls `inventory.Add(Stack)`, then destroys the GameObject.

**`ItemDrop`** (serializable, defined in `NodeData.cs`):

| Field  | Purpose                              |
|--------|--------------------------------------|
| `Item` | Which item type to drop              |
| `Min`  | Minimum random amount (inclusive)    |
| `Max`  | Maximum random amount (inclusive)    |

---

## UI Layer

All UI scripts live in `Assets/Scripts/UI/Inventory/` under the `GlowCore.UI.Inventory` namespace.

### `InventoryUI` — MonoBehaviour (`InventoryUI.cs`)

**The central controller.** Manages the inventory panel: builds the slot grid, handles
drag-and-drop, tracks hover state, and controls the cursor icon.

**Drag-and-drop flow:**
1. `OnSlotPressed(index)` → called by ItemSlotUI on pointer down (left button only)
2. `PickUpItem(index)` → ghosts the source slot, shows cursor RawImage at 75% opacity
3. `Update()` → moves cursor icon to mouse position while holding; detects left button release
4. Mouse release detected → `DropHeldItem()`
5. If hovering over a different slot with the **same item type** → `TryMerge`; otherwise → `Swap`. If same slot or no slot → no-op.
6. `CancelHeldItem()` → always called after drop. Unghosts source slot, hides cursor icon.

**Panel visibility** uses CanvasGroup (alpha/interactable/blocksRaycasts) — never `SetActive`,
so `Update()` keeps running to handle the cursor icon.

**Key fields (Inspector):**
| Field               | What to assign                                     |
|---------------------|----------------------------------------------------|
| `m_playerInventory` | Player's PlayerInventory component                 |
| `m_gridParent`      | Transform of the upper GridLayoutGroup             |
| `m_hotbarRowParent` | Transform of the hotbar HorizontalLayoutGroup      |
| `m_slotPrefab`      | The ItemSlot prefab                                |
| `m_panelCanvasGroup`| CanvasGroup on the inventory panel                 |
| `m_cursorIcon`      | RawImage for the drag ghost (direct child of Canvas root) |
| `m_parentCanvas`    | The root Canvas                                    |
| `m_tooltip`         | TooltipUI component                                |

**Public API:** `RefreshAll()` — forces a full visual refresh of all slots.

### `ItemSlotUI` — MonoBehaviour (`ItemSlotUI.cs`)

**Reusable slot component.** Used for both upper inventory slots and the hotbar row inside the
inventory panel. One instance per slot, spawned at runtime from a prefab.

**States:**
- **Normal** — icon + count visible if valid item; border = `SlotBorder`; bg = `SlotBg` or `SlotFilledBg`
- **Hovered** — border = `SlotHoverBorder`; bg tints to `SlotHoverBg` (empty) or `SlotFilledBg` (filled)
- **Ghosted** — icon + count hidden (item is being dragged from this slot)
- **Hotbar style** — border = `WoodBorderLight`; key number label visible

**Count text visibility:** hidden when `Amount == 1` (only shows for stacks of 2+).

**Inspector fields:** `m_background`, `m_icon`, `m_countText`, `m_border`, `m_keyLabel`

**Pointer events (new Input System / EventSystem):**
- `IPointerDownHandler` → tells InventoryUI to start drag (left button only)
- `IPointerEnterHandler / IPointerExitHandler` → hover visuals + tooltip show/hide

### `CraftingUI` — MonoBehaviour (`CraftingUI.cs`)

**Crafting panel controller.** Manages recipe display, crafting logic, and panel visibility.

**Key behavior:**
- Opens via a button click (`Toggle()`) or `C` key while inventory is open
- Spawns `RecipeRowUI` rows from a `Recipe[]` array assigned in the inspector
- Auto-refreshes all rows when any inventory slot changes (while visible)
- Closed automatically when the inventory closes (via `InventoryUI.OnInventoryToggled`)

**Inspector fields:**
| Field               | What to assign                                    |
|---------------------|---------------------------------------------------|
| `m_playerInventory` | Player's PlayerInventory component                |
| `m_panelCanvasGroup`| CanvasGroup on the crafting panel                 |
| `m_contentParent`   | Transform of the ScrollRect Content area          |
| `m_recipeRowPrefab` | The RecipeRow prefab                              |
| `m_recipes`         | Array of Recipe assets to display                 |

**Public API:**
| Method             | Purpose                                                        |
|--------------------|----------------------------------------------------------------|
| `Toggle()`         | Show/hide the crafting panel                                   |
| `Show()` / `Hide()`| Explicit show/hide                                             |
| `CanCraft(Recipe)` | Checks material counts AND inventory space for result          |
| `Craft(Recipe)`    | Consumes ingredients, adds result, refreshes all rows          |
| `GetItemCount(Item)`| Returns total count of an item in the inventory               |

### `RecipeRowUI` — MonoBehaviour (`RecipeRowUI.cs`)

**Single recipe row.** Shows result icon, recipe name, ingredient requirements with have/need counts, and a CRAFT button.

**Visual states:**
- **Craftable:** green border (`UIColors.Green`), normal background, button enabled, ingredient counts green
- **Not craftable:** default border (`UIColors.SlotBorder`), dimmed background (alpha 0.35), button disabled, insufficient ingredients shown in `UIColors.MissingMat`

**Inspector fields:** `m_bgImage`, `m_borderImage`, `m_resultIcon`, `m_recipeName`, `m_ingredientParent`, `m_craftButton`

**Public API:**
| Method       | Purpose                                            |
|--------------|----------------------------------------------------|
| `Initialize(CraftingUI, Recipe)` | Sets up static content, builds ingredient labels |
| `Refresh()`  | Updates have/need counts, visual state, button interactability |

### `HotbarUI` — MonoBehaviour (`HotbarUI.cs`)

**Always-visible hotbar** at the bottom of the screen. Spawns 8 `HotbarSlotUI` instances.
Reads from the same inventory data as the inventory panel's hotbar row.

**Behavior:**
- Number keys 1–8 select a hotbar slot (via `Keyboard.current[Key.Digit1 + i]`)
- When inventory opens → dims to 35% alpha, non-interactable, blocks no raycasts
- When inventory closes → restores full alpha and interactivity
- Listens to `OnSlotChanged` and maps flat indices to hotbar indices for targeted refresh

### `HotbarSlotUI` — MonoBehaviour (`HotbarSlotUI.cs`)

**Single slot in the standalone hotbar.** Initialized with a `PlayerInventory` reference and
a hotbar index. Reads from `PlayerInventory.GetHotbarSlot(i)`.

- Shows item icon, count (hidden when Amount == 1), key number label
- `SetSelected(bool)` highlights the slot with `SelectedBorder` color
- Click selects the slot via `PlayerInventory.SelectHotbarSlot()`
- No drag-and-drop — that is handled exclusively inside the inventory panel

### `TooltipUI` — MonoBehaviour (`TooltipUI.cs`)

**Mouse-following tooltip panel.** Shows item name and description on hover.

- Pivot set to (0, 1) → top-left corner anchors to mouse; tooltip expands right and down
- `blocksRaycasts = false` → doesn't intercept pointer events from slots underneath
- Offset: 14px right, 14px down from cursor
- Shows on slot hover via `Show(ItemStack)`, hides on exit or when dragging starts
- **Layout:** fixed width (~180 px) with `VerticalLayoutGroup` (8 px padding, 4 px spacing) + `ContentSizeFitter` (Vertical Fit = Preferred Size). Both TMP children have word wrap enabled and Overflow = Overflow so long descriptions wrap and the panel grows in height automatically.

### `ItemIconHelper` — Static class (`ItemIconHelper.cs`)

Converts `Texture2D` → `Sprite` for uGUI `Image` components (used by slot icons).
Caches results so each texture is only converted once.

**Important:** Requires the texture to have **Read/Write Enabled** in its import settings.
The drag cursor uses `RawImage` directly (no conversion needed), which is why `m_cursorIcon`
is a `RawImage` rather than an `Image`.

### `UIColors` — Static class (`Assets/Scripts/UI/UIColors.cs`)

All color tokens from the approved HTML prototype (v4). Never hardcode colors — always use these.

| Token             | Usage                                     |
|-------------------|-------------------------------------------|
| `WoodDark`        | Panel backgrounds                         |
| `WoodBorder`      | Panel borders                             |
| `WoodBorderLight` | Inner borders, hotbar slot idle border    |
| `Accent`          | Gold — selected items, titles             |
| `AccentDim`       | Dimmed gold — labels, hotbar key numbers  |
| `SlotBg`          | Empty slot background                     |
| `SlotBorder`      | Normal slot border                        |
| `SlotHoverBg`     | Slot background on hover (empty slot)     |
| `SlotHoverBorder` | Slot border on hover                      |
| `SlotFilledBg`    | Slot background when containing an item   |
| `SelectedBorder`  | Active hotbar slot border (full gold)     |

### `UITiming` — Static class (`Assets/Scripts/UI/UITiming.cs`)

Animation duration constants for DOTween. Not yet wired into the inventory (future polish).

---

## Unity Hierarchy

```
InventoryCanvas (Canvas — Screen Space Overlay, CanvasScaler 854×480)
  │
  ├── InventoryPanel (CanvasGroup + VerticalLayoutGroup)
  │     ├── UpperGrid (GridLayoutGroup: 40×40 cells, 4px spacing, 4 columns)
  │     │     └── [24 ItemSlotUI spawned at runtime — indices 0–23]
  │     │
  │     ├── Spacer (empty RectTransform, ~12px height)
  │     │
  │     └── HotbarRow (HorizontalLayoutGroup: 4px spacing)
  │           └── [8 ItemSlotUI spawned at runtime, hotbar-styled — indices 24–31]
  │
  ├── CraftTabButton (Button — anchored top-right of inventory panel, onClick → CraftingUI.Toggle())
  │
  ├── CraftingPanel (CraftingUI + CanvasGroup, positioned right of inventory)
  │     ├── Title (TMP — "CRAFTING", Cinzel Bold, AccentDim)
  │     └── ScrollView (ScrollRect, vertical only)
  │           └── Viewport (Mask + Image)
  │                 └── Content (VerticalLayoutGroup spacing 4 + ContentSizeFitter)
  │                       └── [RecipeRowUI instances spawned at runtime]
  │
  ├── HotbarPanel (CanvasGroup, anchored bottom-center)
  │     └── HotbarGrid (HorizontalLayoutGroup: 4px spacing)
  │           └── [8 HotbarSlotUI spawned at runtime]
  │
  ├── CursorIcon (RawImage — 40×40, raycast OFF, direct child of Canvas)
  │
  └── Tooltip (CanvasGroup — raycast OFF, interactable OFF)
        ├── TooltipName (TMP — Cinzel Bold 12px, color: Accent)
        └── TooltipDesc (TMP — Nunito 10px, color: WhiteDim)
```

### ItemSlot Prefab Structure
```
ItemSlot (Image = border, 40×40)
  ├── Background (Image = fill, stretch inset 1px)
  ├── Icon (Image = item sprite, 32×32 centered, raycast OFF)
  ├── Count (TMP — bottom-right, Nunito Bold 9px)
  └── KeyLabel (TMP — top-left, Nunito Bold 8px, hidden by default)
```

### HotbarSlot Prefab Structure
```
HotbarSlot (Image = border, 40×40)
  ├── Background (Image = fill)
  ├── Icon (Image = item sprite, 32×32 centered, raycast OFF)
  ├── Count (TMP — bottom-right, Nunito Bold 9px)
  └── KeyLabel (TMP — top-left, 8px, WhiteFaint)
```

---

## Data Flow

### Adding an item (e.g. player picks up a world drop)
```
ItemStackDrop.OnTriggerEnter()
  → PlayerInventory.Add(stack)
      → Inventory.AddItems(stack, emptySlotStart: HotbarStartIndex=24)
           fills existing partial stacks first (scans all 32 slots)
           then fills empty slots starting from index 24 (hotbar first),
           wrapping to the upper grid if hotbar is full
        → modifies ItemStack data
        → fires OnSlotChanged(flatIndex)
             → InventoryUI.OnSlotDataChanged(index) → ItemSlotUI.Refresh()
             → HotbarUI.OnSlotDataChanged(index) → HotbarSlotUI.Refresh()
```

### Drag-and-drop (same item type → merge, different → swap)
```
User presses left mouse on slot 5 (has Wood ×24):
  ItemSlotUI.OnPointerDown → InventoryUI.OnSlotPressed(5)
    → PickUpItem(5): ghost slot 5, show RawImage cursor with Wood icon at 75% alpha

User drags to slot 12 (has Wood ×10):
  ItemSlotUI.OnPointerEnter(12) → InventoryUI.m_hoveredSlotIndex = 12

User releases mouse:
  InventoryUI.Update() detects leftButton.wasReleasedThisFrame
    → DropHeldItem():
        same item type → Inventory.TryMerge(5, 12)
          → OnSlotChanged(5) + OnSlotChanged(12) → both slots refresh
    → CancelHeldItem(): unghost slot 5, hide cursor icon

If slot 12 had a different item:
    → Inventory.Swap(5, 12) → both slots refresh
```

### Feeding wood to the GlowCore
```
GlowCoreObject.Interact()
  reads woodAmount from PlayerHand.Instance.ItemsInHand
  → PlayerInventory.ConsumeHandItem()
      → GetHotbarSlot(m_selectedHotbarIndex).Set(null, 0)
      → UpdatePlayerHand() → PlayerHand.SetItemInHand(null) → destroys hand visual
      → Inventory.NotifySlotChanged(selectedIndex)
           → HotbarUI.OnSlotDataChanged(index) → HotbarSlotUI.Refresh()
           → InventoryUI.OnSlotDataChanged(index) → ItemSlotUI.Refresh()
  → Fire.FeedWood(woodAmount)
```

### Crafting an item
```
User opens inventory (TAB), then opens crafting panel (C key or Craft button):
  CraftingUI.Toggle() → SetVisible(true) → RefreshAll()
    → each RecipeRowUI.Refresh():
        checks CanCraft(recipe) → green border + enabled button, or dimmed + disabled

User clicks CRAFT on a recipe row:
  RecipeRowUI.OnCraftClicked() → CraftingUI.Craft(recipe)
    → CanCraft(recipe): checks CountItem() for each ingredient + CanAccept() for result
    → For each ingredient: Inventory.RemoveItems(item, amount)
         → fires OnSlotChanged per affected slot
    → ItemStack.Create(resultItem, resultAmount)
    → Inventory.AddItems(result)
         → fires OnSlotChanged for the target slot
    → CraftingUI.RefreshAll() → all rows re-evaluate CanCraft and update visuals
```

### Opening/closing inventory
```
User presses TAB:
  PlayerInventory.Update() → ToggleInventory()
    → OnInventoryToggled(true)
         → InventoryUI: sets CanvasGroup alpha=1, interactable=true, blocksRaycasts=true
         → HotbarUI: dims to 35% alpha, interactable=false, blocksRaycasts=false

User presses TAB again:
  → OnInventoryToggled(false)
       → InventoryUI: alpha=0, interactable=false; cancels any held item; hides CraftingUI
       → HotbarUI: restores full alpha and interactivity
```

### Changing selected hotbar slot
```
User presses key 3 (or scrolls mouse wheel):
  HotbarUI.Update() → PlayerInventory.SelectHotbarSlot(2)
    → m_selectedHotbarIndex = 2
    → OnHotbarSelectionChanged(2)
         → HotbarUI.UpdateSelection() → HotbarSlotUI.SetSelected()
    → PlayerHand.SetItemInHand(GetHotbarSlot(2))
         → Destroys old in-hand model
         → Instantiates Item.Prefab, scaled by Item.InHandScale
         → Removes Rigidbody and Outline from the new visual
```

---

## File Index

| File | Layer | Type | Purpose |
|------|-------|------|---------|
| `ScriptableObjects/Item.cs` | Data | ScriptableObject | Item type definition |
| `ScriptableObjects/ItemStack.cs` | Data | ScriptableObject | Runtime stack instance |
| `ScriptableObjects/NodeData.cs` | Data | ScriptableObject | Node resource config; defines `ItemDrop` and `UsableTool` |
| `ScriptableObjects/Recipe.cs` | Data | ScriptableObject | Crafting recipe definition |
| `Inventory.cs` | Data | Plain C# class | Slot grid with swap/merge/count/remove/events |
| `PlayerInventory.cs` | Data | MonoBehaviour | Owns Inventory, handles input, coordinates PlayerHand |
| `PlayerHand.cs` | Data | MonoBehaviour (Singleton) | Manages in-hand item visual; implements `IHandItem` dispatch |
| `ItemStackDrop.cs` | Data | MonoBehaviour | World-dropped item with bobbing animation and auto-pickup |
| `UI/Inventory/InventoryUI.cs` | UI | MonoBehaviour | Panel controller + drag-and-drop + crafting wire |
| `UI/Inventory/CraftingUI.cs` | UI | MonoBehaviour | Crafting panel controller |
| `UI/Inventory/RecipeRowUI.cs` | UI | MonoBehaviour | Single recipe row display |
| `UI/Inventory/ItemSlotUI.cs` | UI | MonoBehaviour | Single slot (upper grid + hotbar row in panel) |
| `UI/Inventory/HotbarUI.cs` | UI | MonoBehaviour | Standalone hotbar controller |
| `UI/Inventory/HotbarSlotUI.cs` | UI | MonoBehaviour | Single standalone hotbar slot |
| `UI/Inventory/TooltipUI.cs` | UI | MonoBehaviour | Mouse-following tooltip |
| `UI/Inventory/ItemIconHelper.cs` | UI | Static utility | Texture2D → Sprite cache |
| `UI/UIColors.cs` | UI | Static constants | Color tokens from design system |
| `UI/UITiming.cs` | UI | Static constants | Animation durations (future) |

---

## Key Dependencies & Must-Knows

- **`Item.Icon` must have Read/Write Enabled** in texture import settings — `ItemIconHelper` calls `Sprite.Create` which requires CPU-readable pixel data. The drag cursor (`RawImage`) bypasses this requirement.
- **`PlayerHand` is a singleton** (`PlayerHand.Instance`). Only one may exist in the scene. It is wired into `PlayerInventory` via `[SerializeField] private PlayerHand m_playerHand`.
- **`Add()` fills hotbar first.** `PlayerInventory.Add()` passes `emptySlotStart = HotbarStartIndex`. Partial-stack merging still scans all 32 slots (indices 0–31), but empty-slot overflow starts at index 24. Items overflow to the upper grid only when the hotbar is full.
- **`ItemDrop` lives in `NodeData.cs`**, not in `ItemStackDrop.cs`. Any code that spawns drops uses `NodeData.ItemDrops[]` as the source.
- **`IHandItem` interface** — if a Prefab's root GameObject has a component implementing `IHandItem`, pressing the Use input action calls `IHandItem.Use(InputValue)` on it. This is how items have active effects when held.
- **Inventory toggle input** uses the new Input System (`Keyboard.current.tabKey`), not legacy `Input.GetKeyDown`. Hotbar scroll also uses new Input System callbacks (`OnNext`/`OnPrevious` on the PlayerInput component).
- **Scroll wheel direction is inverted from slot order:** `OnNext` (scroll down) decrements the slot index; `OnPrevious` (scroll up) increments it.
- **Slot count text is hidden for single items** (`Amount == 1`). Count only appears for stacks of 2 or more.

---

## How to Extend

### Add a new item
1. Right-click in Project → Create → Scriptable Objects → Item
2. Fill in Name, Description, Icon (Texture2D with **Read/Write ON**), Prefab, MaxStack
3. Set `DropScale` for world drop size, `InHandScale` for hand visual size
4. Done — the inventory system picks it up automatically

### Change grid size
Edit `kColumns` and `kTotalRows` in `PlayerInventory.cs`. Update the GridLayoutGroup column
count in the InventoryCanvas prefab to match. Everything else adapts automatically.

### Change hotbar size
Edit `kHotbarSlots` in `PlayerInventory.cs`. The last N slots become the hotbar. Update
`HotbarUI` slot prefab count and key handling in `HotbarUI.Update()` if count exceeds 9.

### Add right-click actions (use, drop, split)
Add an `IPointerClickHandler` to `ItemSlotUI` and check for `PointerEventData.InputButton.Right`.
Call a new method on `InventoryUI` such as `OnSlotRightClicked(index)`.

### Add a new crafting recipe
1. Right-click in Project → Create → Scriptable Objects → Recipe
2. Fill in Name, Ingredients (Item + Amount pairs), ResultItem, ResultAmount
3. Add the Recipe asset to `CraftingUI.m_recipes` array in the InventoryCanvas prefab
4. Done — the crafting panel picks it up automatically on next Start()

### Add equipment slots
Create `EquipmentUI` with specialized `ItemSlotUI` instances that validate item type before
accepting a drop. Add equipment data fields to `PlayerInventory` or a new `PlayerEquipment` component.

### Make a usable held item
Attach a component implementing `IHandItem` to the item's Prefab root. `PlayerHand.OnUse()`
(driven by the Use input action) will forward the call automatically when that item is held.
