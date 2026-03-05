#import "../lib.typ": meeting_info, todo_table, agenda_table, next_dates

// ==============================
// 1) Sprint Review (Advisor)
// ==============================
=== Sprint Review (Advisor) / YYYY-MM-DD
#meeting_info(

  sprint: "Sprint 3",
  lead: "Name",
  scribe: "Name",
  time: "15:00–17:00",
  location: "Online (Teams)",
  participants: "Team A, Team B, Advisor",
  links: "Repo / Jira / Doc link"
)

==== Agenda
#agenda_table((
  [Approval last minutes], [Lead], [Approved / changes: ...],
  [Demo], [Dev], [OK / open issues: ...],
  [Feedback], [Advisor], [Notes: ...],
  [Decisions], [Lead], [Recorded decisions: ...],
))

==== Demo / Increment
- Shown: ...
- Not shown (and why): ...

==== Advisor feedback (key points)
- ...
- ...

==== Decisions
- Decision: ... (Reason: ...)


==== Todos
#todo_table((
  [Update architecture doc section X], [Name], [YYYY-MM-DD], [Open],
  [Fix bug #123], [Name], [YYYY-MM-DD], [Open],
))

==== Next dates
#next_dates((
  [Next advisor review], [YYYY-MM-DD HH:MM],
))

#pagebreak()