== Quality Measures

=== Working Environment

#v(23cm)
==== Definition of Ready and Definition of Done

Every Jira ticket follows a structured lifecycle governed by our Definition of Ready (DoR) and Definition of Done (DoD). A ticket may only be pulled into a sprint once all DoR criteria are met, including a clear description, defined acceptance criteria, story point estimate, epic assignment, and team sign-off during sprint planning. A ticket is only considered done once all DoD criteria are satisfied, covering code quality, CI/CD pipeline status, test coverage, PR approval and conditionally -> UI consistency and documentation updates.

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

==== Non-Functional Requirements (NFR) Verification
NFR tests are not done once at the end, but at appropriate points throughout the project. Each NFR is assigned to a milestone at which it must be verified. Additionally, our Definition of Done requires that any change which could affect an NFR is accompanied by a re-verification of that NFR and an updated result in the documentation. This ensures NFR results stay current and are not just a snapshot from a single test session.

A huge portion of our testing isn't just about finding bugs, but verifying our NFRs. We have defined a wide spectrum of NFRs covering different quality attributes. They are sorted by priority, meaning that while our ultimate goal is to fulfill all of them, the ones marked as "Required" are strict targets we have to hit for the final submission.

We cover NFR testing through a mix of automated benchmarks and manual playtesting protocols:

- *Performance Efficiency:* We regularly check our performance manually to make sure the game runs smoothly. We aim to ensure that the world loads quickly, in-game scene transitions are fast, and the game maintains a solid 60 FPS on recommended hardware.

- *Reliability:* Since saving the game correctly is critical, we manually test save and load cycles to ensure data doesn't corrupt. We also verify that save files remain small enough (under 5MB), that they are fully portable between machines, and that auto-saves trigger correctly.

- *Portability & Maintainability:* We manually verify that the game runs identically across Windows, Linux, and WebGL, and that it supports standard 16:9 resolutions. From a code perspective, we constantly test extensibility during normal development. We frequently add new elements like resource nodes, items, or recipes based on existing systems. If our internal architecture was too complicated to extend, we would notice it very quickly.

- *Usability & Visuals:* Beyond the playtesting mentioned earlier, we also manually review the game's language to ensure all text is in English. We test controller and keyboard inputs by simply playing with both, and we hold art reviews to make sure all new assets fit our low-poly pastel style guide.

By combining the automated safety net of our UTF pipeline with heavy manual playtesting and strict NFR benchmarking, we believe this concept will keep the game stable while still allowing us to iterate quickly on the gameplay.

#show ref: it => {
  if it.element != none and it.element.func() == figure {
    link(it.target, str(it.target))
  } else {
    it
  }
}

==== Coverage Summary

At the current stage of the project (approaching M09 Beta), 4 out of 11 Use Cases are fully verified (@UC01, @UC03, @UC04, @UC07), 3 are partially implemented (@UC02, @UC05, @UC06), and 4 have not been tested yet as they are planned for M10 (@UC08, @UC09, @UC10, @UC11).

For the NFRs, 7 requirements have fully passed (@NFR103, @NFR105, @NFR201, @NFR202, @NFR206, @NFR401, @NFR501), 7 are partially met (@NFR101, @NFR104, @NFR204, @NFR301, @NFR302, @NFR303, @NFR402), and 4 have not been tested yet as they are scheduled for M10 (@NFR203, @NFR205, @NFR502, @NFR503). @NFR102 is not applicable since in-game scene transitions were removed from scope. The partially met NFRs are mostly related to the save system and input support, which still have known issues to be resolved before the final release.
// TODO: Describe the test strategy as discussed in SEP1.
// - How each functional and non-functional requirement is verified
// - At what level (Unit, Integration, System)


=== Code metrics
==== Lines of Code
#figure(
  image("../../resources/01 Product Documentation/loc-diagramm.png", width: 100%),
  caption: [Interface Overview Diagram],
  supplement: [Image],
)
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
#image("../../resources/01 Product Documentation/test-coverage-chart.png")

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
The participant was mainly motivated to progress because he wanted to craft better tools in order to break things faster to progress faster creating a continous progression loop. This loop is called the core game loop of a game and this is exactly what we wanted to accomplish and what makes the game entertaining ultimately. This user test confirms that the core game loop is engaging.

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
