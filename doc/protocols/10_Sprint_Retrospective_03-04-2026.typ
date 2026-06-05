#import "../lib.typ": agenda_table, meeting_info, todo_table

// ==============================
// 3) Sprint Retrospective
// ==============================
=== Weekly Scrum and Sprint Retrospective / 03.04.2026
#meeting_info(
  date: "03.04.2026",
  sprint: "Sprint 3",
  lead: "Cedric Cathomas",
  scribe: "Dominik Wyss",
  time: "13:00–14:00",
  location: "Online",
  participants: "Nathanael Fässler, Dominik Wyss, Yoris Kucera, Cedric Cathomas",
  links: "-",
)

==== Agenda
#agenda_table((
  [Add new nodes, items, tools],
  [Dominik Wyss],
  [- New ScriptableObjects and Prefabs
  - Updated MainWorld to demo new features],
  [Pixelized Shader],
  [Yoris Kucera],
  [- Working shader with editable fields],
  [Building System],
  [Nathanael Fässler],
  [- Chest as first building block
  - Valid/invalid placement indicator],
  [Inventory UI with system architecture changes],
  [Cedric Cathomas],
  [- Inventory and handslot],
  [Feedback Review 2: FR coloring indicator same as NFR],
  [Cedric Cathomas],
  [],
  [Feedback Review 2: Enemy state machine: add explicit color legend to diagram],
  [Yoris Kucera],
  [],
))

==== What went well
- Good communication within the team
- Strong interest and engagement in the project
- Attention to detail
- Review comments were questioned and discussed, not just blindly accepted
- Everything came together into a working game

==== What didn’t
- Conflicting ideas about how to implement features and resulting dependencies sometimes disrupted the workflow.
- Tickets should be described more clearly to support conceptual implementation
- Planned too much
- The 3D player model was more difficult than expected

==== Todos
#todo_table((
  [Send message to advisor about feedback from last review],
  [Dominik],
  [10.04.2026],
  [Send message to advisor about review 3, point A5. Remove A5 from checklist because we don't have external systems.],
  [Dominik],
  [10.04.2026],
))

