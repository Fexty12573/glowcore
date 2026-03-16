# GC-46: GlowCore Expands World

## Branch
`feature/gc-46-glowcore-expands-world`

---

## Architecture Overview

### Namespaces
- `GlowCore.World` — Grid system, nodes, fire
- `GlowCore.Player` — Inventory interface, player interaction

### New Files

| File | Namespace | Description |
|------|-----------|-------------|
| `Assets/Scripts/WorldGrid.cs` | `GlowCore.World` | Grid system — 2D array of nodes, border management, expansion |
| `Assets/Scripts/Node.cs` | `GlowCore.World` | Marker component for any object occupying a tile |
| `Assets/Scripts/Fire.cs` | `GlowCore.World` | GlowCore fire — receives wood, triggers grid expansion |
| `Assets/Scripts/IInventory.cs` | `GlowCore.Player` | Interface for inventory (to be implemented by another team member) |
| `Assets/Scripts/PlayerInteraction.cs` | `GlowCore.Player` | Raycast-based mouse interaction (hover + hold E) |
| `Assets/Scripts/DummyInventory.cs` | `GlowCore.Player` | **TEMPORARY** — Fake inventory with 10 wood for testing |

### Class Relationships
```
PlayerInteraction --raycast--> Fire --calls--> WorldGrid.Expand()
       |                        |
       v                        v
  IInventory                   Node
  (DummyInventory)         (on prefabs)
```

---

## Scene Setup

### WorldGrid GameObject
- Attach `WorldGrid` script
- Assign 4 border transforms (BorderNorth/South/East/West) in Inspector

### GlowCore Prefab
- `Node` component (auto-added by RequireComponent)
- `Collider` component (SphereCollider — needed for raycast)
- `Fire` component (finds WorldGrid automatically via FindFirstObjectByType)

### PlayerController (child of Player)
- `PlayerInteraction` — assign MainCamera to `m_camera` slot
- `DummyInventory` — **TEMPORARY**, remove when real inventory is implemented

### Other Node Prefabs (Tree, etc.)
- Add `Node` component to each prefab that occupies a tile

---

## Requirements Status

### Done
- [x] The world is a grid (WorldGrid with 2D Node array)
- [x] 2D Array of all tiles where Nodes are saved at corresponding index
- [x] Player can't physically cross the border (4 border colliders positioned by WorldGrid)
- [x] At start, border has size 5x5 (kInitialSize = 5)
- [x] There is a fire / GlowCore (Fire component on GlowCore prefab)
- [x] Border expands when fire grows (Fire.FeedWood -> WorldGrid.Expand)
- [x] Player can click on fire to transfer wood (PlayerInteraction raycast + hold E)

### Blocked / Waiting
- [ ] Real inventory implementation (another team member) — replace DummyInventory with actual IInventory implementation
- [ ] Wood pickup mechanic — how the player gets wood into the inventory (not part of this ticket)

---

## Things to Clean Up

### Temporary Code — Remove Before Merge
1. **`DummyInventory.cs`** — Replace with real inventory implementation
2. **`Debug.Log` statements in `PlayerInteraction.cs`** — Lines 20, 24, 29, 36, 41, 48. Remove all debug logs
3. **`DebugExpand` ContextMenu in `WorldGrid.cs`** — Lines 118-122. Remove the debug expand method

### Design Decisions to Revisit
- **Expand per wood ratio** (`Fire.m_expandPerWood`) — Currently 1 tile per wood. Adjustable in Inspector
- **Interact range** (`PlayerInteraction.m_interactRange`) — Currently 5 units. May need tuning
- **Interact input** — Currently "Hold E" (Interact action has Hold interaction). Confirm if this is intended or should be a press
- **Border height/thickness** — `kBorderHeight = 5f`, `kBorderThickness = 0.001f` in WorldGrid. Adjust if needed

---

## Key Rules / Patterns

- **Node definition**: A Node is an object occupying a Tile. Not every Tile has a Node, but every Node belongs to a Tile.
- **Grid is centered at origin (0,0,0)** where the GlowCore sits
- **1x1 tile size** matching the visual grid on the ground
- **Nodes are auto-registered** on scene load via `FindObjectsByType<Node>()`
- **Fire finds WorldGrid automatically** via `FindFirstObjectByType<WorldGrid>()` (can't use SerializeField due to prefab/scene reference limitation)
- **IInventory interface** is the contract — whoever implements the real inventory must implement `WoodCount`, `AddWood(int)`, and `RemoveAllWood()`
