== Requirements

=== Functional Requirements
==== Actors / Personas
Since a game typically only has one primary actor - *The Player* - it makes sense to define Personas that reflect different types of players. The categorization into Casual- and Dedicated Players covers most player types. In the following, the pronoun *he* is used for simplicity, but the Personas are meant to include all players.



===== Casual Player
- The Casual Player plays GlowCore infrequently and for short amounts of time. He wants to relax and unwind from his life. He prefers to play the game just for fun without putting effort into it.
- The Casual Player wants to experience the game from beginning to end with a story and explore the world. When he plays the game for the first time, he depends on the game to explain the rules of the game to him without being too overwhelming and tedious. As soon as he finishes the game, he will be done with it, having experienced everything he wanted.
- The Casual Player never changes settings in games, he never feels the need to customize anything. He likes to play the game as it is.

===== Dedicated Player
- The Dedicated Player plays GlowCore in long sessions. He loves games where you build factories and automate everything. He searches for the feeling of progression and finds satisfaction in it. He always searches for a better way to progress and optimize his gameplay.
- The Dedicated Player has already played the game multiple times before, thus he doesn't want to be bored with a long tutorial everytime he starts a new game. He wants to be able to dictate the pacing of the game.
- He doesn't care for the story and lore in games. He doesn't want to read through dialogues and doesn't like long cutscenes.

==== Use Case Diagram
The diagram shows a modified version of the Use Case Diagram. The Personas take the place of the Actors. An arrow indicates that a Persona uses a feature. A missing arrow means that the Persona is not interested in using a feature, although it remains accessible to him.

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

#show ref: it => {
  if it.element != none and it.element.func() == figure {
    link(it.target, str(it.target))
  } else {
    it
  }
}

// ── UC Status Registry ──
// Update the status color here once — it syncs to both the overview table and the detail table.
// Colors: blue (not tested), green (passed), orange (partial), red (failed), gray (not applicable)
#let uc-status = (
  uc01: green,
  uc02: green,
  uc03: green,
  uc04: green,
  uc05: green,
  uc06: green,
  uc07: green,
  uc08: red,
  uc09: gray,
  uc10: green,
  uc11: green,
)

// Helper to render a status box from the registry
#let uc-status-box(id) = box(width: 10pt, height: 10pt, fill: uc-status.at(id))

Each Use Case table includes a colored status indicator in the top-right corner representing its current verification state:

#figure(
  table(
    columns: (0.4fr, 1fr),
    stroke: 0.5pt + gray,
    align: left,
    fill: (x, y) => if y == 0 { luma(230) },
    [*Color*], [*Status*],
    [#box(width: 12pt, height: 12pt, fill: blue)], [Not yet verified (default)],
    [#box(width: 12pt, height: 12pt, fill: green)], [Implemented -- use case fully functional and verified],
    [#box(width: 12pt, height: 12pt, fill: orange)], [Partially implemented -- some aspects functional, others pending],
    [#box(width: 12pt, height: 12pt, fill: red)], [Not implemented -- use case not yet functional],
    [#box(width: 12pt, height: 12pt, fill: gray)], [N/A -- decided not to implement],
  ),
  caption: [UC Status Color Legend],
  supplement: [Table],
)

==== UC Overview

The following table provides a compact overview of all Use Cases, their persona, priority, and current verification status.

#figure(
  table(
    columns: (0.5fr, 1.8fr, 1.6fr, 1.2fr, 1.2fr, 0.5fr),
    stroke: 0.5pt + gray,
    align: left,
    fill: (x, y) => if y == 0 { luma(230) },
    [*ID*], [*Name*], [*Persona*], [*Priority*], [*Verify at*], [*Status*],
    [@UC01], [Install Game], [Casual & Dedicated Player], [High -- MVP], [M09 Beta], [#uc-status-box("uc01")],
    [@UC02], [Save Game], [Casual & Dedicated Player], [High -- MVP], [M09 Beta], [#uc-status-box("uc02")],
    [@UC03], [Break Nodes], [Casual & Dedicated Player], [High -- MVP], [M08 Alpha], [#uc-status-box("uc03")],
    [@UC04], [Expand Map], [Casual & Dedicated Player], [High -- MVP], [M08 Alpha], [#uc-status-box("uc04")],
    [@UC05], [Store Items], [Casual & Dedicated Player], [High -- MVP], [M08 Alpha], [#uc-status-box("uc05")],
    [@UC06], [Craft Items], [Casual & Dedicated Player], [High -- MVP], [M08 Alpha], [#uc-status-box("uc06")],
    [@UC07], [Build Nodes], [Casual & Dedicated Player], [High -- MVP], [M08 Alpha], [#uc-status-box("uc07")],
    [@UC08], [Experience Story], [Casual Player], [Low], [M10 Release], [#uc-status-box("uc08")],
    [@UC09], [Fight Enemies], [Casual & Dedicated Player], [Low], [M10 Release], [#uc-status-box("uc09")],
    [@UC10], [Automate Resource Gathering], [Dedicated Player], [Middle], [M10 Release], [#uc-status-box("uc10")],
    [@UC11], [Change Settings], [Dedicated Player], [Middle], [M09 Beta], [#uc-status-box("uc11")],
  ),
  caption: [UC Overview -- All Use Cases at a Glance],
  supplement: [Table],
)

#use_case(
  id: "UC01",
  name: "Install Game",
  persona: "Casual Player & Dedicated Player",
  priority: "High - Required for MVP",
  description: "The player can install the game. The installation requires no technical skills.",
  result: "The game can be installed on Windows by downloading the zip from the GitHub release and launching the .exe directly. For that no technical knowledge is required.",
  uc_caption: "UC01 - Install Game",
  status_color: uc-status.at("uc01"),
) <UC01>
#use_case(
  id: "UC02",
  name: "Save Game",
  persona: "Casual Player & Dedicated Player",
  priority: "High - Required for MVP",
  description: "When the player stops playing, the game saves his save state in a file and loads it when he resumes the game.",
  result: "The game autosaves every 20 seconds and the save is loaded correctly the next time the player starts the game. Inventory contents, chest contents, the GlowCore level and any blocks the player built or broke are all restored. The player can also save at any time from the pause menu, and the game saves automatically when returning to the main menu. If the player just closes the window without saving, the most recent autosave is loaded next time.",
  uc_caption: "UC02 - Save Game",
  status_color: uc-status.at("uc02"),
) <UC02>
#use_case(
  id: "UC03",
  name: "Break Nodes",
  persona: "Casual Player & Dedicated Player",
  priority: "High - Required for MVP",
  description: "The player can break Nodes in the world with tools. When the Nodes break, they drop items depending on the type of Node. E.g. a Tree drops a Wood-Item. Certain Nodes that are built by the player themselves such as a workbench drop themselves upon breaking.",
  result: "Verified manually. Trees drop wood when broken with an axe, stone nodes drop stone when broken with a pickaxe, and coal nodes drop both coal and stone. The crafting bench drops itself when broken with an axe. All acceptance criteria met.",
  uc_caption: "UC03 - Break Nodes",
  status_color: uc-status.at("uc03"),
) <UC03>
#use_case(
  id: "UC04",
  name: "Expand Map",
  persona: "Casual Player & Dedicated Player",
  priority: "High - Required for MVP",
  description: "The player is confined to only move on tiles that are unlocked at a given point. The player can drop off items at the GlowCore, whereupon the map expands. The player can then move into and explore the new area.",
  result: "Verified manually. The player is correctly confined to unlocked tiles and cannot move into locked areas. Feeding resources into the GlowCore triggers a map expansion, and the newly unlocked area becomes accessible immediately. All acceptance criteria met.

",
  uc_caption: "UC04 - Expand Map",
  status_color: uc-status.at("uc04"),
) <UC04>
#use_case(
  id: "UC05",
  name: "Store Items",
  persona: "Casual Player & Dedicated Player",
  priority: "High - Required for MVP",
  description: "Items that the player picks up are stored in his inventory. The player's inventory has a limited size. To organize items, the player can move items into chests and take them out again.",
  result: "Verified manually. Items picked up by the player are stored in his inventory, and the inventory has a limited size and refuses additional items when it is full. The player can open a chest by interacting with it, move items between the chest and his inventory by dragging them, and the chest closes automatically when he walks away. Chest contents are saved and restored correctly on the next start, and breaking a chest drops its remaining contents on the ground.",
  uc_caption: "UC05 - Store Items",
  status_color: uc-status.at("uc05"),
) <UC05>
#use_case(
  id: "UC06",
  name: "Craft Items",
  persona: "Casual Player & Dedicated Player",
  priority: "High - Required for MVP",
  description: "The player can craft items into other items with predefined recipes. Without a workbench the player is limited to a few simple recipes. The player can interact with a workstation such as a workbench to unlock more crafting recipes. Workstations such as an oven require additional items: e.g. Coal to smelt an Ingot.",
  result: "Verified manually. Without a workbench the player has access to a few basic recipes that can be crafted directly from the inventory. Interacting with a crafting table opens its crafting UI and unlocks the full set of standard recipes. A furnace is also available as a second workstation with its own recipe list, where the player can smelt ores into ingots and has other furnace specific recipes.",
  uc_caption: "UC06 - Craft Items",
  status_color: uc-status.at("uc06"),
) <UC06>
#use_case(
  id: "UC07",
  name: "Build Nodes",
  persona: "Casual Player & Dedicated Player",
  priority: "High - Required for MVP",
  description: "The player can place certain items onto the world, where they become Nodes. Each Tile can hold only one Node, and the player is not allowed to build a Node on an already occupied Tile.",
  result: "Verified manually. The player can place items such as crafting tables and chests onto the world grid, where they become Nodes. Placing a Node on an already occupied tile is correctly prevented. All acceptance criteria met.",
  uc_caption: "UC07 - Build Nodes",
  status_color: uc-status.at("uc07"),
) <UC07>
#use_case(
  id: "UC08",
  name: "Experience Story",
  persona: "Casual Player",
  priority: "Low",
  description: "The player can experience a story while playing the game. When the game starts there is an intro sequence. There is an end to the game, at which point a cutscene is played. The game contains small narrative hints that fit into the story conveyed by the GlowCore-World, including the acoustic, the visual art style and the overall gameplay experience.",
  result: "Not implemented. Due to time constraints, no intro sequence, cutscene, or narrative elements were built. The feature was intentionally deprioritised as a Low-priority item, and the team's effort was directed at the MVP and automation instead.",
  uc_caption: "UC08 - Experience Story",
  status_color: uc-status.at("uc08"),
) <UC08>
#use_case(
  id: "UC09",
  name: "Fight Enemies",
  persona: "Casual Player & Dedicated Player",
  priority: "Low",
  description: "The player can engage in combat with enemies. Defeated enemies drop items that are useful for the player. When the enemy hits the player, he loses hitpoints. When the player has no hitpoints left, he loses some of his items and respawns near the center of the map.",
  result: [],
  uc_caption: "UC09 - Fight Enemies",
  status_color: uc-status.at("uc09"),
) <UC09>
#use_case(
  id: "UC10",
  name: "Automate Resource Gathering",
  persona: "Dedicated Player",
  priority: "Middle",
  description: "The player can automate tedious tasks, such as breaking Nodes. He can craft machines that are placed as Nodes and they automatically perform a task e.g. chopping down trees. There is a selection of machines that can be combined to automate more and more. Example: The player places multiple Replanter-Machines in a row which automatically replant trees on the neighbouring tile. The player places an Axe-Machine onto the start of the line, at which point the Axe-Machine chops one tree after another and then moves onto the next tile. To automate it even further, the player places Rotaters on both which cause the Axe-Machine to turn around on contact.",
  result: "Verified manually. The player can craft three automation machines and place them as nodes. The Axe Machine faces a direction and automatically breaks the node in front of it using the appropriate tool. When the target tile is empty it drives forward to the next node. Drops are stored in the Axe Machine's built-in chest and the player can retrieve them at any time. The Replanter Machine plants saplings or other block items from its own chest onto the tile in front of it, automatically restocking resource nodes after the Axe Machine passes. Rotator nodes (Right, Down, Left) redirect the Axe Machine's heading when it drives into them, allowing the player to build looping or branching automation tracks. All three machines are craftable from the workbench. All acceptance criteria met.",
  uc_caption: "UC10 - Automate Resource Gathering",
  status_color: uc-status.at("uc10"),
) <UC10>
#use_case(
  id: "UC11",
  name: "Change Settings",
  persona: "Dedicated Player",
  priority: "Middle",
  description: "The player can change settings of the game, such as the FOV and the Volume.",
  result: "Verified manually. The player can open the settings from both the title screen and the in-game pause menu. The audio section has separate sliders for master, music and sound-effect volume. The display section has a fullscreen toggle, a resolution dropdown and separate camera sensitivity sliders for the X and Y axes. Changes are kept between sessions, and the player is asked to confirm before leaving the settings with unsaved changes.",
  uc_caption: "UC11 - Change Settings",
  status_color: uc-status.at("uc11"),
) <UC11>



=== Non-Functional Requirements

#import "../../lib.typ": nfr_table

// ── NFR Status Registry ──
// Update the status color here once — it syncs to both the overview table and the detail table.
// Colors: blue (not tested), green (passed), orange (partial), red (failed)
#let nfr-status = (
  nfr101: red,
  nfr102: gray,
  nfr103: green,
  nfr104: green,
  nfr105: green,
  nfr201: green,
  nfr202: green,
  nfr203: green,
  nfr204: orange,
  nfr205: green,
  nfr206: green,
  nfr301: green,
  nfr302: green,
  nfr303: green,
  nfr401: green,
  nfr402: green,
  nfr501: green,
  nfr502: green,
  nfr503: green,
)

// Helper to render a status box from the registry
#let status-box(id) = box(width: 10pt, height: 10pt, fill: nfr-status.at(id))

// ── Priority Matrix Colors ──
#let prio-req-high = rgb("#f2665c")    // red — critical MVP
#let prio-req-med = rgb("#ffa787")     // orange — important MVP
#let prio-opt-med = rgb("#fff987")     // yellow — post-MVP
#let prio-opt-low = rgb("#8fff87")     // green — nice to have
#let prio-empty = luma(245)            // light gray — no NFRs

This chapter documents the Non-Functional Requirements (NFRs) for the GlowCore project, categorized according to the ISO/IEC 25010:2011 quality model. The ISO/IEC 25010 standard defines a comprehensive set of software quality characteristics that serve as a framework for specifying and evaluating system quality. The following categories are used to structure our NFRs:

- *Performance Efficiency* -- NFR1XX
- *Usability* -- NFR2XX
- *Reliability* -- NFR3XX
- *Maintainability* -- NFR4XX
- *Portability* -- NFR5XX
\

Each NFR is documented with a description, concrete acceptance criteria, a measurement method, a verification approach, and a priority. The priority consists of two parts:

- *MVP Relevance*: \
  - *Required* -- must be fulfilled for the MVP.\
  - *Optional* -- planned for a later stage, not required for MVP delivery.
- *Importance*:
  - *High* -- critical for the product.
  - *Medium* -- important but not blocking.
  - *Low* -- nice to have.

\

Each NFR table includes a colored status indicator in the top-right corner representing its current verification state:

#figure(
  table(
    columns: (0.4fr, 1fr),
    stroke: 0.5pt + gray,
    align: left,
    fill: (x, y) => if y == 0 { luma(230) },
    [*Color*], [*Status*],
    [#box(width: 12pt, height: 12pt, fill: blue)], [Not yet tested (default)],
    [#box(width: 12pt, height: 12pt, fill: green)], [Passed -- all acceptance criteria met],
    [#box(width: 12pt, height: 12pt, fill: orange)], [Partially met -- some criteria passed, others pending],
    [#box(width: 12pt, height: 12pt, fill: red)], [Failed -- acceptance criteria not met],
    [#box(width: 12pt, height: 12pt, fill: gray)], [N/A -- decided not to implement],
  ),
  caption: [NFR Status Color Legend],
  supplement: [Table],
)

==== NFR Overview

The following table provides a compact overview of all non-functional requirements, their category, priority, verification milestone, and current status. This serves as a quick reference before diving into the detailed NFR descriptions below.

#figure(
  table(
    columns: (0.6fr, 1.8fr, 1.2fr, 1.2fr, 1.2fr, 0.5fr),
    stroke: 0.5pt + gray,
    align: left,
    fill: (x, y) => if y == 0 { luma(230) },
    [*ID*], [*Name*], [*Category*], [*Priority*], [*Verify at*], [*Status*],
    [@NFR101], [World Load Time], [Performance], [Required / High], [M09 Beta], [#status-box("nfr101")],
    [@NFR102], [In-Game Transition Time], [Performance], [Required / High], [M08 Alpha], [#status-box("nfr102")],
    [@NFR103], [Frame Rate], [Performance], [Required / High], [M08 Alpha], [#status-box("nfr103")],
    [@NFR104], [Save File Size], [Performance], [Required /\ Medium], [M09 Beta], [#status-box("nfr104")],
    [@NFR105], [Minimal Hardware Req.], [Performance], [Optional /\ Medium], [M09 Beta], [#status-box("nfr105")],
    [@NFR201], [Player Restriction Visibility], [Usability], [Required / High], [M08 Alpha], [#status-box("nfr201")],
    [@NFR202], [Light Upgrade Progress], [Usability], [Required / High], [M08 Alpha], [#status-box("nfr202")],
    [@NFR203], [New Player Learnability], [Usability], [Required / High], [M09 Beta], [#status-box("nfr203")],
    [@NFR204], [Input Support], [Usability], [Optional /\ Medium], [M09 Beta], [#status-box("nfr204")],
    [@NFR205], [Game Language], [Usability], [Optional / Low], [M10 Release], [#status-box("nfr205")],
    [@NFR206], [Visual Style Consistency], [Usability], [Optional /\ Medium], [M09 Beta], [#status-box("nfr206")],
    [@NFR301], [Save File Portability], [Reliability], [Required / High], [M09 Beta], [#status-box("nfr301")],
    [@NFR302], [Game Progress Persistence], [Reliability], [Required / High], [M09 Beta], [#status-box("nfr302")],
    [@NFR303], [Save Data Integrity], [Reliability], [Required / High], [M09 Beta], [#status-box("nfr303")],
    [@NFR401],
    [Extensibility of Game\ Systems],
    [Maintainability],
    [Optional /\ Medium],
    [M09 Beta],
    [#status-box("nfr401")],

    [@NFR402], [Automated Test Coverage], [Maintainability], [Required /\ Medium], [M09 Beta], [#status-box("nfr402")],
    [@NFR501], [Multi-Platform Support], [Portability], [Required /\ Medium], [M09 Beta], [#status-box("nfr501")],
    [@NFR502],
    [Engine and Rendering\, Pipeline],
    [Portability],
    [Optional / Low],
    [M10 Release],
    [#status-box("nfr502")],

    [@NFR503],
    [Display Resolution Support],
    [Portability],
    [Optional /\ Medium],
    [M10 Release],
    [#status-box("nfr503")],
  ),
  caption: [NFR Overview -- All Non-Functional Requirements at a Glance],
  supplement: [Table],
)


Verification is tied to project milestones (see @UpdatedMilestones) rather than fixed dates to ensure NFRs are checked at meaningful delivery checkpoints:

- *M08 -- Alpha Release (10.04.2026)*: All Required / High NFRs must pass. This is the MVP gate.
- *M09 -- Beta Release (15.05.2026)*: Required / Medium and Optional / Medium NFRs are verified. Performance tuning and playtesting rounds.
- *M10 -- Official Release (05.06.2026)*: Remaining Optional / Low NFRs are verified. Final polish and compliance check.

==== Priority Matrix

The priority matrix visualizes the distribution of NFRs across the two priority dimensions. This helps identify where the project's quality focus lies and whether the prioritization is balanced.

#figure(
  table(
    columns: (0.8fr, 1.6fr, 1.6fr),
    stroke: 0.5pt + gray,
    align: left,
    fill: (x, y) => {
      if y == 0 or x == 0 { luma(230) } else if x == 1 and y == 1 { prio-req-high } else if x == 2 and y == 1 {
        prio-empty
      } else if x == 1 and y == 2 { prio-req-med } else if x == 2 and y == 2 { prio-opt-med } else if (
        x == 1 and y == 3
      ) { prio-empty } else if x == 2 and y == 3 { prio-opt-low }
    },
    [], [*Required (MVP)*], [*Optional (Post-MVP)*],
    [*High*], [NFR101, NFR102, NFR103 \ NFR201, NFR202, NFR203 \ NFR301, NFR302, NFR303], [--],
    [*Medium*], [NFR104, NFR402, NFR501], [NFR105, NFR204, NFR206 \ NFR401, NFR503],
    [*Low*], [--], [NFR205, NFR502],
  ),
  caption: [NFR Priority Matrix -- MVP Relevance vs. Importance],
  supplement: [Table],
)

The matrix shows that the core quality effort is concentrated in the *Required / High* cell (9 NFRs), covering performance, usability, and reliability which are the three pillars that directly impact the player experience in the MVP. The *Optional / Low* cell contains only 2 NFRs that represent project constraints rather than testable quality goals. Priority is on delivering a stable, playable MVP before addressing optional requirements.

==== Performance Efficiency

Performance Efficiency addresses the amount of resources used under stated conditions. For GlowCore, this primarily concerns load times, frame rates, and resource consumption to ensure a smooth and responsive gameplay experience.


#nfr_table(
  id: [NFR101],
  description: [World Load Time],
  requirements: [Existing and newly created worlds must load within 3 seconds when the player presses "Start".],
  priority: [Required / High],
  measurement: [Measure elapsed time from pressing "Start" until the world is fully loaded and interactive. Tested on minimum hardware specification.],
  verification: [Automated load-time benchmark test across 10 consecutive loads with varying world states (new and existing). All runs must complete within 3 seconds.],
  result: [Failed. Tested manually on the Windows build. A new world takes about 4 seconds, a small save about 6 seconds, and a late-game save up to 20 seconds. The world stays the same size, but the player fills it with placed and planted things, and all of that has to load. The more a save grows, the longer it takes. The 3-second target is not met. When we set this target, we did not expect to build a game of this size, so the original goal no longer fits what we ended up with. Bringing it back in range would need extra work on how the world is loaded.],
  nfr_caption: [NFR101 -- World Load Time],
  status_color: nfr-status.at("nfr101"),
) <NFR101>

#nfr_table(
  id: [NFR102],
  description: [In-Game Transition Time],
  requirements: [In-game transitions (e.g. walking into a cave) shall not exceed 2 seconds.],
  priority: [Required / High],
  measurement: [Measure elapsed time from triggering the transition until the new scene is fully loaded and interactive.],
  verification: [Trigger each scene transition during playtesting and record the load time. Automated scene-transition benchmark covering all transition points.],
  result: "Not applicable. Due to the lack of time the team decided not to implement cave areas or any scene transitions during the project. This requirement no longer applies to the current scope of GlowCore.",
  nfr_caption: [NFR102 -- In-Game Transition Time],
  status_color: nfr-status.at("nfr102"),
) <NFR102>

#nfr_table(
  id: [NFR103],
  description: [Frame Rate],
  requirements: [The game must run at a minimum of 60 FPS with 1% lows not dropping below 50 FPS on recommended hardware.],
  priority: [Required / High],
  measurement: [Use Unity Profiler or an external frame-time analysis tool to record FPS over a 5-minute gameplay session. Evaluate average FPS and 1% low values.],
  verification: [Run a standardized gameplay scenario on minimum and recommended hardware. Capture FPS metrics and verify thresholds are met.],
  result: "Manually tested on WebGL build using Chrome's Frame Rendering Stats overlay. The game ran at a stable 120 FPS during active gameplay with no significant drops observed. Exceeds the minimum threshold of 60 FPS and the 1% low threshold of 50 FPS. All acceptance criteria met.",
  nfr_caption: [NFR103 -- Frame Rate],
  status_color: nfr-status.at("nfr103"),
)<NFR103>

#nfr_table(
  id: [NFR104],
  description: [Save File Size],
  requirements: [Save files should remain compact and use a binary serialization format. Under the currently defined maximum expected game progression and world configuration, a save file should not exceed 5MB.],
  priority: [Required / Medium],
  measurement: [Measure the file size of the save file after a full gameplay session under the defined maximum expected game progression and world configuration.],
  verification: [Create save files at various progression stages and verify the file size remains within the defined limit for the current scope. Confirm that a binary serialization format is used.],
  result: [Tested by playing the game through several progression stages (fresh world, a few broken nodes, multiple chests filled with items, several built blocks) and measuring the on-disk size of the save file after each save. A fresh save was around 1KB, and a save taken after reaching the maximum GlowCore level peaked at 3KB, well under the 5MB limit. The save file is written in a custom binary format, as required.],
  nfr_caption: [NFR104 -- Save File Size],
  status_color: nfr-status.at("nfr104"),
)<NFR104>

#nfr_table(
  id: [NFR105],
  description: [Minimal Hardware Requirements],
  requirements: [The game must run on minimal hardware with the following specifications:
    - 2 GHz CPU
    - 8 GB RAM
    - NVIDIA GeForce GTX 1660
  ],
  priority: [Optional / Medium],
  measurement: [Test the game on a defined minimum hardware baseline and verify all performance NFRs (FPS, load times) are met.],
  verification: [Execute a full gameplay session on minimum-spec hardware. Record FPS, load times, and memory usage to confirm compliance.],
  result: [Tested on a Microsoft Surface Pro 8 running on integrated Intel Iris Xe graphics with no dedicated GPU. The game ran smoothly throughout the entire session with no frame drops or lag. FPS stayed consistently high and load times were near instant. Memory usage remained stable with no noticeable spikes. For a game of this scope running on integrated graphics only, the minimal hardware requirement is met.],
  nfr_caption: [NFR105 -- Minimal Hardware Requirements],
  status_color: nfr-status.at("nfr105"),
)<NFR105>

==== Usability

Usability covers the degree to which the product can be used effectively, efficiently, and satisfactorily. For GlowCore, this includes visual clarity of game mechanics, learnability for new players, and input support.

#nfr_table(
  id: [NFR201],
  description: [Player Restriction Visibility],
  requirements: [Player restrictions in the game must be clearly communicated through visual indicators such as greyed-out or red-tinted elements. Players must understand restrictions without additional explanation.],
  priority: [Required / High],
  measurement: [Conduct playtesting with at least 3 new players. Each player must correctly identify restricted actions without external guidance.],
  verification: [During playtesting sessions, observe whether players attempt restricted actions and whether they understand the visual cues. Document success rate.],
  result: [User testing was conducted. The locked area border is visually communicated to the player. There is a progress indicator when breaking nodes and an interaction prompt when hovering over workstations such as the crafting table. All acceptance criteria met.],
  nfr_caption: [NFR201 -- Player Restriction Visibility],
  status_color: nfr-status.at("nfr201"),
)<NFR201>

#nfr_table(
  id: [NFR202],
  description: [Light Upgrade Progress Indication],
  requirements: [Light upgrade progress must be clearly indicated to the player at all times during gameplay.],
  priority: [Required / High],
  measurement: [During playtesting, ask players to describe their current upgrade progress. At least 80% must answer correctly.],
  verification: [Conduct playtesting sessions and survey players about their perceived progress. Verify the UI communicates upgrade status unambiguously.],
  result: "User Testing was conducted. The GlowCore upgrade UI clearly displays the current level, progress bar with percentage, required materials and amounts, a preview of the next level appearance, and the effect of the next upgrade. All information updates in real time as materials are fed. All acceptance criteria met.",
  nfr_caption: [NFR202 -- Light Upgrade Progress Indication],
  status_color: nfr-status.at("nfr202"),
)<NFR202>

#nfr_table(
  id: [NFR203],
  description: [New Player Learnability],
  requirements: [A new player shall understand core mechanics (movement, gathering, upgrading) within 5 minutes without external instructions.],
  priority: [Required / High],
  measurement: [Time how long a new player takes to perform their first gather, move to a new area, and initiate an upgrade without any external help.],
  verification: [Conduct playtesting with at least 3 players who have never seen the game. Measure time to complete core actions. All players must achieve this within 5 minutes.],
  result: [User testing was conducted. All participants were able to upgrade the GlowCore multiple times in under 5 minutes. They learned the core mechanic quickly and were able progress without external help.],
  nfr_caption: [NFR203 -- New Player Learnability],
  status_color: nfr-status.at("nfr203"),
)<NFR203>

#nfr_table(
  id: [NFR204],
  description: [Input Support],
  requirements: [The game must support both controller and keyboard input methods.],
  priority: [Optional / Medium],
  measurement: [Test all core gameplay actions with both a standard controller (e.g. Xbox controller) and keyboard. All actions must be executable with both input methods.],
  verification: [Perform a full gameplay session using only controller, then only keyboard. Verify all interactions, menus, and gameplay mechanics are fully accessible with each input method.],
  result: [Keyboard input works for all core gameplay actions. Controller support was not implemented due to time constraints. Since the game uses Unity's new Input System, all input actions are already defined and adding controller bindings would not require significant effort.],
  nfr_caption: [NFR204 -- Input Support],
  status_color: nfr-status.at("nfr204"),
)<NFR204>

#nfr_table(
  id: [NFR205],
  description: [Game Language],
  requirements: [All in-game text, UI elements, and instructions must be in English.],
  priority: [Optional / Low],
  measurement: [Review all in-game text and UI elements for non-English content.],
  verification: [Manual review of all text-containing assets and UI screens to confirm English language throughout.],
  result: [Verified by walking through every screen of the game (title screen, new game, pause menu, settings, inventory, crafting and chest UIs) and reviewing all visible text. All labels, buttons, item names, recipe names and tooltips are in English. No localized or translated strings are present.],
  nfr_caption: [NFR205 -- Game Language],
  status_color: nfr-status.at("nfr205"),
)<NFR205>

#nfr_table(
  id: [NFR206],
  description: [Visual Style Consistency],
  requirements: [All visual assets must follow the low-poly pastel art style consistently throughout the game.],
  priority: [Optional / Medium],
  measurement: [Visual review of all assets against the defined art style guide. No asset shall deviate from the low-poly pastel aesthetic.],
  verification: [Conduct an art review session where all in-game assets are compared against reference material. Flag and correct any deviations.],
  result: [Went through all in-game assets during a gameplay session and compared them against our reference material. Tiles, trees, rocks, the player character, placed nodes, and UI elements all follow the low-poly pastel aesthetic consistently. No asset stood out as visually inconsistent. The color palette stays coherent throughout and nothing looks out of place.],
  nfr_caption: [NFR206 -- Visual Style Consistency],
  status_color: nfr-status.at("nfr206"),
)<NFR206>

==== Reliability

Reliability addresses the degree to which a system performs specified functions under specified conditions for a specified period of time. For GlowCore, this is critical for save data integrity and game stability.

#nfr_table(
  id: [NFR301],
  description: [Save File Portability],
  requirements: [Each world save must be stored as a single file that can be copied and moved. When placed into the game's save file folder, the save file must be recognized and function without issues.],
  priority: [Required / High],
  measurement: [Copy a world save file to a different machine or location, place it in the save folder, and verify the game loads it correctly with all progress intact.],
  verification: [Create a world save file with significant progress. Copy it to a new installation. Load the game and verify all progress, items, and world state for that world are preserved.],
  result: [Tested manually by copying the save file (glowcore.bin) out of the save folder on one machine and dropping it into the save folder on a different installation. The save was checked after every scenario (moving the player, filling inventory and chests, breaking and building blocks, upgrading the GlowCore) and in each case the game recognized the file and restored everything correctly: player position, inventory contents, chest contents, GlowCore level and all blocks the player had built or broken. The save is a single self-contained file, so copying and moving it works without any additional steps.],
  nfr_caption: [NFR301 -- Save File Portability],
  status_color: nfr-status.at("nfr301"),
)<NFR301>

#nfr_table(
  id: [NFR302],
  description: [Game Progress Persistence],
  requirements: [Game progress shall not be lost during normal shutdown. Auto-save shall occur at least every 2 minutes or immediately after a light upgrade.],
  priority: [Required / High],
  measurement: [Verify auto-save triggers by monitoring save file timestamps during gameplay. Confirm no data loss after normal shutdown.],
  verification: [Play for several minutes, perform a light upgrade, then shut down the game normally. Restart and verify all progress is retained. Additionally, verify auto-save timestamps occur within 2-minute intervals.],
  result: [Tested manually by playing several sessions and watching the save file timestamp. The autosave now fires every 20 seconds and keeps firing for the entire session, comfortably below the 2-minute limit required. After a normal shutdown and restart, the player position, inventory contents, chest contents, GlowCore level and all built or broken blocks were preserved. The worst case for unsaved progress is the 20 seconds since the last autosave, which is well within what the requirement allows.],
  nfr_caption: [NFR302 -- Game Progress Persistence],
  status_color: nfr-status.at("nfr302"),
)<NFR302>

#nfr_table(
  id: [NFR303],
  description: [Save Data Integrity],
  requirements: [Save data must persist correctly across sessions without corruption.],
  priority: [Required / High],
  measurement: [Perform several manual save/load cycles and verify that the game state after loading matches the state at the time of saving.],
  verification: [Play the game, note the current state (inventory, player position, placed blocks), let the autosave trigger, restart the game, and verify the loaded state matches what was saved.],
  result: [Tested manually by playing the game, noting the current state (player position, inventory contents, chest contents, GlowCore level and all placed or broken blocks), letting the autosave trigger, restarting the game, and comparing the loaded state to what was noted before. Across multiple save and load cycles the loaded state matched the saved state exactly in every case, with no missing items, misplaced blocks or corrupted data.],
  nfr_caption: [NFR303 -- Save Data Integrity],
  status_color: nfr-status.at("nfr303"),
)<NFR303>

==== Maintainability

Maintainability represents the degree of effectiveness and efficiency with which a product can be modified. For GlowCore, this covers extensibility of game systems and test coverage.

#nfr_table(
  id: [NFR401],
  description: [Extensibility of Game Systems],
  requirements: [The code must allow adding new LightCore upgrades and new items with their resource nodes without requiring code restructuring.],
  priority: [Optional / Medium],
  measurement: [A developer unfamiliar with the codebase must be able to add a new upgrade or item by following existing patterns, without modifying core system code.],
  verification: [Task a team member with adding a new test upgrade and resource node. Measure the time required and verify no structural code changes were necessary.],
  result: "Verified manually by a team member adding a new resource node. The architecture supports adding new nodes and items without modifying core system code.",
  nfr_caption: [NFR401 -- Extensibility of Game Systems],
  status_color: nfr-status.at("nfr401"),
)<NFR401>

#nfr_table(
  id: [NFR402],
  description: [Automated Test Coverage],
  requirements: [All systems must be covered by automated tests. The project targets a minimum of 40% code coverage. (This coverage target is relatively common for video games)],
  priority: [Required / Medium],
  measurement: [Measure code coverage using the Unity Code Coverage package. Report overall and per-system coverage percentages.],
  verification: [Run the full automated test suite and generate a coverage report. Verify that overall coverage meets or exceeds 40%.],
  result: [Coverage was measured using the Unity Code Coverage package. The project achieves 43.1% method coverage, exceeding the 40% target. Line coverage sits at 38.9%.],
  nfr_caption: [NFR402 -- Automated Test Coverage],
  status_color: nfr-status.at("nfr402"),
)<NFR402>

==== Portability

Portability addresses the degree of effectiveness and efficiency with which a system can be transferred from one environment to another. For GlowCore, this covers platform support and display resolution compatibility.

#nfr_table(
  id: [NFR501],
  description: [Multi-Platform Support],
  requirements: [The game must run on Windows, Linux, and Web platforms.],
  priority: [Required / Medium],
  measurement: [Build and deploy the game on all three platforms. Launch and complete a full gameplay session on each.],
  verification: [Execute the full test suite and a manual gameplay session on Windows, Linux, and a Web browser. Verify all features work identically across platforms.],
  result: "Manually verified on WebGL (browser), Windows (native build via GitHub release zip), and Linux (native build tested in a Linux VM). The game launched and ran correctly on all three platforms. All three platform builds are generated automatically via the CI/CD pipeline. All acceptance criteria met.",
  nfr_caption: [NFR501 -- Multi-Platform Support],
  status_color: nfr-status.at("nfr501"),
) <NFR501>

#nfr_table(
  id: [NFR502],
  description: [Engine and Rendering Pipeline],
  requirements: [The game must run on the Unity engine using the PhysX physics engine and the Universal Render Pipeline (URP).],
  priority: [Optional / Low],
  measurement: [Verify project settings confirm Unity with PhysX and URP are configured as the active engine and rendering pipeline.],
  verification: [Review Unity project settings and confirm PhysX is the active physics engine and URP is the active render pipeline. Verify no fallback to built-in pipeline occurs during gameplay.],
  result: [Verified via Unity project settings and by running the game. The GraphicsSettings.asset confirms URP is the active render pipeline, and no fallback to the built-in pipeline was observed during gameplay. PhysX is Unity's default physics engine and is used throughout the project with no override configured. Both requirements are met.],
  nfr_caption: [NFR502 -- Engine and Rendering Pipeline],
  status_color: nfr-status.at("nfr502"),
) <NFR502>

#nfr_table(
  id: [NFR503],
  description: [Display Resolution Support],
  requirements: [The game must support common display resolutions with a 16:9 aspect ratio. Minimum: 1280×720. Recommended: 1920×1080, 2560×1440, and 3840×2160 (4K).],
  priority: [Optional / Medium],
  measurement: [Launch the game at each supported resolution and verify the UI scales correctly, no elements are clipped, and the game renders properly.],
  verification: [Test the game at all specified resolutions. Take screenshots and verify correct rendering, UI scaling, and absence of visual artifacts.],
  result: [Tested manually in windowed mode. The settings menu provides a resolution dropdown and switching between resolutions correctly resizes the window and scales the UI. All tested resolutions from 1280x720 up to 1920x1080 rendered correctly with no clipping or visual artifacts.],
  nfr_caption: [NFR503 -- Display Resolution Support],
  status_color: nfr-status.at("nfr503"),
) <NFR503>

#pagebreak()
==== NFR-Architecture Traceability

The table below links NFRs to the specific technical solution in the architecture that is primarily responsible for satisfying it. The purpose of this table is to make the connection between quality requirements and design decisions explicit and verifiable.

#figure(
  table(
    columns: (0.55fr, 1.2fr, 1.7fr, 1.9fr),
    inset: 5pt,
    align: (left, left, left, left),
    stroke: 0.5pt,
    fill: (x, y) => if y == 0 { luma(220) } else if calc.even(y) { luma(245) } else { white },
    [*NFR*], [*Quality Attribute*], [*Technical Solution*], [*How the Architecture Addresses It*],

    [@NFR401],
    [Maintainability\ (Extensibility)],
    [`IInteractable`, `IHandItem`,\ ScriptableObjects],
    [New nodes implement `IInteractable`; new usable items implement `IHandItem`; new content is added as ScriptableObject assets. No existing class needs to change.],

    [@NFR402],
    [Maintainability\ (Test Coverage)],
    [Plain C\# classes;\ interface-based design],
    [`Inventory` and `CraftingSystem` are plain C\# classes, not MonoBehaviours, so NUnit can test them without a Unity scene. The service interfaces enable mock injection in integration tests.],

    [@NFR501],
    [Portability\ (Multi-Platform)],
    [Unity, URP,\ New Input System],
    [Unity builds natively to Windows, Linux, and WebGL from one codebase. URP renders consistently across all three. The New Input System abstracts platform-specific input, requiring no per-platform code.],

    [@NFR502],
    [Portability\ (Rendering Pipeline)],
    [Universal Render Pipeline (URP)],
    [URP was chosen over the Built-in Pipeline (deprecated) and HDRP (too heavy). It supports cross-platform shaders and custom post-processing (e.g. `LowResViewport`) without HDRP's hardware requirements.],

    [@NFR103],
    [Performance\ (Frame Rate)],
    [URP; event-driven UI;\ Component-Based Architecture],
    [URP has a lower per-frame GPU cost than HDRP. The event-driven UI only runs when events like `OnSlotChanged` fire, never polling each frame. Component-Based Architecture avoids monolithic `Update()` methods.],

    [@NFR301\ @NFR302\ @NFR303],
    [Reliability\ (Save System)],
    [Binary serialization;\ ScriptableObject asset refs],
    [Binary serialization keeps file sizes small (NFR104). Items are stored as ScriptableObject references rather than copies, ensuring consistency across sessions. Auto-save triggers on key game events, not only on shutdown.],

    [@NFR201\ @NFR202\ @NFR203],
    [Usability\ (Feedback &\ Learnability)],
    [`GetActionPromptText()`;\ event-driven UI],
    [The Use/Interact abstraction reduces controls to two input types, lowering cognitive load (NFR203). Each node provides its own prompt via `GetActionPromptText()` (NFR201). The event-driven UI updates instantly on state changes, keeping upgrade indicators accurate (NFR202).],
  ),
  caption: [NFR-to-Architecture Traceability],
  supplement: [Table],
)
