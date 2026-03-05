#import "../lib.typ": meeting_info, todo_table

// ==============================
// 4) Sprint Planning
// ==============================
=== Sprint Planning / YYYY-MM-DD
#meeting_info(
  sprint: "Sprint 4",
  lead: "PO / SM",
  scribe: "Name",
  time: "16:00–17:00",
  location: "Room / Online",
  participants: "Team A, Team B",
  links: "Backlog link"
)

==== Sprint goal
- ...

==== Selected backlog (commitment)
#table(
  columns: (0.9fr, 2.2fr, 0.8fr, 1fr),
  stroke: 0.5pt + gray,
  fill: (x, y) => if y == 0 { luma(230) },
  [*ID*], [*Item*], [*Est.*], [*Owner*],
  [T-123], [Implement login flow], [5], [Name],
  [T-124], [Add CI pipeline], [3], [Name],
)

==== Risks / Dependencies
- Dependency: ...
- Risk: ...

==== Todos
#todo_table((
  [Prepare demo script], [Name], [YYYY-MM-DD], [Open],
))

#pagebreak()