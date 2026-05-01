== Quality Measures

=== Working Environment

// TODO: Describe the quality measures applied in your project (SEP1 & SEP2).
// Things to include:
// - Organizational means (Merge Requests, Definition of Done, etc.)
// - Guidelines for code, documentation, version control, etc.
// - Tools used to assess quality (linter, metrics, ...)
// - Tools used to build and deploy (CI/CD)
// Tip: Avoid duplication with other chapters — use cross-references when appropriate.

=== Testing

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

For the NFRs, 6 requirements have fully passed (@NFR103, @NFR105, @NFR201, @NFR202, @NFR206, @NFR501), 8 are partially met (@NFR101, @NFR104, @NFR204, @NFR301, @NFR302, @NFR303, @NFR401, @NFR402), and 4 have not been tested yet as they are scheduled for M10 (@NFR203, @NFR205, @NFR502, @NFR503). @NFR102 is not applicable since in-game scene transitions were removed from scope. The partially met NFRs are mostly related to the save system and input support, which still have known issues to be resolved before the final release.
// TODO: Describe the test strategy as discussed in SEP1.
// - How each functional and non-functional requirement is verified
// - At what level (Unit, Integration, System)
