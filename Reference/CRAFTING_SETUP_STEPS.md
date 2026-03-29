# Crafting Panel — Unity Editor Setup Steps

All C# scripts are already written. These steps wire everything up in the Unity Editor.

---

## Step 1: Create the RecipeRow Prefab

1. In Hierarchy, right-click > UI > Image. Delete the auto-created Canvas, keep just the Image GO. Rename to `RecipeRow`.

2. Set up the root (`RecipeRow`):
   - Add component: **RecipeRowUI**
   - The existing `Image` = the border. Color: #5A4123 alpha ~102 (SlotBorder). Raycast Target ON.
   - Add `LayoutElement`, Preferred Height = 48.

3. Child: **Background** (right-click RecipeRow > UI > Image):
   - Stretch fill, inset 1px all sides (Left/Right/Top/Bottom = 1)
   - Color: #0C0804 alpha ~153 (SlotBg)
   - Raycast Target OFF

4. Child: **ResultIcon** (right-click RecipeRow > UI > Image):
   - Anchor middle-left. Width 32, Height 32, Pos X = 8.
   - Preserve Aspect ON. Raycast Target OFF.

5. Child: **InfoColumn** (right-click RecipeRow > Create Empty):
   - Add `VerticalLayoutGroup`: spacing 1, child alignment Middle Left, force expand width ON, height OFF
   - Anchor stretch-stretch. Left = 46, Right = 60, Top = 2, Bottom = 2.

6. Inside InfoColumn: **RecipeName** (right-click > UI > Text - TextMeshPro):
   - Font: Cinzel Bold, size 10, color #E8C777 (Accent)
   - Overflow Ellipsis. Raycast Target OFF.
   - Add `LayoutElement`, Preferred Height = 14.

7. Inside InfoColumn: **Ingredients** (right-click > Create Empty):
   - Add `VerticalLayoutGroup`: spacing 0, force expand width ON, height OFF
   - Add `ContentSizeFitter`: Vertical Fit = Preferred Size
   - (RecipeRowUI spawns ingredient labels here at runtime)

8. Child: **CraftButton** (right-click RecipeRow > UI > Button - TextMeshPro):
   - Anchor middle-right. Width 50, Height 24, Pos X = -8.
   - Child TMP: font Nunito Bold, size 8, text "CRAFT", color white, center aligned
   - Button colors: Normal = SlotFilledBg, Highlighted = SlotHoverBg, Disabled = dark gray low alpha

9. Wire RecipeRowUI fields in Inspector:
   - `m_borderImage` -> RecipeRow root Image
   - `m_bgImage` -> Background Image
   - `m_resultIcon` -> ResultIcon Image
   - `m_recipeName` -> RecipeName TMP
   - `m_ingredientParent` -> Ingredients Transform
   - `m_craftButton` -> CraftButton Button

10. Drag `RecipeRow` from Hierarchy into `Assets/Prefabs/UI/` to save as prefab. Delete from scene.

---

## Step 2: Build CraftingPanel in InventoryCanvas

1. Open `Assets/Prefabs/UI/InventoryCanvas.prefab`.

2. Right-click root > Create Empty, rename `CraftingPanel`:
   - Add components: **CraftingUI**, **CanvasGroup**
   - CanvasGroup: alpha 0, interactable OFF, blocksRaycasts OFF (starts hidden)
   - Position right of inventory panel. ~200px wide, match inventory height.
   - Add `Image` for background: color #1C140C alpha ~235 (WoodDark). Optional border child in WoodBorder color.

3. Inside CraftingPanel: **Title** (UI > Text - TextMeshPro):
   - Text "CRAFTING", Cinzel Bold, size 12, color #C4A05A (AccentDim)
   - Anchor top-center, height ~20.

4. Inside CraftingPanel: **ScrollView** (UI > Scroll View):
   - Stretch below title (Top ~24, Left/Right/Bottom = 4)
   - ScrollRect: uncheck Horizontal, keep Vertical
   - Delete Scrollbar Horizontal child if created

5. Inside ScrollView > Viewport > **Content**:
   - Add `VerticalLayoutGroup`: spacing 4, padding 4 all sides, force expand width ON, height OFF
   - Add `ContentSizeFitter`: Vertical Fit = Preferred Size

6. Wire CraftingUI fields:
   - `m_playerInventory` -> Player GO (wire in scene if editing prefab in isolation)
   - `m_panelCanvasGroup` -> CraftingPanel's CanvasGroup
   - `m_contentParent` -> ScrollView > Viewport > Content
   - `m_recipeRowPrefab` -> Assets/Prefabs/UI/RecipeRow.prefab
   - `m_recipes` -> leave empty for now (Step 5)

---

## Step 3: Add CraftTabButton

1. Inside InventoryCanvas prefab, right-click the InventoryPanel > UI > Button - TextMeshPro, rename `CraftTabButton`:
   - Anchor top-right of inventory panel
   - Size ~60 x 20
   - Child TMP: "CRAFT", Cinzel Bold, size 9, color AccentDim
   - Button background: SlotFilledBg or WoodDark

2. Button On Click ():
   - Click +, drag CraftingPanel GO into object field
   - Function: **CraftingUI > Toggle()**

---

## Step 4: Wire InventoryUI.m_craftingUI

1. Select the GO with the **InventoryUI** component
2. Find the **Crafting** header, `m_craftingUI` field
3. Drag CraftingPanel GO into that slot

---

## Step 5: Create Recipe Assets

1. Project window: right-click `Assets/ScriptableObjects/` > Create > Folder > name `Recipes`

2. Inside Recipes: right-click > Create > Scriptable Objects > Recipe. Example:
   - Name: "Plank"
   - Ingredients: + -> Item = Wood, Amount = 3
   - ResultItem: (create/assign the result Item asset)
   - ResultAmount: 1

3. Repeat for other recipes.

4. Select CraftingPanel > CraftingUI > `m_recipes` array:
   - Set size to number of recipes
   - Drag each Recipe asset into the slots

---

## Step 6: Scene Wiring

If `m_playerInventory` on CraftingUI is unset (edited prefab in isolation):
- Open MainWorldScene
- Find InventoryCanvas instance > CraftingPanel
- Drag the Player GO into `m_playerInventory`

---

## Testing

- TAB to open inventory
- Click CRAFT button (top-right) or press C to toggle crafting panel
- Recipes with enough materials: green border, enabled button
- Recipes missing materials: dimmed, disabled button, red counts
- Click CRAFT: materials consumed, result added, counts update
- Close inventory (TAB): crafting panel auto-closes
