# GC-46: GlowCore Expands World

## Branch
`feature/gc-46-glowcore-expands-world`

---

## Architecture Overview

### Namespaces
- `GlowCore.World` — Grid system, nodes, fire, GlowCore object

### Files

| File | Namespace | Description |
|------|-----------|-------------|
| `Assets/Scripts/WorldGrid.cs` | `GlowCore.World` | Singleton grid system — 2D tile array, border management, tree spawning, world mode, expansion |
| `Assets/Scripts/Node.cs` | `GlowCore.World` | Base marker component for any object occupying a tile |
| `Assets/Scripts/TreeNode.cs` | `GlowCore.World` | Node subclass identifying tree objects specifically |
| `Assets/Scripts/Fire.cs` | `GlowCore.World` | GlowCore fire — receives wood, triggers grid expansion and GlowCore visual upgrade |
| `Assets/Scripts/GlowCore.cs` | `GlowCore.World` | GlowCore visual controller — progressively activates log child objects on upgrade |

### Class Relationships
```
Fire.FeedWood(amount)
    |
    +---> accumulates wood toward m_woodPerExpansion threshold
    |         for each threshold reached:
    |             WorldGrid.Instance.Expand(m_regularExpansionSize)
    |
    +---> GlowCoreObject.FeedWood(amount)
              |
              +---> ActivateLogs(amount)      (visual log reveal)
              +---> accumulates wood toward m_woodToLevelUp
              +---> returns true if leveled up
                        |
                        Fire --> WorldGrid.Instance.Expand(m_levelUpExpansionSize, isLevelUp: true)
```

---

## WorldMode Enum

Controls how the world initializes and whether expansions auto-spawn trees.

| Value | Start behaviour | On expansion |
|-------|-----------------|--------------|
| `ProceduralGeneration` | Clears only in-bounds trees, spawns `m_initialTreeCount` random trees. Designer-placed trees outside the initial grid survive until `Expand()` sweeps over them, then are destroyed and replaced by the ring spawner | Spawns trees in the new outer ring |
| `DesignedWorld` | Registers all hand-placed nodes without altering them | No auto-spawn — the designer controls all content |

---

## WorldGrid — Public API

| Method / Property | Description |
|-------------------|-------------|
| `GetNodeAt(x, z)` | Returns the Node at world position (x, z), or null if empty or out of bounds |
| `SetNodeAt(x, z, node)` | Registers (or clears) a node at world position (x, z). Returns false if out of bounds |
| `PlaceTree(x, z)` | Instantiates the tree prefab at (x, z), registers it, increments `TotalTreeCount`. Returns null if cell is occupied |
| `SpawnRandomTrees(density)` | Fills the entire current grid randomly at the given density (0–1). Skips occupied cells and the origin |
| `ClearAllTrees()` | Destroys all TreeNode instances — both those tracked in the grid array and any remaining children of the Nodes parent. Resets `TotalTreeCount` to 0 |
| `IsInBounds(index)` | Returns whether a grid-space index is within the current bounds |
| `WorldToGrid(x, z)` | Converts world position to grid array index |
| `GridToWorld(x, z)` | Converts grid array index to world position |
| `Expand(amount, isLevelUp)` | Expands the grid by `amount` tiles on all four sides. Repositions fog borders, flushes pending nodes, and (if `ProceduralGeneration`) spawns trees in the new outer ring |
| `Initialize()` | Allocates the tile array and sets the origin. Called from `Awake`; can be called externally to reset |
| `Instance` | Singleton accessor |
| `Mode` | The active `WorldMode` |
| `GridSize` | Current grid side length |
| `TotalTileCount` | Total number of tiles (`GridSize * GridSize`) |
| `TotalTreeCount` | Cumulative count of trees placed via `PlaceTree` since startup (reset by `ClearAllTrees`) |

---

## WorldGrid — Inspector Fields

### World Mode
| Field | Description |
|-------|-------------|
| `World Mode` | Selects the `WorldMode` — controls startup tree behaviour and whether expansions auto-spawn trees |
| `Initial Tree Count` | *(ProceduralGeneration only)* Exact number of trees placed on startup (min 0). Uses Fisher-Yates shuffle — count is guaranteed, placement is random |

### Grid
| Field | Description |
|-------|-------------|
| `Grid Size` | Initial grid size (default 5, equals `kInitialSize`) |

### Borders
| Field | Description |
|-------|-------------|
| `Border North/South/East/West` | Transform references for the 4 fog border wall GameObjects |

### Trees
| Field | Description |
|-------|-------------|
| `Tree Prefab` | Prefab to instantiate for trees — must have a `TreeNode` component |
| `Nodes Parent` | Transform under which spawned tree instances are parented. Falls back to WorldGrid's own transform if unassigned |

### Tree Expansion *(ProceduralGeneration only)*
| Field | Description |
|-------|-------------|
| `Trees Per Regular Expansion` | Number of trees spawned in the new outer ring on a regular expansion (default 5) |
| `Trees Per Level Up Expansion` | Number of trees spawned in the new outer ring on a level-up expansion (default 20) |

---

## Startup Sequence (`WorldGrid.Awake`)

1. **Singleton check** — destroys duplicate instances
2. **`Initialize()`** — allocates `m_tiles` array, sets origin
3. **Mode branch:**
   - `ProceduralGeneration` → `RegisterExistingNodes()`, `ClearTreesInGrid()`, `SpawnTrees(m_initialTreeCount)`
   - `DesignedWorld` → `RegisterExistingNodes()` only
4. **`LogGrid()`** — prints the full grid state to the console (node names or `.` for empty, with world Z labels and active mode)
5. **`UpdateBorders()`** — positions and scales the 4 fog border walls to match the current grid size

---

## Expansion Sequence (`WorldGrid.Expand`)

1. Allocates a new larger tile array (`m_gridSize + amount * 2` on each axis)
2. Copies existing nodes into their shifted positions (origin moves outward symmetrically)
3. Updates `m_tiles`, `m_gridSize`, `m_origin`
4. **`UpdateBorders()`** — resizes fog border walls to the new size
5. **`RegisterPendingNodes()`** — for each out-of-bounds node that is now within the expanded grid:
   - `ProceduralGeneration` + `TreeNode` → **destroyed** (the ring spawner replaces it)
   - All other nodes → registered normally
6. *(ProceduralGeneration only)* — spawns trees in the new outer ring:
   - `isLevelUp == true` → `m_treesPerLevelUpExpansion` trees
   - `isLevelUp == false` → `m_treesPerRegularExpansion` trees
   - Uses Fisher-Yates shuffle; only free cells in the new ring are candidates

---

## Border / Fog Walls

The four border walls serve as both **physics colliders** (stopping the player) and **fog walls** (visually covering the outside world).

- `kBorderFogDepth = 100f` — how far each wall extends outward from the grid edge
- The wall center is positioned at `halfSize + kBorderFogDepth / 2`, so its **inner face lands exactly at the grid edge** — the player collision boundary is unchanged
- Width of each wall is `m_gridSize + kBorderFogDepth * 2` to fully cover the corners (no gaps between adjacent walls)
- On every `Expand()` call, all four walls are repositioned and rescaled to match the new grid size

---

## Scene Setup

### WorldGrid GameObject
- Attach `WorldGrid` script
- Set `World Mode` to the desired `WorldMode`
- Assign 4 border transforms (BorderNorth/South/East/West)
- Assign `Tree Prefab` (`Assets/Prefabs/Nodes/Tree.prefab`)
- Assign `Nodes Parent` — the "Nodes" GameObject in the scene hierarchy
- *(ProceduralGeneration)* Set `Initial Tree Count` for startup trees (default 10)
- *(ProceduralGeneration)* Set `Trees Per Regular Expansion` and `Trees Per Level Up Expansion`

### GlowCore Prefab
- `Node` component — occupies the center tile (0, 0)
- `Collider` component (SphereCollider)
- `Fire` component
  - `Glow Core` — assign the `GlowCoreObject` on this same prefab
  - `Wood Per Expansion` — how many wood must be fed to trigger one regular border expansion (default 1)
  - `Regular Expansion Size` — tiles added per regular expansion (default 1)
  - `Level Up Expansion Size` — tiles added when GlowCore levels up (default 5, should be large)
- `GlowCoreObject` component
  - `Logs` — assign all log child GameObjects for this level
  - `Next Level Prefab` — the prefab to spawn when this GlowCore levels up. Each prefab only references the next one (chain, not a tree)
  - `Initial Active Logs` — logs visible at startup (0–17, default 3)
  - `Starting Level` — which level this prefab represents (set to 1 on level 1 prefab, 2 on level 2, etc.)
  - `Wood To Level Up` — wood required to advance to the next level (default 17)

### Tree Prefab (`Assets/Prefabs/Nodes/Tree.prefab`)
- Must have a `TreeNode` component (not just `Node`) — required for `ClearAllTrees` to identify and `PlaceTree` to validate

### Nodes GameObject (in scene)
- Empty GameObject, parent for all runtime-spawned tree instances
- Assign to `Nodes Parent` on WorldGrid

---

## Key Rules / Patterns

- **Node**: any object occupying a tile. Not every tile has a node, but every node belongs to exactly one tile.
- **TreeNode**: subclass of `Node` used as a type marker. Allows `ClearAllTrees` to only destroy trees without affecting the GlowCore, chests, or other node types.
- **Grid is centered at (0,0,0)** — the GlowCore always sits at world position (0, 0). `m_origin` is a `Vector2Int` storing the array index that maps to world (0, 0). Note: `m_origin.y` represents the Z-axis offset (not Y/height) because `Vector2Int` has no `.z` field.
- **1×1 tile size** — one grid cell = one Unity world unit.
- **Singleton** — always access via `WorldGrid.Instance`.
- **Pending nodes** — nodes placed outside the initial grid bounds are held in `m_pendingNodes` and processed on the next `Expand()` call that brings their tile into bounds. In `ProceduralGeneration` mode, pending trees are destroyed at that point; non-tree nodes are registered normally.
- **`ClearAllTrees` is exhaustive** — clears the grid array AND destroys any `TreeNode` children of the Nodes parent (pending/designer-placed trees outside bounds). `ClearTreesInGrid` is the scoped internal variant used during procedural startup.
- **Occupied tiles are never overwritten** — `PlaceTree` checks via `GetNodeAt` before placing. Chests, GlowCore, and any other non-tree nodes are safe.
- **`TotalTreeCount`** is incremented in `PlaceTree` and reset to 0 in `ClearAllTrees` / `ClearTreesInGrid`.
- **GlowCore level up swaps the entire prefab** — old GameObject (Fire + GlowCoreObject + Node) is destroyed, new level prefab is instantiated at the same position (always world 0,0). The new prefab registers its Node in the grid immediately.
- **Level prefab chain** — each GlowCore prefab has a single `Next Level Prefab` field pointing to the next one. Level 1 → Level 2 → Level 3. No prefab needs to know about anything beyond its direct successor.

---

## Requirements Status

### Done
- [x] The world is a grid (WorldGrid with 2D Node array)
- [x] 2D array of all tiles where Nodes are saved at corresponding index
- [x] Player can't physically cross the border (4 border colliders positioned by WorldGrid)
- [x] At start, border has size 5×5 (`kInitialSize = 5`)
- [x] There is a fire / GlowCore (`Fire` component on GlowCore prefab)
- [x] Border expands when fire grows (`Fire.FeedWood` → `WorldGrid.Expand`)
- [x] WorldGrid is a singleton (`WorldGrid.Instance`)
- [x] Trees placed at specific grid positions (`PlaceTree`)
- [x] World initialises with exactly N randomly placed trees (`SpawnTrees` — ProceduralGeneration mode)
- [x] New outer ring gets trees on each expansion (`SpawnTreesOnNewRing` — ProceduralGeneration only)
- [x] Separate tree counts for regular vs. level-up expansions (`m_treesPerRegularExpansion`, `m_treesPerLevelUpExpansion`)
- [x] GlowCore tracks its own level separately from border expansion (`m_glowCoreLevel`, `m_woodToLevelUp`)
- [x] Wood-per-expansion threshold — feeding N wood triggers floor(N/threshold) regular expansions
- [x] Level-up triggers a large one-off border expansion (`m_levelUpExpansionSize`)
- [x] GlowCore level up replaces the prefab — old destroyed, new spawned, Node re-registered in grid
- [x] Cumulative tree counter (`TotalTreeCount` property)
- [x] GlowCore progressively reveals logs on each wood feed (`GlowCoreObject.ActivateLogs`)
- [x] Grid state logged to console on startup (`LogGrid`) including active WorldMode
- [x] Duplicate node detection — second node at same tile is destroyed with error log
- [x] WorldMode enum — two modes: `ProceduralGeneration`, `DesignedWorld`
- [x] Designed world support — designer-placed nodes registered without clearing or re-spawning
- [x] Pending nodes — out-of-bounds nodes held and registered (or destroyed) on next covering expansion
- [x] Lazy tree cleanup — in ProceduralGeneration, designer-placed trees outside the initial grid are destroyed dynamically as `Expand()` reaches them, not upfront
- [x] Fog border walls — 100-unit deep walls cover the outside world; inner face aligns with grid edge so player collision is unaffected; corners fully covered

### Blocked / Waiting
- [ ] Real inventory + wood pickup mechanic (another team member) — when ready, call `Fire.FeedWood(amount)` from the interaction system

---

## Temporary / Debug Code

| Location | Description |
|----------|-------------|
| `Fire` — `DebugFeedWood()` (private) | ContextMenu: right-click Fire component → "Debug: Feed 1 Wood". Simulates feeding without player interaction. Remove when real interaction is wired up. |
