== Savefile

The game periodically saves the player's progress in a savefile. This file contains all the necessary information to restore the game state when the player loads it again. It is stored as a binary file on the player's device. The following diagram illustrates the structure of the savefile:

#figure(
  image("../../resources/01 Product Documentation/savefile-structure.png"),
  caption: [Savefile Structure],
  supplement: [Image],
)

The savefile is organized into several sections, each containing specific types of data:
- *Header*: Contains metadata about the savefile, such as an identifier, version number, and offsets to subsequent sections.
- *Player Data*: Stores information about the player itself, including name, GlowCore level, inventory, etc.
- *World Data*: Contains information about the game world itself, such as where nodes have been placed or destroyed.

The world data is stored as a list of deltas, instead of a full snapshot of the world state. Each delta represents a change to the world from its initial state, such as placing or destroying a node. This approach allows for more efficient storage and faster loading times, as only the changes need to be recorded rather than the entire world state.
