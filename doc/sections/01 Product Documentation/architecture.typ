== Architecture
=== Frontend Architecture
The user interface of GlowCore is designed to be self-explanatory. Therefore, only selected screens are explained in detail below.

==== Main Menu
#figure(
  image("../../resources/01 Product Documentation/frontend/MainMenu.png", width: 70%),
  caption: [Main Menu],
  supplement: [Image],
)
The Main Menu is the entry point of the game. The player can continue with an existing save of the game or create a new one. The player can only have one save at a time.

==== New Game
#figure(
  image("../../resources/01 Product Documentation/frontend/NewGame.png", width: 60%),
  caption: [New Game],
  supplement: [Image],
)


==== Pause Menu
#figure(
  image("../../resources/01 Product Documentation/frontend/PauseMenu.png", width: 60%),
  caption: [Pause Menu],
  supplement: [Image],
)
In the Pause Menu the player can save the game manually. The game is also automatically saved when the player exits to the Main Menu.

==== Audio Settings
#figure(
  image("../../resources/01 Product Documentation/frontend/AudioSettings.png", width: 60%),
  caption: [Audio Settings],
  supplement: [Image],
)

==== Display Settings
#figure(
  image("../../resources/01 Product Documentation/frontend/DisplaySettings.png", width: 60%),
  caption: [Display Settings],
  supplement: [Image],
)
The appropriate screen size is automatically detected and selected when the game starts.
==== Unapplied Changes
#figure(
  image("../../resources/01 Product Documentation/frontend/UnsavedChanges.png", width: 60%),
  caption: [Unapplied Changes],
  supplement: [Image],
)
When the player changes settings without applying them, this confirmation dialog is displayed. It prevents confusion by ensuring that unsaved changes are clearly communicated.
==== Inventory
#figure(
  image("../../resources/01 Product Documentation/frontend/Inventory.png", width: 100%),
  caption: [Inventory],
  supplement: [Image],
)
The item stacks can be drag-and-dropped within the inventory. By dragging an item outside the inventory panel, the player drops it into the world. The player can also craft certain items in the inventory without the need for a crafting station.
==== Crafting Table
#figure(
  image("../../resources/01 Product Documentation/frontend/CraftingTable.png", width: 100%),
  caption: [Crafting Table],
  supplement: [Image],
)

==== More Crafting Stations
#figure(
  image("../../resources/01 Product Documentation/frontend/Furnace.png", width: 90%),
  caption: [More Crafting Stations],
  supplement: [Image],
)
Crafting Tables, Furnaces, Anvils etc. are abstracted into crafting stations that share a common UI but with different backgrounds and different available crafting recipes. This design follows our extensibility principle and allows new crafting stations to be added easily.

==== GlowCore Upgrade
#figure(
  image("../../resources/01 Product Documentation/frontend/GlowCoreUpgrade.png", width: 87%),
  caption: [GlowCore Upgrade],
  supplement: [Image],
)

==== Chest
#figure(
  image("../../resources/01 Product Documentation/frontend/Chest.png", width: 100%),
  caption: [Chest],
  supplement: [Image],
)



=== C4 Model
To model the architecture of GlowCore we decided to use the C4 Model. The model is designed to provide stakeholders new to the project a high-level overview of GlowCore with different levels of abstraction. We decided against arc42 because it is too extensive for this project.

We intentionally omitted the Code Diagram (Layer 4 of C4) in this documentation because of how much complexity it would add without providing much value. It is common practice in project management to omit the Code Diagram.


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

#pagebreak()

==== Component Diagram
The Component Diagram zooms into selected Containers to provide a deeper overview.


===== Gameplay Logic
A naive approach to implement features like Breaking Nodes, Building, Inventories and Crafting would be to just implement them separately, each with its own isolated input handling and execution logic. This would result in duplicated code and poorly maintainable code.

Instead, we analyzed the features and abstracted them into two groups: *Use Actions* and *Interact Actions*.

Use Actions are responsible for using the Item in the player's hand (The item that is selected in the hotbar and visually displayed in the player's hand). The player can only break Nodes when he holds a tool in his hand and he can only place Nodes if he holds a Block in his hand.

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
The most important decision was the selection of the game engine. We decided to use Unity because we had the most experience with it and all members have already learned C\#. Had our game been 2D and not 3D, Godot would have been our preferred choice.

==== Universal Render Pipeline (URP)
Unity provides 3 rendering pipelines:
- *Built-in Render Pipeline*: Legacy render pipeline
- *Universal Render Pipeline (URP)*: Lightweight scriptable render pipeline.
- *High Definition Render Pipeline (HDRP)*: For realistic surfaces.
We chose URP because the HDRP would be overkill for our project and the Built-in Render Pipeline is deprecated and new projects should use URP instead. URP is perfectly suitable to create custom shaders and viewports.

==== .NET & C\#
Unity's default scripting language is C\#, so this decision came bundled with the selection of the game engine. Although it is possible to use Plugins to run other languages such as C++, there are no significant benefits, so we settled on C\#.

Unity uses Mono, a .NET runtime, to create cross-platform applications. We use NUnit via the Unity Testing Framework for automated testing.

==== GitHub
We use GitHub for version control. At the beginning of the project we used GitLab, but we switched to GitHub because we experienced problems setting up the CI/CD Pipeline to work. The documentation on GitLab was limited, since most tutorials and examples are focused on GitHub Actions.

==== Blender
To create our 3D Models we use Blender. Blender is free and open-source and offers all features of its competitors, so the choice was obvious. The workflow to import 3D Models from Blender into Unity is great and preserves all important information.

==== Audacity
We use Audacity to record and edit our sound effects. Audacity is free, easy to use and provides all necessary tools for simple sound design, so there was no need to choose a more sophisticated audio editing software.

#figure(
  image("../../resources/01 Product Documentation/dev-env-diagram.png"),
  caption: [Technologies Diagram],
  supplement: [Image],
)

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
In our game we have to define and store data about Items and Nodes in an organized way. Meant are the Item/Node-Definitions, not the instances at runtime. For example for the Wood-Item we need to store its ID, 3D Model, Max Stack-Size etc.

Scriptable Objects solve this problem. Scriptable Objects are objects that are not bound to a GameObject. Instances of ScriptableObjects can be saved as *.asset* files. This improves Version Control for Items/Nodes since they are saved as individual files.

==== Prefabs
We use Prefabs to save specific GameObjects as *.prefab* files. This enables us to dynamically Instantiate Prefabs as GameObjects during runtime, which is necessary for e.g. Building.

#pagebreak()
=== Extensibility
Our game is designed to be extensible and exchangeable wherever feasible. Thanks to our ScriptableObject Architecture we can easily add new Items/Nodes just by creating a new Instance of the corresponding ScriptableObject and setting the parameters and referencing the 3D Model and Icon.

Thanks to the Component-Based Architecture we can easily add new Items that perform a specific action when used by the player. It would be easy to add for example Consumables. An additional benefit is that it is easy to add Nodes with custom behaviour for when the player interacts with it. For example you could just create a Signpost component and add it to a Node to create a Node where you can write something on it and read it.

We save our 3D Models separately from the ScriptableObjects in the .fbx format to improve exchangeability. It is possible to only exchange the 3D Model without touching the ScriptableObject.

A major limit of our architecture is that we are strongly tied to Unity. Switching to another game engine would require a significant effort due to how our systems are relying on Unity's systems.

=== External Dependencies
Our game is distributed via Steam, which is our only external dependency. Steam is responsible for installing, launching and updating the game on the end-user side. We also use Steam for marketing.

We create the store page for our game on Steamworks, where we can configure the description, upload marketing material etc. The executable is uploaded using Steamworks SteamPipe, which handles distribution and versioning.

=== Game Design
This chapter covers Gameplay Decisions rather than technical solutions, focusing on how the game is structured to create an entertaining experience. Game design can be understood as the application of user experience principles within the domain of game development.

==== Core Gameplay Loop
The core gameplay loop of GlowCore can be broken down into the following steps:
+ Gather resources
+ Craft items
+ Upgrade tools
+ Build Machines
+ Upgrade the GlowCore, which unlocks new areas and items
+ Repeat loop

==== Gameplay Progression
The following sections describe the gameplay progression in chronological order and explain the design decisions and their reasons.

===== Early Game
The player starts with a basic wooden axe and gathers wood by manually cutting trees. Early trees can be chopped quickly but only give a small amount of wood. Over time, larger trees appear which require significantly more time to cut but also give more wood.

#pagebreak()

===== Automation Phase 1
At this point, manual gathering becomes inefficient, encouraging the player to craft their first Axe Machine. This machine is placed in front of a tree and automatically cuts it over time. While the machine is operating, the player is free to perform other actions, such as gathering additional resources to craft more machines.

This creates a strong feeling of progress and a passive progression reward, since the player knows that progress is being made even while he is not directly interacting with the machine. This effect is reinforced as the player uses gathered resources to construct more machines, amplifying the loop.

When returning to the machine, the player collects the gathered resources and moves the machine to a new tree. This is acceptable at this stage because repositioning requires significantly less effort than manual tree cutting.

===== Automation Phase 2
Over time, the player reaches a point where moving so many Axe Machines becomes tedious. At this stage, a new system is introduced: The Replanter block. The Replanter automatically places saplings. This allows trees to regrow automatically, removing the need to constantly reposition machines. However, this introduces a trade-off, as machines remain inactive while waiting for trees to regrow. This adds strategic depth to the system.

===== Automation Phase 3
Later in progression, a new tree type is introduced that has short cut time and drops a relatively big amount of wood but has a very long growth time. To improve efficiency the player crafts the newly unlocked Axe Machine Tier 2, which moves automatically in the direction it's facing and harvests trees sequentially.

At this stage, multiple Replanter blocks can be used per machine, enabling more complex automated setups. A key design decision is that Replanter blocks must be significantly cheaper than Tier 2 machines, encouraging players to build efficient production lines.


===== Full Automation
However, this introduces a new problem: machine orientation and placement still require manual adjustment. This is resolved by the introduction of Rotator blocks. When a machine collides with a Rotator block, it automatically rotates by 90, 180, or 270 degrees depending on the block type. This enables fully automated resource gathering systems and allows players to design complex production paths.

==== Automation Constraints
A core design principle in this system is that not everything should be fully automatable. If all actions were automated, the player would just watch the machines and become bored. Instead, the player must still actively gather certain resources in order to craft and expand automation systems.

==== Depth and Complexity
We followed the principle of providing as much depth as possible with as little complexity as possible.
The goal of this design is to allow the player to create complex and interesting systems without introducing a large number of different mechanics and complicated rules.

In GlowCore, this is achieved by limiting the core automation system to only three main components: the Axe Machine, the Replanter, and the Rotator. When combined, these simple systems interact in ways that allow for emergent complexity, such as automation lines with different layouts.

#pagebreak()

=== Key Architectural Decisions

The following decisions had the greatest impact on the structure and long-term maintainability of the game. Each one is recorded together with the problem it solved and the reasoning behind the chosen approach.

==== ScriptableObjects as the Data Layer

*Decision:* All item, node, and recipe definitions are stored as ScriptableObject `.asset` files rather than in C\# code or external data files (e.g. JSON).

*Why:* Game content, items, nodes, crafting recipes, must be defined once and consumed from many places without creating code dependencies. ScriptableObjects let non-programmers create and configure new content entirely through the Unity Inspector. Each asset is an individual file, which keeps version control diffs small and conflict-free. Crucially, the connection to gameplay code is a serialised Unity reference, so adding a new item, node, or recipe never requires touching any C\# source file.

==== IInteractable for World Interaction

*Decision:* Every node that responds to player interaction implements the `IInteractable` interface. `NodeActionSystem` calls `Interact()` on whichever node is currently hovered, without knowing its concrete type.

*Why:* Without a shared interface, `NodeActionSystem` would require an explicit branch for every interactive object type: chests, crafting stations, the GlowCore, signs, and anything added later. With `IInteractable` the input handler is permanently closed to modification on this axis, adding a new interactive node is purely additive. The `GetActionPromptText()` method on the same interface also lets each node control its own UI prompt label, removing another potential switch statement from the core input loop.

==== IInventoryService and Event-Driven UI

*Decision:* The inventory system is exposed to all UI components exclusively through the `IInventoryService` interface, which carries both commands and `Action<T>` events (`OnSlotChanged`, `OnHotbarSelectionChanged`, etc.).

*Why:* Inventory state is modified from many independent sources: item collection, crafting, chest transfers, and GlowCore upgrades. A polling-based UI would either check every frame or miss updates; direct method calls from each source to each UI component would create a tightly coupled web of dependencies. Events mean every UI component reacts exactly when something relevant changes, at zero cost when idle. Because all UI depends only on the interface and not on `PlayerInventory` directly, the inventory implementation can change freely without any UI class breaking.

==== Use Actions and Interact Actions

*Decision:* Instead of implementing each player capability (breaking nodes, placing blocks, opening chests, crafting) as an isolated system with its own input handling, all player inputs are routed through exactly two abstractions: *Use Actions* (what the held item does, via `IHandItem`) and *Interact Actions* (what the hovered node does, via `IInteractable`).

*Why:* The two abstractions map directly to the two roles the player's cursor plays: the held item and the targeted node. Routing them separately through `PlayerHand` and `NodeActionSystem` keeps input handling in exactly one place per axis and avoids duplicating raycast logic, inventory checks, and UI-open guards. Adding a new usable item only requires implementing `IHandItem` on its prefab; adding a new interactive node only requires implementing `IInteractable`. Neither addition changes any existing class.

#pagebreak()

==== IPlayerInventoryAware via Explicit Injection

*Decision:* Item prefab components that need access to the player's inventory receive it through `IPlayerInventoryAware.SetInventory()`, called by `PlayerHand` at equip time, not through a singleton or a `FindObjectOfType` lookup.

*Why:* Tools and blocks need to read and modify the inventory at use-time (e.g. consuming the block stack when placing). Fetching `PlayerInventory.Instance` inside a prefab component hides the dependency, makes the coupling invisible to readers, and makes the component untestable in isolation. Passing the reference explicitly at equip time makes the dependency visible, eliminates hidden singleton coupling, and allows the component to be tested with any `PlayerInventory` instance, including a mock.

=== Deployment

The game has two deployment paths, both automated through GitHub Actions. Which path runs depends on what is pushed to the repository.

==== Continuous Deployment - WebGL (GitHub Pages)

Every push to `main` or `dev` triggers the WebGL build job. All jobs run on a self-hosted Linux runner with Unity 6000.3.9f1 installed directly on the machine. The Unity Editor is invoked in batch mode via a custom `BuildScript`, which avoids the overhead of spinning up a Docker container on every run. The license is activated per-run using `buildalon/activate-unity-license`. The resulting build artifact is deployed to the `pages` branch via `JamesIves/github-pages-deploy-action`. Builds from `main` land in `prod/webgl`; builds from `dev` land in `dev/webgl`. This gives stakeholders an always-up-to-date playable version of the game in a browser without any manual steps.

==== Release Deployment - Windows & Linux (GitHub Releases)

Pushing a version tag (e.g. `v1.0.0`) triggers the Windows and Linux build jobs in parallel, both running on the same self-hosted runner. Each job invokes the Unity Editor directly in batch mode, targeting `StandaloneWindows64` and `StandaloneLinux64` respectively. Once both builds complete, a `release` job downloads the artifacts, zips them, and publishes them as a GitHub Release with auto-generated release notes. Players can download the zipped executables directly from the GitHub Releases page.

==== Steam

Once the project is ready to be published to Steam, the release pipeline will be extended with a SteamPipe upload step after the Windows and Linux builds. The `steamcmd` CLI can authenticate using Steamworks credentials stored as GitHub Actions secrets and upload the build depot to Steam. Steam would then handle CDN distribution to all players who own the game. No structural change to the existing pipeline would be required.

==== Deployment Diagram

#figure(
  image("../../resources/01 Product Documentation/steam-deployment.png", width: 100%),
  caption: [Deployment Diagram],
  supplement: [Image],
)
