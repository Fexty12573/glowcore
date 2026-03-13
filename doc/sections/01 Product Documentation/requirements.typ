== Requirements

// TODO: Describe the functional and non-functional requirements as covered in SEP1.

=== Functional Requirements
==== Actors / Personas
Since a game typically only has one primary actor - *The Player* - it makes sense to define Personas that reflect different types of players. The categorization into Casual- and Dedicated Players covers most player types.



===== Casual Player
- The Casual Player plays GlowCore infrequently and for short amounts of time. He wants to relax and unwind from his life. He prefers to play the game just for fun without putting effort into it.
- The Casual Player wants to experience the game from beginning to end with a story and explore the world. When he plays the game for the first time, he depends on the game to explain the rules of the game to him without being too overwhelming and tedious. As soon as he finishes the game, he will be done with it, having experienced everything he wanted.
- The Casual Player never changes settings in games, he never feels the need to customize anything. He likes to play the game as it is.

===== Dedicated Player
- The Dedicated Player plays GlowCore in long sessions. He loves games where you build factories and automate everything. He searches for the feeling of progress and finds satisfaction in it. He always searches for a better way to progress and optimize his gameplay.
- The Dedicated Player has already played the game multiple times before, thus he doesn't want to be bored with a long tutorial everytime he starts a new game. He wants to be able to dictate the pacing of the game.
- He doesn't care for the story and lore in games. He doesn't want to read through dialogues and doesn't like long cutscenes.

==== Use Case Diagram
The diagram shows a modified version of the Use Case Diagram. The Personas take the place of the Actors. An arrow indicates that a Persona uses a feature. A missing arrow means that the persona is not interested in using a feature, although it remains accessible to him.

#figure(
  image("../../resources/02 Project Documentation/UseCaseDiagram.png"),
  caption: [Use Case Diagram],
  supplement: [Image],
)
#v(1em)
#pagebreak()

==== Use Cases
The Use Cases are described in the brief format, with each Use Case including an appropriately detailed description.

It is essential for the project that all Use Cases that define the *MVP* are implemented.
The Use Cases marked with *Priority: Middle* are planned to be implemented, but can be reduced if the MVP takes more time than anticipated. The Use Cases marked with *Priority: Low* will be implemted if enough time is left.


// #table(
//   columns: (0.7fr, 0.8fr, 0.8fr, 1.7fr),
//   stroke: 0.5pt + gray,
//   fill: (x, y) => if y == 0 { luma(230) },
//   align: left,

//   [*ID*], [*Personas*], [*Priority*], [*Description*],
//   [UC01 - Install Game],
//   [Casual Player & Dedicated Player],
//   [High - Required for MVP],
//   [The player can install the game. The installation requires no technical skills.],

//   [UC02 - Save Game],
//   [Casual Player & Dedicated Player],
//   [High - Required for MVP],
//   [When the player stops playing, the game saves his save state in a file and loads it when he resumes the game.],

//   [UC03 - Break Nodes],
//   [Casual Player & Dedicated Player],
//   [High - Required for MVP],
//   [The player can break Nodes in the world with his hands or with tools. The usage of tools e.g. an axe to break a tree, increases the speed significantly. When the Nodes break, they drop items depending on the type of Node. E.g. a Tree drops a Wood-Item. Certain Nodes that are built by the player themselves such as a workbench drop themselves upon breaking.],

//   [UC04 - Expand Map],
//   [Casual Player & Dedicated Player],
//   [High - Required for MVP],
//   [The player is confined to only move on tiles that are unlocked at a given point. The player can drop off items at the GlowCore, whereupon the map expands. The player can then move into and explore the new area.],

//   [UC05 - Store Items],
//   [Casual Player & Dedicated Player],
//   [High - Required for MVP],
//   [Items that the player picks up are stored in his inventory. The player's inventory has a limited size. To organize items, the player can move items into chests and take them out again.],

//   [UC06 - Craft Items],
//   [Casual Player & Dedicated Player],
//   [High - Required for MVP],
//   [The player can craft items into other items with predefined recipes. Without a workbench the player is limited to a few simple recipes. The player can interact with a workstation such as a workbench to unlock more crafting recipes. Workstations such as an oven require additional items: e.g. Coal to smelt an Ingot.],

//   [UC07 - Build Nodes],
//   [Casual Player & Dedicated Player],
//   [High - Required for MVP],
//   [The player can place certain items onto the world, where they become Nodes. Each Tile can hold only one Node, and the player is not allowed to build a Node on an already occupied Tile.],

//   [UC08 - Experience Story],
//   [Casual Player],
//   [Low],
//   [The player can experience a story while playing the game. When the game starts there is an intro sequence. There is an end to the game, at which point a cutscene is played. The game contains small narrative hints that fit into the story conveyed by the GlowCore-World, including the acoustic, the visual art style and the overall gameplay experience.],

//   [UC09 - Fight Enemies],
//   [Casual Player & Dedicated Player],
//   [Middle],
//   [The player can engage in combat with enemies. Defeated enemies drop items that are useful for the player. When the enemy hits the player, he loses hitpoints. When the player has no hitpoints left, he loses some of his items and respawns near the center of the map.],

//   [UC10 - Automate Resource Gathering],
//   [Dedicated Player],
//   [Middle],
//   [The player can automate tedious tasks, such as breaking Nodes. He can craft machines that are placed as Nodes and they automatically perform a task e.g. chopping down trees. There is a selection of machines that can be combined to automate more and more. Example: The player places multiple Replanter-Machines in a row which automatically replant trees on the neighbouring tile. The player places a Axe-Machine onto the start of the line, at which point the Axe-Machine chops one tree after another and then moves onto the next tile. To automate it even further, the player places Rotaters on both which cause the Axe-Machine to turn around on contact.],

//   [UC11 - Change Settings],
//   [Dedicated Player],
//   [Middle],
//   [The player can change settings of the game, such as the FOV and the Volume.],
// )

// #figure(
//   kind: table,
//   caption: [Use Cases],
//   supplement: [Table],
//   none,
// )

#import "../../lib.typ": fr_table
#import "../../lib.typ": use_case

#use_case(
  id: "UC01",
  name: "Install Game",
  persona: "Casual Player & Dedicated Player",
  priority: "High - Required for MVP",
  description: "The player can install the game. The installation requires no technical skills.",
  uc_caption: "UC01 - Install Game",
)
#use_case(
  id: "UC02",
  name: "Save Game",
  persona: "Casual Player & Dedicated Player",
  priority: "High - Required for MVP",
  description: "When the player stops playing, the game saves his save state in a file and loads it when he resumes the game.",
  uc_caption: "UC02 - Save Game",
)
#use_case(
  id: "UC03",
  name: "Break Nodes",
  persona: "Casual Player & Dedicated Player",
  priority: "High - Required for MVP",
  description: "The player can break Nodes in the world with his hands or with tools. The usage of tools e.g. an axe to break a tree, increases the speed significantly. When the Nodes break, they drop items depending on the type of Node. E.g. a Tree drops a Wood-Item. Certain Nodes that are built by the player themselves such as a workbench drop themselves upon breaking.",
  uc_caption: "UC03 - Break Nodes",
)
#use_case(
  id: "UC04",
  name: "Expand Map",
  persona: "Casual Player & Dedicated Player",
  priority: "High - Required for MVP",
  description: "The player is confined to only move on tiles that are unlocked at a given point. The player can drop off items at the GlowCore, whereupon the map expands. The player can then move into and explore the new area.",
  uc_caption: "UC04 - Expand Map",
)
#use_case(
  id: "UC05",
  name: "Store Items",
  persona: "Casual Player & Dedicated Player",
  priority: "High - Required for MVP",
  description: "Items that the player picks up are stored in his inventory. The player's inventory has a limited size. To organize items, the player can move items into chests and take them out again.",
  uc_caption: "UC05 - Store Items",
)
#use_case(
  id: "UC06",
  name: "Craft Items",
  persona: "Casual Player & Dedicated Player",
  priority: "High - Required for MVP",
  description: "The player can craft items into other items with predefined recipes. Without a workbench the player is limited to a few simple recipes. The player can interact with a workstation such as a workbench to unlock more crafting recipes. Workstations such as an oven require additional items: e.g. Coal to smelt an Ingot.",
  uc_caption: "UC06 - Craft Items",
)
#use_case(
  id: "UC07",
  name: "Build Nodes",
  persona: "Casual Player & Dedicated Player",
  priority: "High - Required for MVP",
  description: "The player can place certain items onto the world, where they become Nodes. Each Tile can hold only one Node, and the player is not allowed to build a Node on an already occupied Tile.",
  uc_caption: "UC07 - Build Nodes",
)
#use_case(
  id: "UC08",
  name: "Experience Story",
  persona: "Casual Player",
  priority: "Low",
  description: "The player can experience a story while playing the game. When the game starts there is an intro sequence. There is an end to the game, at which point a cutscene is played. The game contains small narrative hints that fit into the story conveyed by the GlowCore-World, including the acoustic, the visual art style and the overall gameplay experience.",
  uc_caption: "UC08 - Experience Story",
)
#use_case(
  id: "UC09",
  name: "Fight Enemies",
  persona: "Casual Player & Dedicated Player",
  priority: "Middle",
  description: "The player can engage in combat with enemies. Defeated enemies drop items that are useful for the player. When the enemy hits the player, he loses hitpoints. When the player has no hitpoints left, he loses some of his items and respawns near the center of the map.",
  uc_caption: "UC09 - Fight Enemies",
)
#use_case(
  id: "UC10",
  name: "Automate Resource Gathering",
  persona: "Dedicated Player",
  priority: "Middle",
  description: "The player can automate tedious tasks, such as breaking Nodes. He can craft machines that are placed as Nodes and they automatically perform a task e.g. chopping down trees. There is a selection of machines that can be combined to automate more and more. Example: The player places multiple Replanter-Machines in a row which automatically replant trees on the neighbouring tile. The player places an Axe-Machine onto the start of the line, at which point the Axe-Machine chops one tree after another and then moves onto the next tile. To automate it even further, the player places Rotaters on both which cause the Axe-Machine to turn around on contact.",
  uc_caption: "UC10 - Automate Resource Gathering",
)
#use_case(
  id: "UC11",
  name: "Change Settings",
  persona: "Dedicated Player",
  priority: "Middle",
  description: "The player can change settings of the game, such as the FOV and the Volume.",
  uc_caption: "UC11 - Change Settings",
)


// TODO: Include at least:
// - The actors of the system under development (SUD)
// - The goals of each actor
// - A use case diagram (overview of actors, use cases, and relationships)
// - Use case descriptions (brief, casual, or fully-dressed format) -brief
// - Tip: Use identifiers such as 'UC1', 'UC2', ... as references

=== Non-Functional Requirements

// TODO: Include:
// - Choose an appropriate form (ISO/IEC 25010:2011, FURPS, ...) for your NFRs
// - Concentrate on NFRs most relevant or special to the SUD
// - Make sure NFRs are verifiable (acceptance criteria)
// - Document how and when you plan to verify them
// - Tip: Use identifiers such as 'NFR1', 'NFR2', ... as references
