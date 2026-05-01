== Architecture

=== C4 Model
To model the architecture of GlowCore we decided to use the C4 Model. The model is designed to provide stakeholders new to the project a high-level overview of GlowCore with different levels of abstraction. We decided against arc42 because it is too extensive for this project.

We intentionally omitted the Code Diagram (Layer 4 of C4) in this documentation because of how much complexity it would add without providing much value. This is common in project management to omit the Code Diagram.


==== System Context Diagram

#figure(
  image("../../resources/01 Product Documentation/C4SystemContextDiagram.png", width: 40%),
  caption: [C4 System Context Diagram],
  supplement: [Image],
)

#pagebreak()
==== Container Diagram

#figure(
  image("../../resources/01 Product Documentation/C4ContainerDiagram.png"),
  caption: [C4 Container Diagram],
  supplement: [Image],
)

==== Component Diagram
The Component Diagram zooms into selected Containers to provide a deeper overview.


===== Gameplay Logic
A naive approach to implement features like Breaking Nodes, Building, Inventories and Crafting would be to just implement them separately, each with its own isolated input handling and execution logic. This would result in duplicated code and poorly maintanable code.

Instead, we analyzed the features and abstracted them into two groups: *Use Actions* and *Interact Actions*.

Use Actions are responsible for using the Item in the players hand (The item that is selected in the hotbar and visually displayed in the players hand). The player can only break Nodes when he holds a tool in his hand and he can only place Nodes if he holds a Block in his hand.

Interact Actions define the behaviour of hovered over Nodes. For example the player opens a chest by hovering over it with the cursor and pressing the corresponding button. The GlowCore and all crafting stations share the same input to avoid code duplication. This approach also makes our game extensible.


#figure(
  image("../../resources/01 Product Documentation/C4GameplayLogic.png"),
  caption: [C4 Component Diagram - Gameplay Logic],
  supplement: [Image],
)

===== Presentation Layer
#figure(
  image("../../resources/01 Product Documentation/C4Presentation.png"),
  caption: [C4 Component Diagram - Presentation Layer],
  supplement: [Image],
)

#pagebreak()
=== Interfaces

GlowCore's container boundaries are expressed through either a C\# interface or a ScriptableObject asset reference, with the exception of the input boundary where Unity's own message dispatch handles the connection directly.

==== Overview Diagram

#figure(
  image("../../resources/01 Product Documentation/C4InterfaceOverview.png", width: 100%),
  caption: [Interface Overview Diagram],
  supplement: [Image],
)

==== Interface Contracts

#figure(
  table(
    columns: (auto, 1fr),
    inset: 8pt,
    align: (left, left),
    stroke: 0.5pt,
    fill: (x, y) => {
      // Separator rows (y=1, y=7) use a neutral divider colour; data rows use
      // one accent colour per container boundary; right column gets a 55% tint.
      let colors = (
        rgb("C0C0C0"), // y=0  header
        rgb("D4D4D4"), // y=1  separator: Container Boundaries
        rgb("BDD7EE"), // y=2  Input Handler → Gameplay Logic   (blue)
        rgb("C6EFCE"), // y=3  Gameplay Logic → GlowCore World   (green)
        rgb("D9E1F2"), // y=4  Gameplay Logic → Presentation     (lavender)
        rgb("D9E1F2"), // y=5  Gameplay Logic → Presentation     (lavender)
        rgb("FFF2CC"), // y=6  Data Layer → Gameplay Logic       (amber)
        rgb("D4D4D4"), // y=7  separator: GL Internal Boundaries
        rgb("D6DCE4"), // y=8  GL internal → Item Prefabs        (steel)
        rgb("D6DCE4"), // y=9  GL internal → Item Prefabs        (steel)
      )
      let c = colors.at(y, default: white)
      if x == 0 or y == 0 or y == 1 or y == 7 { c } else { c.lighten(55%) }
    },
    // ── header ──────────────────────────────────────────────────────────────
    [*Interface*\
      #text(size: 8pt, fill: rgb("444444"))[_Boundary_]\
      #text(size: 8pt, fill: rgb("444444"))[_Mechanism_]],
    [*Purpose*],
    // ── y=1  separator ───────────────────────────────────────────────────────
    table.cell(colspan: 2)[#text(weight: "bold", size: 9pt)[Container Boundaries]],
    // ── y=2  Input Handler → Gameplay Logic ─────────────────────────────────
    [*Unity Input callbacks*\
      #text(size: 8.5pt, fill: rgb("444444"))[_Input Handler → Gameplay Logic_]\
      #text(size: 8pt, fill: rgb("666666"))[Unity Input System (no C\# interface)]],
    [Unity calls methods like `OnUse`, `OnInteract` and `OnMove` directly on MonoBehaviour components such as `PlayerHand` and `NodeActionSystem`. There is no C\# interface at this boundary. Unity's own message dispatch handles the connection.],
    // ── y=3  Gameplay Logic → GlowCore World ────────────────────────────────
    [*`IInteractable`*\
      #text(size: 8.5pt, fill: rgb("444444"))[_Gameplay Logic → GlowCore World_]\
      #text(size: 8pt, fill: rgb("666666"))[C\# interface]],
    [Nodes that do something when the player interacts with them implement this interface. A `Chest` opens its inventory, a `CraftingTableInteractable` shows the crafting UI, and `GlowCoreObject` runs its own logic, all through the same single-method contract.],
    // ── y=4  Gameplay Logic → Presentation Layer ────────────────────────────
    [*`IInventoryService`*\
      #text(size: 8.5pt, fill: rgb("444444"))[_Gameplay Logic → Presentation Layer_]\
      #text(size: 8pt, fill: rgb("666666"))[C\# interface + events (`Action<T>`)]],
    [The main contract between the inventory system and the UI. Components like `InventoryUI` and `HotbarUI` subscribe to events such as `OnSlotChanged` and `OnHotbarSelectionChanged` to stay in sync with the game state, without ever referencing `PlayerInventory` directly.],
    // ── y=5  Gameplay Logic → Presentation Layer ────────────────────────────
    [*`ICraftingService`*\
      #text(size: 8.5pt, fill: rgb("444444"))[_Gameplay Logic → Presentation Layer_]\
      #text(size: 8pt, fill: rgb("666666"))[C\# interface + event (`Action`)]],
    [Gives the crafting UI everything it needs: the current recipe list, availability checks, the `Craft()` command to actually execute a recipe, and the `OnRecipesRefreshed` event that triggers a refresh whenever the player's inventory changes.],
    // ── y=6  Data Layer → Gameplay Logic ────────────────────────────────────
    [*ScriptableObject asset refs*\
      #text(size: 8.5pt, fill: rgb("444444"))[_Data Layer → Gameplay Logic_]\
      #text(size: 8pt, fill: rgb("666666"))[Unity serialised reference (`[SerializeField]`)]],
    [`Item`, `NodeData` and `Recipe` assets are set up in the Unity Inspector and read at runtime by the components that need them. There is no code dependency; the connection is entirely through serialised Unity references.],
    // ── y=7  separator ───────────────────────────────────────────────────────
    table.cell(colspan: 2)[#text(weight: "bold", size: 9pt)[Gameplay Logic - Internal Component Boundaries]],
    // ── y=8  GL internal → Item Prefabs ─────────────────────────────────────
    [*`IHandItem`*\
      #text(size: 8.5pt, fill: rgb("444444"))[_PlayerHand → Item Prefab (GL internal)_]\
      #text(size: 8pt, fill: rgb("666666"))[C\# interface]],
    [Any item that can be used while held in the player's hand implements this interface. When the player presses the use button, `PlayerHand` calls `Use()` on whichever component is currently active, without caring whether it is a `ToolBehaviour`, a `BlockBehaviour`, or anything else.],
    // ── y=9  GL internal → Item Prefabs ─────────────────────────────────────
    [*`IPlayerInventoryAware`*\
      #text(size: 8.5pt, fill: rgb("444444"))[_PlayerHand → Item Prefab (GL internal)_]\
      #text(size: 8pt, fill: rgb("666666"))[C\# interface]],
    [Some item prefab components need access to the player's inventory to function correctly. This interface lets `PlayerHand` pass them a reference when the item is equipped, avoiding any need for a singleton lookup.],
  ),
  caption: [Interface Contracts],
  supplement: [Table],
)

==== Extensibility through Stable Interfaces

These interfaces are what makes NFR401 achievable in practice. Each interface was designed so that adding something new only requires work in one place.

Adding a new usable item means implementing `IHandItem` on a prefab component. Adding a new interactive node means implementing `IInteractable`. In both cases nothing else in the system needs to change: the Input Handler, the Presentation Layer, and the Data Layer are completely unaware of the new addition.

Extending the item and crafting catalogue is just as simple. A new `Item` or `NodeData` asset can be created as a ScriptableObject, filled in through the Unity Inspector, and referenced wherever it is needed, without touching any C\# code at all. The same applies to crafting recipes: a new `Recipe` asset is all that is needed to make a new combination available in the crafting UI.

The inventory and crafting UI are equally easy to extend. Because `InventoryUI` and `HotbarUI` depend only on `IInventoryService`, and `CraftingTableUI` only on `ICraftingService`, a new UI component for either system can be added by subscribing to the same events and use given methods, without modifying any existing class.

#pagebreak()
=== Technologies

==== Unity
The most important decision was the selection of the game engine. We decided to use Unity because we had the most experience in it and all members have already learned C\#. Had our game been 2D and not 3D, Godot would have been our preferred choice.

==== Universal Render Pipeline (URP)
Unity provides 3 rendering pipelines:
- *Built-in Render Pipeline*: Legacy render pipeline
- *Universal Render Pipeline (URP)*: Lightweight scriptable render pipeline.
- *High Definition Render Pipeline (HDRP)*: For realistic surfaces.
We chose URP because the HDRP would be overkill for our project and the Built-in Render Pipeline is deprecated and new projects should use URP instead. URP is perfectly suitable to create custom shaders and viewports.

==== .NET & C\#
Unity's default scripting language is C\#, so this decision came bundled with the selection of the game engine. Altough it is possible to use Plugins to run other languages such as e.g. C++, there are no significant benefits, so we settled on C\#.

Unity uses Mono, a .NET runtime to create cross-platform applications. We use NUnit via the Unity Testing Framework for automated testing.

==== GitHub
We use GitHub for version control. At the beginning of the project we used GitLab, but we switched to GitHub because we experienced problems setting up the CI/CD Pipeline to work. The documentation on GitLab was limited, since most tutorials and examples are focused on GitHub Actions.

==== Blender
To create our 3D Models we use Blender. Blender is free and open-source and offers all features of its competitors, so the choice was obvious. The workflow to import 3D Models from Blender into Unity is great and preserves all important informations.

==== Audacity
We use Audacity to record and edit our soundeffects. Audacity is free, easy to use and provides all necessary tools for simple sound design, so there was no need to choose a more sophisticated audio editing software.

#pagebreak()
=== Used Design Patterns

==== Component-Based Architecture
We use Unity's Component-Based Architecture, where every GameObject has Components that define its behaviour. To implement custom Components, we use MonoBehaviour-Scripts to define the logic of a GameObject. This approach increases modularity because it is possible to split the logic into different Components and reuse them.

We don't have a real *Entity Component System (ECS)* because we don't separate the logic from the data. To support a real ECS we would need to use Unity's *Data-Oriented Technology Stack (DOTS)*. For our project the additional complexity of DOTS would outweigh its performance benefits.

==== Singleton
We use the Singleton Pattern where appropriate. In our game it definitely makes sense because we have systems that only exist once. For example there is only one Player and only one World-Grid.

==== Dependency Injection
Games often have tightly coupled systems. To counter this we use dependency injection wherever appropriate to decrease coupling.

==== Scriptable Objects
In our game we have to define and store data about Items and Nodes in a organized way. Meant are the Item/Node-Definitions, not the instances at runtime. For example for the Wood-Item we need to store its ID, 3D Model, Max Stack-Size etc..

Scriptable Objects solve this problem. Scriptable Objects are objects that are not bound to a GameObject. Instances of ScriptableObjects can be saved as *.asset* files. This improves Version Control for Items/Nodes since they are saved as individual files.

==== Prefabs
We use Prefabs to save specific GameObjects as *.prefab* files. This enables us to dynamically Instantiate Prefabs as GameObjects during runtime, which is necessary for e.g. Building.

=== CI/CD-Architektur
This section provides a quick overview of our CI/CD setup on GitHub. As shown in the picture, we use five specific workflows to automate our development process.

Each workflow is designed to handle a different part of the pipeline, ranging from code quality checks and automated testing to the final deployment and documentation updates. Having these automated steps helps us maintain a clean codebase and ensures that we can focus more on developing new features.
#image("/resources/01 Product Documentation/cicd-workflow.png")

#pagebreak()
=== Extensibility
Our game is designed to be extensible and exchangeable wherever feasible. Thanks to our ScriptableObject Architecture we can easily add new Items/Nodes just by creating a new Instance of the corresponding ScriptableObject and setting the parameters and referencing the 3D Model and Icon.

Thanks to the Component-Based Architecture we can easily add new Items that perform a specific action when used by the players. It would be easy to add for example Consumables. An additional benefit that it is easy to add Nodes with custom behaviour for when the player interacts with it. For example you could just create a Signpost component and add it to a Node to create a Node where you can write something on it and read it.

We save our 3D Models separate from the ScriptableObjects in the .fbx format to improve exchangeability. It is possible to only exchange the 3D Model without touching the ScriptableObject.

A major limit of our architecture that we are strongly tied to Unity. Switching to another game engine would require a significant effort due to how our systems are relying on Unity's systems.

=== External Dependencies
Our game is distributed via Steam, which is our only external dependency. Steam is responsible for installing, launching and updating the game on the end-user side. We also use Steam for marketing.

We create the store page for our game on Steamworks, where we can configure the description, upload marketing material etc. The executable is uploaded using Steamworks SteamPipe, which handles distribution and versioning.
