== Architecture


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
We use the Singleton Pattern where appropriate. In our game it definitively makes sense because we have systems that only exist once. For example there is only one Player and only one World-Grid.

==== Dependency Injection
Games often have tightly coupled systems. To counter this we use dependency injection wherever appropriate to decrease coupling.

==== Scriptable Objects
In our game we have to define and store data about Items and Nodes in a organized way. Meant are the Item/Node-Definitions, not the instances at runtime. For example for the Wood-Item we need to store its ID, 3D Model, Max Stack-Size etc..

Scriptable Objects solve this problem. Scriptable Objects are objects that are not bound to a GameObject. Instances of ScriptableObjects can be saved as *.asset* files. This improves Version Control for Items/Nodes since they are saved as individual files.

==== Prefabs
We use Prefabs to save specific GameObjects as *.prefab* files. This enables us to dynamically Instantiate Prefabs as GameObjects during runtime, which is necessary for e.g. Building.

#pagebreak()
=== Extensibility
Our game is designed to be extensible and exchangeable wherever feasible. Thanks to our ScriptableObject Architecture we can easily add new Items/Nodes just by creating a new Instance of the corresponding ScriptableObject and setting the parameters and referencing the 3D Model and Icon.

Thanks to the Component-Based Architecture we can easily add new Items that perform a specific action when used by the players. It would be easy to add for example Consumables. An additional benefit that it is easy to add Nodes with custom behaviour for when the player interacts with it. For example you could just create a Signpost component and add it to a Node to create a Node where you can write something on it and read it.

We save our 3D Models separate from the ScriptableObjects in the .fbx format to improve exchangeability. It is possible to only exchange the 3D Model without touching the ScriptableObject.

A major limit of our architecture that we are strongly tied to Unity. Switching to another game engine would require a significant effort due to how our systems are relying on Unity's systems.



// TODO: Describe the architecture of your software as covered in SEP2.
// The main goal: a new team member can start working on the product as fast as possible.
//
// - Use an existing template as a starting point (arc42, C4 model, ...)
// - Focus on stable, high-level concepts rather than details
// - Cover different views (static, dynamic, deployment, ...)
// - Prefer diagrams over text (ideally UML)
// - Explain the reasons behind your decisions: "Why did we build it like this?"
