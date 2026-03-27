# UI Workflow — GlowCore

> Read this file at the start of any session that touches UI code or UI-adjacent data.

---

## 1. Pre-work: read before you write

Before modifying any UI file or any data-layer file that UI depends on, read:

- [Reference/INVENTORY_ARCHITECTURE.md](INVENTORY_ARCHITECTURE.md) — authoritative overview of
  the full inventory/hotbar system: classes, events, data flow, hierarchy, and must-knows.

If a relevant script is not in the architecture doc, read it directly before making changes.

---

## 2. Namespace & assembly

| Scope | Namespace | Assembly |
|---|---|---|
| All UI scripts | `GlowCore.UI.Inventory` (inventory) or `GlowCore.UI` (shared) | `Gameplay.asmdef` |
| Data layer | *(no namespace for top-level MBs)* / `ScriptableObjects` for SOs | `Gameplay.asmdef` |

`Gameplay.asmdef` references: **TextMeshPro**, **Unity.InputSystem**. Do not add bare `using TMPro` or `using UnityEngine.InputSystem` without confirming the asmdef has the reference.

---

## 3. Colors — always use `UIColors`

Never hardcode a `Color` or hex value in UI code. Every color lives in `Assets/Scripts/UI/UIColors.cs`.

| Token | Use for |
|---|---|
| `WoodDark` | Panel backgrounds |
| `WoodBorder` | Panel/frame borders |
| `WoodBorderLight` | Inner slot borders, hotbar idle border |
| `Accent` | Gold — selected items, section titles |
| `AccentDim` | Dimmed gold — key-number labels, secondary labels |
| `SlotBg` | Empty slot background |
| `SlotBorder` | Normal slot border |
| `SlotHoverBg` | Hovered empty slot background |
| `SlotHoverBorder` | Hovered slot border |
| `SlotFilledBg` | Slot background when item is present |
| `SelectedBorder` | Active hotbar slot border |

If a new color is needed, add it to `UIColors` first — do not inline it.

---

## 4. Typography — always use these TMP settings

| Usage | Font | Size | Color token |
|---|---|---|---|
| Item name / section title | Cinzel Bold | 12 px | `Accent` |
| Body / description | Nunito Regular | 10 px | `WhiteDim` |
| Slot count | Nunito Bold | 9 px | *(white)* |
| Hotbar key label | Nunito Bold | 8 px | `AccentDim` (panel) / `WhiteFaint` (standalone) |

Do not use other fonts or sizes without a design decision.

---

## 5. Canvas & layout conventions

- **Reference resolution:** 854 × 480, Scale Mode: Scale With Screen Size, Match: 0.5
- **Render mode:** Screen Space — Overlay
- **CursorIcon** (`RawImage`) must be a **direct child of the root Canvas** and rendered last (`SetAsLastSibling` when drag starts). It uses `RawImage`, not `Image`, to avoid the Read/Write texture requirement.
- **Panel show/hide:** use `CanvasGroup` (alpha / interactable / blocksRaycasts) — **never `SetActive`** on panels that run `Update()` or need to catch input while hidden.
- **Slot size:** 40 × 40 px. Icon inside: 32 × 32, centered, `raycastTarget = false`.
- **GridLayoutGroup** for the upper inventory: cell 40 × 40, spacing 4 px, 4 columns, constraint = Fixed Column Count.
- **HorizontalLayoutGroup** for the hotbar row and standalone hotbar: spacing 4 px.

---

## 6. Input — new Input System only

- Use `Mouse.current`, `Keyboard.current`, and `InputValue` callbacks.
- Do **not** use `Input.GetKey`, `Input.GetMouseButton`, or any legacy Input API.
- Pointer events in UI use `IPointerDownHandler`, `IPointerEnterHandler`, `IPointerExitHandler` from `UnityEngine.EventSystems`.

---

## 7. Prefab locations

| Prefab | Path |
|---|---|
| ItemSlot (inventory + hotbar row) | `Assets/Prefabs/UI/ItemSlot.prefab` |
| HotbarSlot (standalone hotbar) | `Assets/Prefabs/UI/HotbarSlot.prefab` |
| InventoryCanvas (root canvas) | `Assets/Prefabs/UI/InventoryCanvas.prefab` |

Slot prefabs are instantiated at runtime by `InventoryUI` and `HotbarUI` — do not place them in the scene manually.

---

## 8. Item icons

- Icons are `Texture2D` assets. **Read/Write must be enabled** in their import settings.
- Always go through `ItemIconHelper.GetSprite(item)` when assigning to a `Image.sprite` — it caches the conversion.
- The drag cursor (`RawImage`) skips `ItemIconHelper` and uses `Texture2D` directly — this is intentional.

---

## 9. Event subscription hygiene

Any MonoBehaviour that subscribes to events in `Start()` must unsubscribe in `OnDestroy()`. Pattern:

```csharp
private void Start()   => m_playerInventory.Inventory.OnSlotChanged += OnSlotChanged;
private void OnDestroy() => m_playerInventory?.Inventory?.OnSlotChanged -= OnSlotChanged; // null-safe
```

---

## 10. After any UI change — update the architecture doc

At the end of every session where you:
- Add, remove, or rename a UI class or member
- Change how events are wired or data flows between layers
- Modify the Unity hierarchy structure (Canvas, panels, slot prefabs)
- Add a new dependency (new SO field, new MonoBehaviour reference)
- Change any behavior documented in a Data Flow section

**Update `Reference/INVENTORY_ARCHITECTURE.md`** to reflect the new state.
Keep it accurate and current — it is the single source of truth for anyone picking up this work.
