#import "../lib.typ": agenda_table, meeting_info, next_dates, todo_table

// ==============================
// 1) Sprint Review (Advisor)
// ==============================
=== Sprint Review (Advisor) / 24.03.2026
#meeting_info(
  date: "24.03.2026",
  sprint: "Sprint 3",
  lead: "Yoris Kucera",
  scribe: "Cedric Cathomas",
  time: "15:00–16:00",
  location: "Building 8",
  participants: "Thomas Bocek, Dominik Wyss, Yoris Kucera, Nathanael Fässler, Cedric Cathomas",
  links: "GitHub repository + Jira board (internal team links)",
)

==== Agenda
#agenda_table((
  [Documentation status (FR, architecture, protocols)],
  [Yoris Kucera],
  [Overall quality was positively received; minor consistency updates required (see Todos)],
  [Current game state demo],
  [Yoris Kucera],
  [Game is playable as planned for pre-alpha release],
  [Mockups],
  [Yoris Kucera],
  [Gave a clear picture of how our UI will look],
  [Code quality (linter/status)],
  [Yoris Kucera],
  [Everything was good],
  [Advisor feedback],
  [Thomas Bocek],
  [Very good feedback, overall impression better than last week],
))

==== Todos
#todo_table((
  [FR: clarify whether "dedicated player" is categorized as "casual player"],
  [Nathanael Fässler],
  [27.03.2026],
  [FR: align color definitions and structure as in the NFRs],
  [Cedric Cathomas],
  [27.03.2026],
  [Enemy state machine: add explicit color legend to diagram],
  [Yoris Kucera],
  [27.03.2026],
))

==== Next dates
#next_dates((
  [Review 4 - Sprint Review],
  [14.04.2026 15:00],
))

#pagebreak()
