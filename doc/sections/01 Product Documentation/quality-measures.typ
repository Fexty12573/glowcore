== Quality Measures

=== Working Environment

==== CI/CD Pipeline

The project uses GitHub Actions for all build, test, and deployment automation. Five pipelines are defined, each with a distinct responsibility:

#figure(
  image("../../resources/01 Product Documentation/deployment-diagram.png", width: 100%),
  caption: [CI/CD Pipeline Overview],
  supplement: [Image],
)

- *MAIN*: runs on every push to `main` or `dev` (excluding `doc/**` changes) and on version tags (`v*`). Builds and deploys the WebGL build for pushes to the two main branches. On a version tag it additionally compiles Windows and Linux standalone builds and creates a GitHub Release.

- *TEST*: runs on push to `main` or `dev` and on pull requests targeting either branch (both excluding `doc/**`). Executes the Unity Test Framework suite in both Edit Mode and Play Mode and generates a report on both results and coverage.

- *DOC*: triggered only when `doc/**` files are modified: on push to `main` or `dev`, on doc tags (`d*.*`), and on pull requests that touch the documentation. Builds the Typst documentation and deploys it to GitHub Pages.

- *LINTER*: runs on the same push and pull-request triggers as TEST (excluding `doc/**`). Runs the Roslyn code-analysis and `dotnet format` checks described in the Coding Guidelines section. A failed lint check blocks the merge.

- *LABELER*: fires on pull-request targets to `main` or `dev` (skipped for `dev` → `main` promotion PRs). Automatically applies labels to each PR based on branch prefix and changed paths (`feature/*` → _feature_, `bug/*` → _bug_, `docs/*` or changes under `doc/**/*` → _doc_).

#pagebreak()


==== Definition of Ready and Definition of Done

Every Jira ticket follows a structured lifecycle governed by our Definition of Ready (DoR) and Definition of Done (DoD). A ticket may only be pulled into a sprint once all DoR criteria are met, including a clear description, defined acceptance criteria, story point estimate, epic assignment, and team sign-off during sprint planning. A ticket is only considered done once all DoD criteria are satisfied, covering code quality, CI/CD pipeline status, test coverage, PR approval and, conditionally, UI consistency and documentation updates.

#figure(
  image("../../resources/01 Product Documentation/dor_dod_flowchart.drawio.png", width: 100%),
  caption: [Definition of Ready and Definition of Done - ticket lifecycle flowchart],
  supplement: [Image],
  placement: auto,
)

===== DoR Example

The following example shows a real ticket (GC-186) that satisfies all DoR criteria and is ready to be worked on.

#figure(
  image("../../resources/01 Product Documentation/DoR_Example.png", width: 100%),
  caption: [GC-186 - Example of a ticket satisfying all Definition of Ready criteria],
  supplement: [Image],
  placement: auto,
)

===== DoD Example

The following example shows the same ticket (GC-186) with all acceptance criteria checked and status set to Done.

#figure(
  image("../../resources/01 Product Documentation/DoD_Example.png", width: 100%),
  caption: [GC-186 - Example of a ticket satisfying all Definition of Done criteria],
  supplement: [Image],
  placement: auto,
)

=== Test Concept
Our testing strategy for GlowCore is fundamentally shaped by the nature of the project. Since GlowCore is a standalone Unity game built for Windows, Linux, and WebGL, we don't have any server connections, huge amount of data in a database or a frontend beyond simple game-menus. This allows us to focus our entire testing effort directly on the client-side application, ensuring gameplay mechanics, performance, and user experience are as solid as possible.

The test concept is divided into automated testing for our codebase, manual system verification, extensive usability testing, and targeted checks for our non-functional requirements (NFRs).

==== Automated Testing (Unit & Integration)
At the code level, we rely heavily on the Unity Test Framework (UTF). We use it to write both unit tests and integration tests that verify whether individual functionalities and interconnected systems are working correctly.

A core part of our workflow is our CI/CD pipeline. Our UTF tests are set up to run automatically on the pipeline every time someone pushes new code. Because these tests run continuously on the latest codebase, they effectively act as our regression tests. They give us immediate feedback if a new feature or bug fix accidentally breaks an older, already working part of the game. We are aiming for a minimum of 40% automated test coverage across our systems, which is a pretty standard and realistic target for game development projects.

==== System Testing
We don't do automated system tests. Fully automating the entire game loop in Unity is just very difficult. It usually results in tests that break way too easily, for example, just because a small visual asset was moved slightly.

Instead, system testing is handled manually. For this, we regularly take the automated WebGL build generated from the current pipeline state and compare it directly against a known, stable release build. This manual side-by-side verification helps us ensure that the game as a whole still compiles, runs, and functions as expected before we consider a new version stable.

==== Usability Testing
There is a strict limit to what we can test automatically in a game project. A script might tell you if a player's health goes down correctly, but it can't tell you if the game is actually fun or if the controls make sense. Because of this, usability testing is a massive aspect of our QA process.

We will conduct regular playtesting sessions where we let several new users play the game without giving them any prior instructions. Our main goal here is to find out what feels good, what is frustrating, and if the core mechanics are actually understandable. For instance, we want to see if a brand-new player can figure out how to move, gather resources, and upgrade within the first 5 minutes. We will also observe if players naturally understand the visual cues for restricted actions (like red-tinted UI elements) or if they can accurately track their light upgrade progress.

#pagebreak()

==== Non-Functional Requirements (NFR) Verification
NFR tests are not done once at the end, but at appropriate points throughout the project. Each NFR is assigned to a milestone at which it must be verified. Additionally, our Definition of Done requires that any change which could affect an NFR is accompanied by a re-verification of that NFR and an updated result in the documentation. This ensures NFR results stay current and are not just a snapshot from a single test session.

A huge portion of our testing isn't just about finding bugs, but verifying our NFRs. We have defined a wide spectrum of NFRs covering different quality attributes. They are sorted by priority, meaning that while our ultimate goal is to fulfill all of them, the ones marked as "Required" are strict targets we have to hit for the final submission.

We cover NFR testing through a mix of automated benchmarks and manual playtesting protocols:

- *Performance Efficiency:* We regularly check our performance manually to make sure the game runs smoothly. We aim to ensure that the world loads quickly, in-game scene transitions are fast, and the game maintains a solid 60 FPS on recommended hardware.

- *Reliability:* Since saving the game correctly is critical, we manually test save and load cycles to ensure data doesn't corrupt. We also verify that save files remain small enough (under 5MB), that they are fully portable between machines, and that auto-saves trigger correctly.

- *Portability & Maintainability:* We manually verify that the game runs identically across Windows, Linux, and WebGL, and that it supports standard 16:9 resolutions. From a code perspective, we constantly test extensibility during normal development. We frequently add new elements like resource nodes, items, or recipes based on existing systems. If our internal architecture was too complicated to extend, we would notice it very quickly.

- *Usability & Visuals:* Beyond the playtesting mentioned earlier, we also manually review the game's language to ensure all text is in English. We test controller and keyboard inputs by simply playing with both, and we hold art reviews to make sure all new assets fit our low-poly pastel style guide.

By combining the automated safety net of our UTF pipeline with heavy manual playtesting and strict NFR benchmarking, we believe this concept will keep the game stable while still allowing us to iterate quickly on the gameplay.

=== Unity Testing
Testing a game is a bit different from testing a "standard" software project, like a web-based accounting tool or a simple CRUD application. In a typical software project, you are mostly concerned with whether a specific input leads to a specific output in a database or a UI. You can easily mock external dependencies and call functions in isolation.

In Unity, however, our code doesn't just sit there. It lives inside an engine that is constantly ticking. We have to deal with the "Game Loop," physics calculations, and the complex lifecycle of MonoBehaviours. To test a full cycle in the Unity engine, we are able to use Edit Mode and Play Mode.

==== Edit Mode Tests
Edit Mode tests are the closest thing we have to "traditional" unit tests. They run entirely within the Unity Editor and do not require the game to actually start. Since they don't have to load scenes or wait for the physics engine to initialize, they are fast.

Edit Mode tests can be used for our "pure" logic things like calculating resource costs, managing inventory math, or utility functions that don't depend on the game's frame rate. If a bug appears in our math, an Edit Mode test will catch it during our CI/CD run without us ever having to open a game window.

#pagebreak()

==== Play Mode Tests
Play Mode tests are where we handle the Unity Engine of the project. Unlike Edit Mode, these tests actually trigger the full game engine. They can load specific test scenes, instantiate Prefabs, and, most importantly, they can run over multiple frames.

This is essential for Games because many mechanics are time dependent. For example, if we want to test if the player really moves on input, we need the game clock to actually run. In Play Mode, we can use yield return new WaitForSeconds(1); to let the game simulate for a moment before checking the results. Play Mode tests can be used for:

- Physics interactions
- Component Lifecycle
- Player Input

The downside is that Play Mode tests are significantly slower because they have to "play" the game. However, they give us the confidence that our code isn't just working on paper, but actually performing correctly within the simulated world of the engine.
#show ref: it => {
  if it.element != none and it.element.func() == figure {
    link(it.target, str(it.target))
  } else {
    it
  }
}

==== Coverage Summary

At the final stage of the project (M10 Release reached), 9 out of 11 Use Cases are fully verified (@UC01, @UC02, @UC03, @UC04, @UC05, @UC06, @UC07, @UC10, @UC11). @UC08 (Experience Story) was not implemented due to time constraints. @UC09 (Fight Enemies) is not applicable, since the team decided not to implement combat within the project scope.

For the NFRs, 16 requirements have fully passed (@NFR103, @NFR104, @NFR105, @NFR201, @NFR202, @NFR203, @NFR205, @NFR206, @NFR301, @NFR302, @NFR303, @NFR401, @NFR402, @NFR501, @NFR502, @NFR503), 1 is partially met (@NFR204), and 1 failed (@NFR101). @NFR102 is not applicable since in-game scene transitions were removed from scope. The partial NFR is input support: keyboard input works for all gameplay actions, but controller support was not implemented due to time constraints. @NFR101 (World Load Time) failed because load times grew with world content beyond what the 3-second target anticipated.


=== Code metrics
==== Lines of Code
#figure(
  image("../../resources/01 Product Documentation/loc-diagramm.png", width: 100%),
  caption: [Lines of Code over Time],
  supplement: [Image],
)

#pagebreak()

#{
  let data = csv("../../resources/01 Product Documentation/loc_files_final.csv")
    .flatten()
    .slice(4)
    .map(item => {
      if item.len() > 20 { item.slice(0, 13) + "..." } else { item }
    })

  grid(
    columns: 2,
    gutter: 0.5cm,
    align: left,
    table(
      fill: (x, y) => if y == 0 { luma(230) },
      stroke: 0.5pt + gray,
      columns: 4,
      table.header([File], [Blank], [Comment], [Code]),
      ..data.slice(0, calc.floor(data.len() / 2)),
    ),
    table(
      fill: (x, y) => if y == 0 { luma(230) },
      stroke: 0.5pt + gray,
      columns: 4,
      table.header([File], [Blank], [Comment], [Code]),
      ..data.slice(calc.floor(data.len() / 2)),
    ),
  )
}
==== Test Coverage
#figure(
  image("../../resources/01 Product Documentation/test-coverage-chart.png"),
  caption: [Test Coverage Chart],
  supplement: [Image],
)

=== User Testing
In game development, the most important aspect of the product is that it is entertaining. As developers, we are not representative users for playtesting, since we have spent many hours developing the features and already know how everything works. A first-time player may not immediately understand game rules that are obvious to us. This strongly relates to our usability NFRs, which require the game to be intuitive which is a prerequisite for it to be enjoyable. This is the main purpose of the user tests of GlowCore.

During testing, the participant is given the game on a device with a keyboard and mouse, where no further instructions are given beyond what is provided in the actual game. They are then asked to play the game while commenting on what they are currently doing. At the end, the participant answers a set of questions to evaluate the corresponding NFRs. The results of the NFRs are documented in their chapter.

#pagebreak()
==== User Test 1 (UT01)
- *Participant*: omega-800
- *Gaming Experience / Similar Games*: Has played Factorio
- *Date*: 28.04.2026
- *Tested release*: v0.2.0

#figure(
  table(
    columns: (1fr, 1fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    [*Field*], [*Result*],
    [*NFR201* Player Restriction Visibility. Did you understand the visual clues?],
    [Yes, I especially liked the red and green clues when building.],

    [*NFR202* Light Upgrade Progress Indication. Did you understand what causes the map to expand?],
    [Yes, when I feed wood into the fire it grows, and the darkness goes away.],

    [*NFR203* New Player Learnability. How long did it take for the first upgrade?], [2 minutes.],
    [What was confusing?], [I couldn't move the camera vertically.],
    [Did you have fun? What did you like?],
    [Yes, I enjoyed playing it. I liked the visual art style with the outlines.],

    [Was the pacing good, too slow or too fast?],
    [The pacing was good. I had fun and felt motivated to progress. I got bored at level 11 because there were no better tools to craft, nothing to aim for.],
  ),
  caption: [UT01],
  supplement: [Table],
)

===== Additional Feedback
#figure(
  table(
    columns: (1fr, 2fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    [*Topic*], [*Explanation*],
    [GlowCore UI],
    [He didn't notice that he had to click "Upgrade To Level 2" to progress. At level 14 it was not obvious that he had to scroll down to add coal.],

    [Signs], [At first he did not read second sign, assuming that all signs display the same text.],
    [Node hovering],
    [It should be possible to select a Node located behind the player (the raycast should go through the player).],

    [Hotbar], [He suggested allowing the player to scroll through the hotbar using the mouse wheel.],
    [Building],
    [He stopped the main task of progressing to build something. This is considered positive, because the game is intended to be open-world to a certain degree.],
  ),
  caption: [UT01 - Additional Feedback],
  supplement: [Table],
)

===== Conclusion
The participant was mainly motivated to progress because he wanted to craft better tools in order to break things faster to progress faster creating a continuous progression loop. This loop is called the core game loop of a game and this is exactly what we wanted to accomplish and what makes the game entertaining ultimately. This user test confirms that the core game loop is engaging.

Based on the feedback we will make improvements to the UI, in particular we will change the GlowCore upgrade UI to be more intuitive.


#pagebreak()
==== User Test 2 (UT02)
- *Participant*: Anonymous
- *Gaming Experience / Similar Games*: Has played various game genres including automation games.
- *Date*: 30.04.2026
- *Tested release*: v0.2.0

#figure(
  table(
    columns: (1fr, 1fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    [*Field*], [*Result*],
    [*NFR201* Player Restriction Visibility. Did you understand the visual clues?], [Yes, the outlines were obvious.],

    [*NFR202* Light Upgrade Progress Indication. Did you understand what causes the map to expand?],
    [Yes, when I upgrade the fire.],

    [*NFR203* New Player Learnability. How long did it take for the first upgrade?], [3 minutes.],
    [What was confusing?], [The border visual is strange.],
    [Did you have fun? What did you like?],
    [It got boring fast, because it's very repetitive. I liked how the crafting table looked. The proportions of the stone-fire and the player were a bit off.],

    [Was the pacing good, too slow or too fast?],
    [The pacing was okay, but I think you should get more wood out of trees.],
  ),
  caption: [UT02],
  supplement: [Table],
)

===== Additional Feedback
#figure(
  table(
    columns: (1fr, 2fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    [*Topic*], [*Explanation*],
    [Gameplay], [The game has potential but there are not enough things to do, not enough content.],

    [Node break prompt], [The break prompt should only be displayed for the first few Nodes that the player breaks.],
    [Camera], [He suggested that you should be able to zoom in and out a bit with the mouse wheel.],
    [Building bug], [When you hold a building block and click somewhere in the UI it still builds.],
  ),
  caption: [UT02 - Additional Feedback],
  supplement: [Table],
)

===== Conclusion
This user test provided valuable feedback on what the gameplay is missing and what it is already doing right. In Sprint 6 we will refine the UI and controls accordingly. We will also add more Items, Nodes and Crafting Recipes. Thanks to our extensible architecture which follows the Open-Closed Principle (OCP), new content can be added without modifying existing code.

#pagebreak()

=== Coding Guidelines

Our coding guidelines are based on the Unity C\# scripting conventions and are enforced through Roslyn analyzers and EditorConfig rules. Every C\# file must conform to these rules, which are checked automatically on every push to `main` or `dev` via the CI pipeline.

==== Naming Conventions

#table(
  columns: (auto, auto, auto),
  fill: (x, y) => if y == 0 { luma(230) },
  stroke: 0.5pt + gray,
  table.header([*Element*], [*Convention*], [*Example*]),
  [Classes / Interfaces], [PascalCase; interfaces prefixed with `I`], [`PlayerController`, `ICollectible`],
  [Methods], [PascalCase], [`MovePlayer()`, `TakeDamage()`],
  [Bool methods], [Question-word prefix], [`IsAlive()`, `CanCollect()`],
  [Parameters / Local variables], [camelCase], [`playerSpeed`, `itemCount`],
  [Constants (`const`)], [`k` prefix + PascalCase], [`kMaxHealth`, `kGravity`],
  [Private fields], [`m_` prefix + camelCase], [`m_playerHealth`, `m_enemyCount`],
  [Public fields], [PascalCase (avoid in classes)], [`PlayerHealth`],
  [Properties], [PascalCase], [`Health`, `Score`],
  [Static fields], [`s_` prefix + camelCase], [`s_instanceCount`],
  [Events], [`On` prefix + PascalCase], [`OnPlayerDeath`, `OnItemCollected`],
  [Enums / Values], [PascalCase], [`PlayerState.Idle`],
  [Namespaces], [PascalCase, project-based], [`GlowCore.Player`],
  [Generic type parameters], [`T` prefix + PascalCase], [`TItem`, `TState`],
)

==== Class Member Ordering

Members within a class must appear in the following order:

+ Constants
+ Static fields
+ Instance fields
+ Constructors
+ Properties
+ Public methods
+ Private methods

==== General Guidelines

*Properties:* For get-only properties, prefer expression-bodied members (e.g. `public int Health => m_health;`). Simple get/set properties may also use expression bodies when the logic fits on one line.

*Method parameters:* Avoid `in` parameters. The compiler may silently create defensive copies, which is a hidden performance cost.

*Access modifiers:* Always declare access modifiers explicitly; never rely on C\# defaults.

*Public fields:* Avoid `public` fields in classes, use properties instead. Public fields in structs are acceptable.

#pagebreak()

==== Formatting Rules

These rules are enforced by `.editorconfig` and verified by `dotnet format` in CI. A build with formatting violations is treated as a failure.

*Line endings:* LF (`\n`) only. CRLF is not permitted in any file.

*Indentation:* 4 spaces for C\# files; 2 spaces for XML/JSON/config files. Tabs are never used.

*Brace style (Allman):* Opening braces always appear on their own line for all constructs (`if`, `for`, method bodies, class bodies, etc.).

*Single-line bodies:* `if`, `for`, `while`, and similar statements with a single-line body omit braces; the body is placed on the next indented line.

*`this.` qualifier:* Do not qualify field or member access with `this.` unless it is required to resolve an ambiguity.

*Language keywords over BCL types:* Use `int`, `string`, `bool`, etc. instead of `Int32`, `String`, `Boolean`.

*`var` keyword:* Use `var` for built-in types and whenever the type is apparent from the right-hand side of an assignment.

*`using` directives:* Placed outside the namespace declaration. `System.*` namespaces are sorted first.

*Modifier order:* `public` / `private` / `protected` / `internal` > `static` > `extern` / `new` / `virtual` / `abstract` / `sealed` / `override` > `readonly` > `unsafe` / `volatile` / `async`.

*Null checks:* Prefer null-coalescing (`??`) and null-conditional (`?.`) operators over explicit null comparisons where possible.

=== Version Control Workflow

The project uses Git with a two-tier branching model:

- *`main`*: stable production branch. Only receives merges from `dev` after a release is considered stable.
- *`dev`*: integration branch. All feature work is merged here first and must pass CI before it is promoted to `main`.

===== Branch Naming

Every branch must be linked to a Jira issue. Branch names follow the pattern:

#align(center)[`<type>/gc-<issue-id>-short-description`]

Where `<type>` is one of:

#table(
  columns: (auto, 1fr),
  fill: (x, y) => if y == 0 { luma(230) },
  stroke: 0.5pt + gray,
  table.header([*Type*], [*When to use*]),
  [`feature`], [New features or significant enhancements],
  [`bug`], [Bug fixes],
  [`docs`], [Documentation updates],
  [`task`], [General tasks that do not fit the above categories],
)

===== Rebasing Workflow

We follow a *rebasing workflow*: before opening a pull request, the feature branch is rebased onto the current tip of `dev`. This keeps the project history linear and avoids unnecessary merge commits.

If a branch accumulates many small or intermediate commits that do not add meaningful history (e.g. "fix typo", "WIP"), the branch is squashed before or during merge to keep the log clean.

===== Pull Requests

Pull requests always target `dev`. Each PR must:

- Reference the corresponding Jira issue in its description.
- Receive *at least one approving review* from a team member before it can be merged.
- Pass all CI checks (linting, formatting, automated tests, and build).

No direct pushes to `dev` or `main` are permitted.
