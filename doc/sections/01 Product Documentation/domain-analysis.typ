== Domain Analysis

Below is the domain model diagram for the problem domain of our project. Due to the nature of this project, the domain is modeled using an entity-relationship diagram, which captures the key entities and their relationships within the system. The diagram provides a high-level overview of the main components and their interactions, which is essential for understanding the structure of the problem domain.

#align(center)[
  #figure(
    image("../../resources/02 Project Documentation/domain-model.png"),
    caption: [Domain Model Diagram],
    supplement: [Image],
  )
]

One of the key aspects of the domain is the relationship between "Nodes" and "Items". Some items can be placed in the world which turns them into Nodes. This means that the same concept can have different representations and behaviors depending on the context.

=== Enemy State Machine

The below diagram represents the state machine for the enemy behavior in our project. It captures the various states an enemy can be in as well as the transitions between those states and the conditions that trigger those transitions.

#align(center)[
  #figure(
    image("../../resources/02 Project Documentation/enemy-state-machine.png"),
    caption: [Enemy State Machine Diagram],
    supplement: [Image],
  )
]
