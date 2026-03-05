#import "../lib.typ": meeting_info, todo_table

// ==============================
// 3) Sprint Retrospective
// ==============================
=== Sprint Retrospective / YYYY-MM-DD
#meeting_info(
  sprint: "Sprint 3",
  lead: "Scrum Master",
  scribe: "Name",
  time: "15:00–16:00",
  location: "Room / Online",
  participants: "Team A, Team B",
  links: "Retro board link"
)

==== What went well
- ...

==== What didn’t
- ...

==== Improvements (Actions)
#todo_table((
  [Introduce PR review rule: 1 reviewer min], [Name], [YYYY-MM-DD], [Open],
  [Add DoD checklist to every ticket], [Name], [YYYY-MM-DD], [Open],
))

==== Retro decisions
- Process decision: ...

#pagebreak()