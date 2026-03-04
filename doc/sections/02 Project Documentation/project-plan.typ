== Project Plan

=== Resources

==== People
The project team consists of 4 students. Each member contributes approximately 8.6 hours per week and is responsible for tasks of their defined roles.

===== Project Management Roles
#table(
  columns: (1fr, 1.5fr, 1fr),
  stroke: 0.5pt + gray,
  fill: (x, y) => if y == 0 { luma(230) },

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
)

===== Developer Roles
#table(
  columns: (1fr, 1.5fr, 1fr),
  stroke: 0.5pt + gray,
  fill: (x, y) => if y == 0 { luma(230) },

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

  [Graphics Programmer (Particle Systems)],
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
  [Nathanael Fässler \\ Cedric Cathomas],
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

#table(
  columns: (1fr, 2fr, 1fr),
  stroke: 0.5pt + gray,
  fill: (x, y) => if y == 0 { luma(230) },

  [*Type*], [*Todos*], [*Frequency*],

  [Sprint Review (Advisor)],
  [
    - Present sprint results  
    - Demonstrate features  
    - Discuss open issues  
    - Collect advisor feedback  
    - Document decisions
  ],
  [Every 2 weeks \ Tuesday 15:00 - 17:00],

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
)



#v(1em)
==== Planning Tools
#table(
  columns: (1fr, 1.5fr),
  stroke: 0.5pt + gray,
  fill: (x, y) => if y == 0 { luma(230) },

  [*Tool*], [*Use Case*],

  [GitLab],
  [- Version control  
   - CI/CD pipelines    
   - Merge requests
   - Time tracking],

  [Typst],
  [- Project documentation  
   - PDF generation],

  [Microsoft Teams],
  [- File sharing  ],

  [Jira],
  [- Task management  
   - Sprint planning  
   - Backlog tracking
   - Issue tracking],

  [Miro],
  [- Brainstorming  
   - Diagrams  
   - Project planning],

  [Discord],
  [- Online meetings
   - Informal communication  
   - Quick coordination],
)
#v(1em)


=== Schedule
The project follows the OST internal SCRUM+ framework for its overall organization. SCRUM+ combines elements of Scrum with the Rational Unified Process (RUP). Long-term planning is based on the RUP approach, while short-term development is organized using Scrum with two-week iterations.


==== Long Term Planning


#v(1em)

==== Short Term Plan


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
