#import "../lib.typ": agenda_table, meeting_info, next_dates, todo_table

// ==============================
// 1) Sprint Review (Advisor)
// ==============================
=== Sprint Review (Advisor) / DD.MM.YYYY
#meeting_info(
  date: "DD.MM.YYYY",
  sprint: "Sprint 3",
  lead: "Yoris Kucera",
  scribe: "Nathanael Fässler",
  time: "15:00–17:00",
  location: "Building 8",
  participants: "Thomas Bocek, Dominik Wyss,  Yoris Kucera, Nathanael Fässler, Cedric Cathomas",
  links: "Repo / Jira / Doc link",
)

==== Agenda
#agenda_table((
  [Approval last minutes],
  [Lead],
  [Approved / changes: ...],
  [Demo],
  [Dev],
  [OK / open issues: ...],
  [Feedback],
  [Advisor],
  [Notes: ...],
  [Decisions],
  [Lead],
  [Recorded decisions: ...],
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
  [Update architecture doc section X],
  [Name],
  [DD.MM.YYYY],
  [Open],
  [Fix bug #123],
  [Name],
  [DD.MM.YYYY],
  [Open],
))

==== Next dates
#next_dates((
  [Next advisor review],
  [DD.MM.YYYY HH:MM],
))

#pagebreak()
