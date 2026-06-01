== Project Plan

=== Resources

==== People
The project team consists of 4 students. Each member contributes approximately 8.6 hours per week and is responsible for tasks of their defined roles.

===== Project Management Roles
#figure(
  table(
    columns: (1fr, 1.5fr, 1fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    align: left,

    [*Role*], [*Description*], [*Names*],

    [Product Owner],
    [
      - Define product vision
      - Manage backlog
      - Prioritize features
      - Validate requirements
    ],
    [Nathanael Fässler],

    [Scrum Master],
    [
      - Facilitate meetings
      - Remove blockers
      - Ensure Scrum process
      - Support team workflow
    ],
    [Dominik Wyss],

    [Project Manager],
    [
      - Schedule planning
      - Resource management
      - Risk control
      - Progress reporting
    ],
    [Cedric Cathomas],

    [DevOps Engineer],
    [
      - CI/CD maintenance
      - Build automation
      - Deployment support
      - Infrastructure management
    ],
    [Yoris Kucera],
  ),
  caption: [Project Management Roles],
  supplement: [Table],
)

===== Developer Roles

#figure(
  table(
    columns: (1fr, 1.5fr, 1fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    align: left,

    [*Role*], [*Description*], [*Names*],

    [Lead Developer],
    [
      - Overall technical coordination
      - Code quality control
      - Architecture decisions
      - Integration management
    ],
    [Yoris Kucera],

    [Developer],
    [
      - Gameplay implementation
      - Bug fixing
      - Feature development
      - System integration
      - Writing Test Cases
    ],
    [Dominik Wyss \ Yoris Kucera \ Nathanael Fässler \ Cedric Cathomas],

    [Art Director / 3D Artist],
    [
      - Asset creation
      - Scene design
      - Visual consistency
      - Asset optimization
    ],
    [Nathanael Fässler],

    [UI Architect],
    [
      - UI/UX design
      - Menu systems
      - HUD implementation
      - User interaction flow
    ],
    [Cedric Cathomas],

    [Graphics Programmer],
    [
      - Shader development
      - Particle effects
      - Performance optimization
      - Visual effects integration
    ],
    [Yoris Kucera],

    [Audio Engineer],
    [
      - Sound design
      - Audio integration
      - Music management
      - Audio balancing
    ],
    [Dominik Wyss],

    [Level Designer],
    [
      - Level layout
      - Gameplay balancing
      - Difficulty tuning
      - Playtesting feedback
    ],
    [Nathanael Fässler \ Cedric Cathomas],
  ),
  caption: [Developer Roles],
  supplement: [Table],
)



#v(1em)


==== Time
The project runs from 16.02.2026 to 12.06.2026 with an estimated effort of 120 hours per person, resulting in a total workload of 480 hours, structured in milestones and sprints.

#v(1em)
==== Cost
Most required tools and platform are freely available through free or student licenses. We plan on publishing GlowCore on steam for which steam charges a 100 US-Dollar publishing fee. The cost will be shared by the members.
#v(1em)

#pagebreak()

=== Organization
==== Meetings
The project meetings follow mainly the Scrum standard. With weekly Scrum meetings replacing daily stand-ups and biweekly sprint planning including retrospectives. Also the Regular advisor meetings to present and discuss our results so far.

#figure(
  table(
    columns: (1fr, 2fr, 1fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    align: left,

    [*Type*], [*Todos*], [*Frequency*],

    [Sprint Review (Advisor)],
    [
      - Present sprint results
      - Demonstrate features
      - Discuss open issues
      - Collect advisor feedback
      - Document decisions
    ],
    [Every 2 weeks \ Tuesday 15:00 - 16:00],

    [Weekly Scrum],
    [
      - Report progress
      - Define next steps
      - Identify blockers
      - Update tasks
      - Support team members
    ],
    [Weekly \ Friday 14:00 - 15:00],

    [Sprint Retrospective],
    [
      - Analyze sprint process
      - Identify problems
      - Define improvements
      - Document lessons learned
    ],
    [Every 2 weeks \ Friday 15:00 - 16:00],

    [Sprint Planning],
    [
      - Define sprint goals
      - Select backlog items
      - Estimate effort
      - Assign tasks
      - Update schedule
    ],
    [Every 2 weeks \ Friday 16:00 - 17:00 \ (after retro)],
  ),
  caption: [Planned Meetings Overview],
  supplement: [Table],
)



#v(1em)
==== Planning Tools

#figure(
  table(
    columns: (1fr, 1.5fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    align: left,

    [*Tool*], [*Use Case*],

    [GitHub],
    [
      - Version control
      - GitHub Actions (CI/CD)
      - Pull requests
      - GitHub notifications via Discord
    ],

    [Typst],
    [
      - Project documentation
      - PDF generation
    ],

    [Microsoft Teams],
    [
      - File sharing
      - Communication
    ],

    [Jira],
    [
      - Task management
      - Sprint planning
      - Backlog tracking
      - Issue tracking
      - Time tracking
    ],

    [Miro],
    [
      - Brainstorming
      - Diagrams
      - Project planning
    ],

    [Discord],
    [
      - Online meetings
      - Informal communication
      - Quick coordination
      - GitHub activity notifications
    ],
  ),
  caption: [Planning Tools],
  supplement: [Table],
)
#v(1em)


=== Schedule
The project follows the OST internal SCRUM+ framework for its overall organization. SCRUM+ combines elements of Scrum with the Rational Unified Process (RUP). Long-term planning is based on the RUP approach, while short-term development is organized using Scrum with two-week iterations.

At the start of each iteration, we estimate all tickets with storypoints. The story *GC-54 Make Player* is the reference story which is worth 5 storypoints.

==== Work Items
We work with 5 types of Work Items
#figure(
  table(
    columns: (1fr, 1fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    [*Name*], [*Description*],
    [Epic], [A group of features that we want to implement.],
    [Story], [Describes a feature that is directly related to the game],
    [Task], [Everything that isn't directly related to the game],
    [Bug], [Bugs are created when a bug is found and fixed in an unplanned matter.],
    [Documentation], [Work related to the documentation],
  ),
  caption: [Work Items],
  supplement: [Table],
)

#pagebreak()

==== Long Term Plan
The Long Term Planning is separated into two categories:
- Milestones: Serve as checkpoint for releases and review meetings.
- Epics: Provide the structure to organize the stories.
===== Milestones
The milestones for the review meetings and for the releases are interleaved and independent.

In agile project management it is common to reevaluate the Long Term Plan during the project. For this reason there are two Milestone Tables. The first table shows the Initial Milestones that were defined at the beginning of the project and the second table contains updated Milestones as of 07.04.2026.
====== Initial Milestones
#figure(
  table(
    columns: (0.3fr, 1fr, 1fr, 2fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },

    [*Nr*], [*Milestone Name*], [*Date*], [*Description*],

    [M01],
    [Review 1],
    [10.03.2026],
    [
      Initial Project Setup
    ],

    [M02],
    [Review 2],
    [24.03.2026],
    [
      Requirements
    ],

    [M03],
    [Review 3],
    [14.04.2026],
    [
      End of Elaboration
    ],

    [M04],
    [Review 4],
    [28.04.2026 ],
    [
      Quality
    ],

    [M05],
    [Review 5],
    [12.05.2026],
    [
      Architecture
    ],

    [M06],
    [Project Presentation],
    [12.06.2026],
    [
      Final Presentation to SEProject-Advisors and fellow students. Completion of SEProject.
    ],

    [M07],
    [Pre-Alpha Release],
    [20.03.2026],
    [
      First playable prototype is ready. Core gameplay loops are implemented. The player can chop trees, pick up wood and expand the GlowCore.
    ],

    [M08],
    [Alpha Release],
    [10.04.2026],
    [
      MVP is done and working. Core gameplay Mechanics are implemented. Player can craft, build, expand the map and he has an inventory. The player can save his savestate.
    ],

    [M09],
    [Beta Release],
    [15.05.2026],
    [
      More Biomes and Materials are added. There is a combat system with enemies and peaceful mobs. There is a title screen and a settings menu.

      Automation mechanics are implemented. There are more GlowCore Phases.
    ],

    [M10],
    [Official Release and Final Submission],
    [05.06.2026],
    [
      The game is polished and has no mayor bugs. The game has a beginning and an ending.
    ],
  ),
  caption: [Initial Milestones],
  supplement: [Table],
) <InitialMilestones>

#pagebreak()

====== Updated Milestones as of 07.04.2026
Milestone M08 - Beta Release was updated and the combat system and the automation mechanics were removed. The reduction in scope allows the team to deliver a stable and more polished game at the end. The savestate system was moved from Alpha to Beta. We prioritize quality over quantity. If there is enough time at the end of the project, we can still implement the features as a bonus.
#figure(
  table(
    columns: (0.3fr, 1fr, 1fr, 2fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },

    [*Nr*], [*Milestone Name*], [*Date*], [*Description*],

    [M01],
    [Review 1],
    [10.03.2026],
    [
      Initial Project Setup
    ],

    [M02],
    [Review 2],
    [24.03.2026],
    [
      Requirements
    ],

    [M03],
    [Review 3],
    [14.04.2026],
    [
      End of Elaboration
    ],

    [M04],
    [Review 4],
    [28.04.2026 ],
    [
      Quality
    ],

    [M05],
    [Review 5],
    [12.05.2026],
    [
      Architecture
    ],

    [M06],
    [Project Presentation],
    [12.06.2026],
    [
      Final Presentation to SEProject-Advisors and fellow students. Completion of SEProject.
    ],

    [M07],
    [Pre-Alpha Release],
    [20.03.2026],
    [
      First playable prototype is ready. Core gameplay loops are implemented. The player can chop trees, pick up wood and expand the GlowCore.
    ],

    [M08],
    [Alpha Release],
    [10.04.2026],
    [
      MVP is done and working. Core gameplay Mechanics are implemented. Player can craft, build, expand the map and he has an inventory.
    ],

    [M09],
    [Beta Release],
    [15.05.2026],
    [
      More Biomes and Materials are added. There is a title screen and a settings menu. There are more GlowCore Phases. The player can save his savestate.
    ],

    [M10],
    [Official Release and Final Submission],
    [05.06.2026],
    [
      The game is polished and has no mayor bugs. The game has a beginning and an ending.
    ],
  ),
  caption: [Updated Milestones],
  supplement: [Table],
) <UpdatedMilestones>


#v(1em)
===== Epics

#figure(
  table(
    columns: (0.4fr, 1fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    align: left,

    [*Epic Name*], [*Description/Requirements*],
    [Documentation],
    [
      This epic includes all documentation related work.
    ],

    [Project Setup],
    [
      This epic contains all work that is done in the Inception-Phase.
    ],

    [Core Gameplay Loop Mechanics],
    [
      - The player can chop down trees, collect wood and feed it into the GlowcCore.
      - The GlowCore levels up and expands the map.
      - The player is restricted to the unlocked map. The grid system is implemented.
    ],

    [Inventory, Crafting, Building],
    [
      - Inventory: The player has an inventory and can store a limited amount of items. He can have items in his hands and in his backpack(or similar).
      - Crafting: The player can craft items to other items on different workstations.
      - Building: The player can place objects onto the world-grid.
    ],

    [Graphics],
    [
      - The Artstyle of the game ist set.
      - The 3D Models follow a consistent Artstyle.
      - A Shader is implementet.
    ],

    [Game Framework & Completeness],
    [
      This epic contains everything that makes a game complete and feel like a game.
      - Intro: When the Game starts the GlowCore Logo is displayed. After the title screen a Cutscene is played that leads into the game.
      - Ending: A ending of the game is defined. When the player finishes the game, a Cutscene is played and the credits roll.
      - UI: A Title Screen is available. In-Game there is a Debug Screen, and a Settings Menu.
      - Gamedata Saving: The player can save his progress.
    ],

    [Big World & GlowCore Phases],
    [
      - World building: The game has a complete world to play in.
      - GlowCore Phases: The GlowCore has multiple phases, all of which need different materials to level up. The GlowCore changes visually with every Level.
    ],

    [Biomes & Materials],
    [
      - The world has multitple biomes: e.g. Forest, Grassland, Mountain, Volcano, Jungle etc.
      - Each biome has unique ressources.
      - There are Caves which you can enter.
    ],

    [Combat],
    [
      - Enemies can spawn and fight with the player.
      - The player can take damage and can die.
      - A respawn mechanic is set up.
      - There are peacuful mobs.
    ],

    [Resource Automation],
    [
      - The Player can replant trees.
      - The Player can craft machines that automatically gather resources.
    ],

    [Playtesting & Release],
    [
      - Playtesting: The game is testet by players that play it for the first time (blind test).
      - Release: The game is released on steam on its own steam page and has a trailer, screenshots and a description.
    ],
  ),
  caption: [Epics],
  supplement: [Table],
)

The following image shows the Timeline in Jira as of 04.03.2026.
#figure(
  image("../../resources/02 Project Documentation/jira-epics-timeline.png"),
  caption: [Initial Jira Timeline],
  supplement: [Image],
)

#v(1em)
The Long Term Plan was updated on 07.04.2026. The following image shows the new Timeline in Jira as of 07.04.2026.
#figure(
  image("../../resources/02 Project Documentation/jira-updated-epics-timeline.png"),
  caption: [Updated Jira Timeline],
  supplement: [Image],
)

#v(1em)
#pagebreak()

===== Initial Backlog & MVP
The priority of Stories is defined by the ordering in the backlog. As we progress in Agile, we may divide Stories further and add new Stories. The MVP is reached when every Story up to and including *GC-101 Build World* is completed.

The following is the Initial Backlog as of 04.03.2026.

#image("../../resources/02 Project Documentation/jira-initial-backlog-1.png")
#figure(
  image("../../resources/02 Project Documentation/jira-initial-backlog-2.png"),
  caption: [Inital Backlog],
  supplement: [Image],
)

#v(1em)

==== Short Term Plan
In a Sprint the priority of the tasks and stories is defined by the ordering.

===== Sprint 1 (Sprint Zero) 20.02.2026 - 06.03.2026
The following shows the Jira sprint backlog of the first sprint. Since this was Sprint Zero, not all guidelines were followed initially because they had not yet been established.

#figure(
  image("../../resources/02 Project Documentation/Sprints/sprint-1-backlog.png"),
  caption: [Sprint 1 Backlog],
  supplement: [Image],
)

#pagebreak()

===== Sprint 2 06.03.2026 - 20.03.2026
In this sprint the first actual work on the game was carried out in Unity. The focus was to implement an initial working prototype.

#figure(
  image("../../resources/02 Project Documentation/Sprints/sprint-2-burndown.png"),
  caption: [Sprint 2 Burndown Chart],
  supplement: [Image],
)

#figure(
  image("../../resources/02 Project Documentation/Sprints/sprint-2-backlog.png"),
  caption: [Sprint 2 Backlog],
  supplement: [Image],
)

#pagebreak()

===== Sprint 3 20.03.2026 - 03.04.2026
In Sprint 3 we focused on implementing the MVP. The planned amount of storypoints was too ambitious so we had to move some Tasks into Sprint 4. From this experience we gained a better understanding of out team's capacity.

#figure(
  image("../../resources/02 Project Documentation/Sprints/sprint-3-burndown.png"),
  caption: [Sprint 3 Burndown Chart],
  supplement: [Image],
)

#figure(
  image("../../resources/02 Project Documentation/Sprints/sprint-3-backlog.png"),
  caption: [Sprint 3 Backlog],
  supplement: [Image],
)

#pagebreak();

===== Sprint 4 03.04.2026 - 17.04.2026
Because in the week of 06.04.2026 to 12.04.2026 was the spring break we had a sprint with reduced work.

#figure(
  image("../../resources/02 Project Documentation/Sprints/sprint-4-burndown.png"),
  caption: [Sprint 4 Burndown Chart],
  supplement: [Image],
)

#figure(
  image("../../resources/02 Project Documentation/Sprints/sprint-4-backlog.png"),
  caption: [Sprint 4 Backlog],
  supplement: [Image],
)

#pagebreak();

===== Sprint 5 17.04.2026 - 01.05.2026
#figure(
  image("../../resources/02 Project Documentation/Sprints/sprint-5-burndown.png"),
  caption: [Sprint 5 Burndown Chart],
  supplement: [Image],
)

#figure(
  image("../../resources/02 Project Documentation/Sprints/sprint-5-backlog.png"),
  caption: [Sprint 5 Backlog],
  supplement: [Image],
)


#pagebreak();

===== Sprint 6 01.05.2026 - 15.05.2026
#figure(
  image("../../resources/02 Project Documentation/Sprints/sprint-6-burndown.png"),
  caption: [Sprint 6 Burndown Chart],
  supplement: [Image],
)

#figure(
  image("../../resources/02 Project Documentation/Sprints/sprint-6-backlog.png"),
  caption: [Sprint 6 Backlog],
  supplement: [Image],
)


#pagebreak();

===== Sprint 7 15.05.2026 - 29.05.2026
#figure(
  image("../../resources/02 Project Documentation/Sprints/sprint-7-burndown.png"),
  caption: [Sprint 7 Burndown Chart],
  supplement: [Image],
)

#image("../../resources/02 Project Documentation/Sprints/sprint-7-backlog1.png")
#figure(
  image("../../resources/02 Project Documentation/Sprints/sprint-7-backlog2.png"),
  caption: [Sprint 7 Backlog],
  supplement: [Image],
)








#pagebreak();
=== Risk Management

This chapter describes potential risks to the project and how we plan to mitigate them. We identified 8 risks, which are listed in the table below. Additionally, these risks are visualized in two risk matrices, one showing the initial risk assessment and the other showing the risk assessment after mitigation strategies are applied.

#table(
  columns: (0.2fr, 0.7fr, 1fr, 1fr),
  stroke: 0.5pt + gray,
  fill: (x, y) => if y == 0 { luma(230) },

  [*No.*], [*Risk*], [*Description*], [*Mitigation Strategy*],
  [R01],
  [Lack of familiarity with Unity],
  [
    None of our team members have any significant experience with the Unity Engine. As such, this could heavily impact our development times.
  ],
  [
    We made ourselves familiar with the engine before the actual start
    of the project, to get an idea of the workflows.
  ],

  [R02],
  [Unity CI/CD Complexity],
  [
    We are depending on Unity's build system being robust enough to work in
    the context of a CI/CD pipeline. We also potentially rely on pre-existing GitHub Actions for Unity, which may not be well-maintained or compatible with our project.
  ],
  [
    We will set up the CI/CD pipeline very early into the project and did
    earlier tests. We tried out pre-existing GitHub Actions to make sure they satisfy our needs.
  ],

  [R03],
  [Scope/Feature Creep],
  [
    As we are developing a game, we run the risk of encountering scope creep, i.e. continuously planning new features, which would result in us eventually not being able to keep up with the workload.
  ],
  [
    We created a detailed project plan with long-term and short-term planning, along with well defined milestones and a clear MVP.
  ],

  [R04],
  [Bad Code Quality],
  [
    In large codebases it is easy to end up with badly designed systems that don't fit together with the rest of the code. This would hinder further development.
  ],
  [
    We do mandatory code reviews before changes can be merged. Additionally, we will set up CI/CD pipelines to automatically run code quality and formatting checks on pull requests.
  ],

  [R05],
  [Team Member is MIA],
  [
    A team member might be MIA for an extended period of time due to various reasons, such as an illness or damaged hardware. This would recude our capacity and slow down development.
  ],
  [
    We can't reduce the possibility of this occurring, but we can distribute work across the remaining team members. We also avoid knowledge monopolies and always aim to share discoveries with the other team members.
  ],

  [R06],
  [Personal Conflicts],
  [
    There is always the potential for personal conflicts within the team, which would complicate development.
  ],
  [
    We openly communicate any interpersonal issues and try to resolve conflicts as early as possible.
  ],

  [R07],
  [Data Loss],
  [
    There is the potential of local storage media failing, resulting in significant data loss.
  ],
  [
    We mititage this by prequently committing our changes to GitHub, and keeping untracked files in cloud based locations.
  ],

  [R08],
  [Workload Underestimation],
  [
    We might be underestimating the amount of work required for specific features and tasks.
  ],
  [
    We plan with a buffer and try to be conservative with our estimates. We also try to break down large tasks into smaller ones, which are easier to estimate.
  ],
)

#figure(
  kind: table,
  caption: [Risk Analysis],
  supplement: [Table],
  none,
)

#let table-header = rgb("#e6e6e6")
#let cell-bg-default = rgb("#ededed")
#let cell-bg-high = rgb("#ffa787")
#let cell-bg-medium = rgb("#fff987")
#let cell-bg-low = rgb("#8fff87")
#let cell-bg-very-high = rgb("#f2665c")
#let cell-bg-eliminated = rgb("#b3f6ff")

#pagebreak()

==== Risk Matrix <risk-matrix>

The below table shows the risk assessment of the identified risks, before any mitigation strategies are applied. The risks are categorized based on their probability of occurrence and their potential severity.

#let risk-cell(level, content: none) = {
  let bg = if level == "Very High" {
    cell-bg-very-high
  } else if level == "High" {
    cell-bg-high
  } else if level == "Medium" {
    cell-bg-medium
  } else if level == "Low" {
    cell-bg-low
  } else {
    cell-bg-default
  }

  // let body = if content != none { content } else { level }
  table.cell(fill: bg)[#content]
}

#figure(
  table(
    columns: (1fr, 1fr, 1fr, 1fr, 1fr),
    stroke: 1pt + rgb("#333333"),
    align: center + horizon,

    table.cell(fill: table-header, rowspan: 2)[*Probability*],
    table.cell(fill: table-header, colspan: 4)[*Severity*],
    table.cell(fill: table-header)[Negligible],
    table.cell(fill: table-header)[Marginal],
    table.cell(fill: table-header)[Critical],
    table.cell(fill: table-header)[Catastrophic],

    [Certain], risk-cell("High"), risk-cell("High"), risk-cell("Very High"), risk-cell("Very High"),
    [Likely],
    risk-cell("Medium"),
    risk-cell("High"),
    risk-cell("High", content: [R04]),
    risk-cell("Very High", content: [R01]),
    [Possible],
    risk-cell("Low"),
    risk-cell("Medium", content: [R02/R06]),
    risk-cell("High"),
    risk-cell("Very High", content: [R03/R08]),
    [Unlikely], risk-cell("Low"), risk-cell("Medium"), risk-cell("Medium"), risk-cell("High", content: [R05]),
    [Rare], risk-cell("Low"), risk-cell("Low"), risk-cell("Medium"), risk-cell("Medium", content: [R07]),
    [None], table.cell(fill: cell-bg-eliminated, colspan: 4)[],
  ),
  caption: [Risk Matrix],
  supplement: [Table],
)

==== Risk Matrix (Mitigated) <risk-matrix-mitigated>

The below table shows the risk assessment of the identified risks, after mitigation strategies have been applied.

#figure(
  table(
    columns: (1fr, 1fr, 1fr, 1fr, 1fr),
    stroke: 1pt + rgb("#333333"),
    align: center + horizon,

    table.cell(fill: table-header, rowspan: 2)[*Probability*],
    table.cell(fill: table-header, colspan: 4)[*Severity*],
    table.cell(fill: table-header)[Negligible],
    table.cell(fill: table-header)[Marginal],
    table.cell(fill: table-header)[Critical],
    table.cell(fill: table-header)[Catastrophic],

    [Certain], risk-cell("High"), risk-cell("High"), risk-cell("Very High"), risk-cell("Very High"),
    [Likely],
    risk-cell("Medium"),
    risk-cell("High"),
    risk-cell("High"),
    risk-cell("Very High"),
    [Possible],
    risk-cell("Low"),
    risk-cell("Medium"),
    risk-cell("High"),
    risk-cell("Very High"),
    [Unlikely],
    risk-cell("Low"),
    risk-cell("Medium", content: [R06]),
    risk-cell("Medium", content: [R01/R04/R05]),
    risk-cell("High", content: [R08]),
    [Rare],
    risk-cell("Low", content: [R07]),
    risk-cell("Low", content: [R02]),
    risk-cell("Medium"),
    risk-cell("Medium"),
    [None], table.cell(fill: cell-bg-eliminated, colspan: 3)[], table.cell(fill: cell-bg-eliminated)[R03],
  ),
  caption: [Risk Matrix (Mitigated)],
  supplement: [Table],
)

==== Assessed Risk Matrix (14.04.2026) <assessed-risk-matrix-14.04.2026>

The below table shows the risk assessment of the identified risks, after mitigation strategies have been applied.

#figure(
  table(
    columns: (1fr, 1fr, 1fr, 1fr, 1fr),
    stroke: 1pt + rgb("#333333"),
    align: center + horizon,

    table.cell(fill: table-header, rowspan: 2)[*Probability*],
    table.cell(fill: table-header, colspan: 4)[*Severity*],
    table.cell(fill: table-header)[Negligible],
    table.cell(fill: table-header)[Marginal],
    table.cell(fill: table-header)[Critical],
    table.cell(fill: table-header)[Catastrophic],

    [Certain], risk-cell("High"), risk-cell("High"), risk-cell("Very High"), risk-cell("Very High"),
    [Likely],
    risk-cell("Medium"),
    risk-cell("High"),
    risk-cell("High"),
    risk-cell("Very High"),
    [Possible],
    risk-cell("Low"),
    risk-cell("Medium"),
    risk-cell("High", content: [R01]),
    risk-cell("Very High"),
    [Unlikely],
    risk-cell("Low"),
    risk-cell("Medium", content: [R06/R02]),
    risk-cell("Medium", content: [R04/R05]),
    risk-cell("High", content: [R08]),
    [Rare],
    risk-cell("Low", content: [R07]),
    risk-cell("Low"),
    risk-cell("Medium"),
    risk-cell("Medium"),
    [None], table.cell(fill: cell-bg-eliminated, colspan: 3)[], table.cell(fill: cell-bg-eliminated)[R03],
  ),
  caption: [Assessed Risk Matrix (14.04.2026)],
  supplement: [Table],
)

#pagebreak()

==== Adjustment of Current Risk Assessment

In the current risk matrix, we increased the probability by one level for both R01 (Lack of familiarity with Unity) and R02 (Unity CI/CD Complexity).

For R01, we observed that working with Unity involves more aspects than initially expected, especially issues that only become visible during actual implementation. The project description already pointed out that Unity could be a risk, so we were aware of this from the beginning. However, during development we realized that some parts are more complex in practice. Despite this, we are still very motivated to use Unity for our project and try to reduce the risk through clear communication and well-defined implementation strategies.

For R02, there are recurring challenges with the CI/CD setup in Unity, which is why we increased the probability here as well. In particular, there is a risk of running out of GitHub Actions minutes. At the moment, we expect the available minutes to be sufficient, but if needed, we already have a plan to set up and use a self-hosted runner on our own machine.

Overall, both risks are now considered slightly more likely, but appropriate mitigation measures are in place.
#v(1em)


