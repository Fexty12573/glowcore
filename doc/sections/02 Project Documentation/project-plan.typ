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
  supplement: [Table]
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
  supplement: [Table]
)



#v(1em)


==== Time
The project runs from 16.02.2026 to 12.06.2026 with an estimated effort of 120 hours per person, resulting in a total workload of 480 hours, structured in milestones and sprints.

#v(1em)
==== Cost
No direct financial costs are currently planned, as all required tools and platforms are available through free or student licenses. Potential future costs are not expected.

#v(1em)


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
  supplement: [Table]
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
    [- Version control  
    - GitHub Actions (CI/CD)    
    - Pull requests
    - GitHub notifications via Discord],

    [Typst],
    [- Project documentation  
    - PDF generation],

    [Microsoft Teams],
    [- File sharing
    - Communication  ],
    

    [Jira],
    [- Task management  
    - Sprint planning  
    - Backlog tracking
    - Issue tracking
    - Time tracking],

    [Miro],
    [- Brainstorming  
    - Diagrams  
    - Project planning],

    [Discord],
    [- Online meetings
    - Informal communication  
    - Quick coordination
    - GitHub activity notifications],
  ),
  caption: [Planning Tools],
  supplement: [Table]
)
#v(1em)


=== Schedule
The project follows the OST internal SCRUM+ framework for its overall organization. SCRUM+ combines elements of Scrum with the Rational Unified Process (RUP). Long-term planning is based on the RUP approach, while short-term development is organized using Scrum with two-week iterations.

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
    [Documentation], [Work related to the documentation]
  ),
  caption: [Work Items],
  supplement: [Table]
)
==== Long Term Planning
The Long Term Planning is separated into two categories:
- Milestones: Serve as checkpoint for releases and review meetings.
- Epics: Provide the structure to organize the stories.
===== Milestones
The milestones for the review meetings and for the releases are interleaved and independent.

#figure(
  table(
    columns: (0.3fr, 1fr, 1fr, 2fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },


    [*Nr*], [*Milestone Name*], [*Duration*], [*Description*],

    [M01], [Review 1], [16.02.2026], [
      Initial Project Setup
    ],
    [M02], [Review 2], [11.03.2026], [
      Requirements
    ],
    [M03], [Review 3], [25.03.2026], [
      End of Elaboration
    ],
    [M04], [Review 4], [08.04.20266], [
      Quality
    ],
    [M05], [Review 5], [29.04.02.2026], [
      Architecture
    ],
    [M06], [Project Presentation], [12.06.2026], [
      Final Presentation to SEProject-Advisors and fellow students. Completion of SEProject.
    ],
    [M07], [Pre-Alpha Release], [20.03.2026], [
      First playable prototype is ready. Core gameplay loops are implemented. The player can chop trees, pick up wood and expand the GlowCore.
    ],
    [M08], [Alpha Release], [10.04.2026], [
      MVP is done and working. Core gameplay Mechanics are implemented. Player can craft, build, expand the map and he has an inventory. The player can save his savestate.
    ],
    [M09], [Beta Release], [15.05.2026], [
      More Biomes and Materials are added. There is a combat system with enemies and peaceful mobs. There is a title screen and a settings menu.

      Automation mechanics are implemented. There are more GlowCore Phases.
    ],
    [M10], [Official Release and Final Submission], [05.06.2026], [
      The game is polished and has no mayor bugs. The game has a beginning and an ending.
    ],
  ),
  caption: [Milestones],
  supplement: [Table]
)

#v(1em)
===== Epics

#figure(

  table(
    columns: (0.4fr, 1fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    align:left,

    [*Epic Name*], [*Description/Requirements*], 
    [Documentation], [
      This epic includes all documentation related work.
    ],
    [Project Setup], [
      This epic contains all work that is done in the Inception-Phase.
    ],
    [Core Gameplay Loop Mechanics], [
      - The player can chop down trees, collect wood and feed it into the GlowcCore.
      - The GlowCore levels up and expands the map.
      - The player is restricted to the unlocked map. The grid system is implemented.
    ],
    [Inventory, Crafting, Building], [
      - Inventory: The player has an inventory and can store a limited amount of items. He can have items in his hands and in his backpack(or similar).
      - Crafting: The player can craft items to other items on different workstations.
      - Building: The player can place objects onto the world-grid.
    ],
    [Graphics], [
      - The Artstyle of the game ist set.
      - The 3D Models follow a consistent Artstyle.
      - A Shader is implementet.
    ],
    [Game Framework & Completeness], [
      This epic contains everything that makes a game complete and feel like a game.
      - Intro: When the Game starts the GlowCore Logo is displayed. After the title screen a Cutscene is played that leads into the game.
      - Ending: A ending of the game is defined. When the player finishes the game, a Cutscene is played and the credits roll.
      - UI: A Title Screen is available. In-Game there is a Debug Screen, and a Settings Menu.
      - Gamedata Saving: The player can save his progress.
    ],
    [Big World & GlowCore Phases], [
    - World building: The game has a complete world to play in.
    - GlowCore Phases: The GlowCore has multiple phases, all of which need different materials to level up. The GlowCore changes visually with every Level.
    ],
    [Biomes & Materials], [
    - The world has multitple biomes: e.g. Forest, Grassland, Mountain, Volcano, Jungle etc. 
    - Each biome has unique ressources. 
    - There are Caves which you can enter.
    ],
    [Combat], [
      - Enemies can spawn and fight with the player.
      - The player can take damage and can die.
      - A respawn mechanic is set up.
      - There are peacuful mobs.
    ],
    [Resource Automation], [
      - The Player can replant trees.
      - The Player can craft machines that automatically gather resources.
    ],
    [Playtesting & Release], [
      - Playtesting: The game is testet by players that play it for the first time (blind test).
      - Release: The game is released on steam on its own steam page and has a trailer, screenshots and a description.
    ],
  ),
  caption: [Epics],
  supplement: [Table]
)

The following image shows the Timeline in Jira as of 04.03.2026.
#image("../../resources/02 Project Documentation/jira-epics-timeline.png")
#v(1em)

===== Initial Backlog & MVP
The priority of Stories is defined by the ordering in the backlog. As we progress in Agile, we may divide Stories further and add new Stories. The MVP is reached when every Story up to and including *GC-101 Build World* is completed.

The following is the Initial Backlog as of 04.03.2026.
#image("../../resources/02 Project Documentation/jira-initial-backlog-1.png")
#image("../../resources/02 Project Documentation/jira-initial-backlog-2.png")
#image("../../resources/02 Project Documentation/jira-initial-backlog-3.png")
#v(1em)

==== Short Term Plan
In a Sprint the priority of the tasks and stories is defined by the ordering.

===== Sprint 1 (Sprint Zero) 20.02.2026 - 06.03.2026
The following shows the Jira sprint backlog as of 04.03.2026
#image("../../resources/02 Project Documentation/jira-sprint-1.png")
#v(1em)


=== Risk Management


#v(1em)


// TODO: Describe the project plan as covered in SEP2.
// A project plan typically consists of:
// - Processes, meetings, and roles
// - Phases, iterations, and milestones
// - A rough list of things to be done (work items)
// - Risk management
// - Planning tools (issue tracker, time tracker, ...)
//
// Do NOT describe your technical solution here — this chapter is about organizing the project.
