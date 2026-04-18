#import "../lib.typ": agenda_table, meeting_info, next_dates, todo_table

// ==============================
// 1) Sprint Review (Advisor)
// ==============================
=== Sprint Review (Advisor) / 14.04.2026
#meeting_info(
  date: "14.04.2026",
  sprint: "Sprint 4",
  lead: "Yoris Kucera",
  scribe: "Nathanael Fässler",
  time: "15:00–16:00",
  location: "Building 8",
  participants: "Thomas Bocek, Dominik Wyss, Yoris Kucera, Nathanael Fässler, Cedric Cathomas",
  links: "GitHub repository + Jira board (internal team links)",
)

==== Agenda
#agenda_table((
  [Showcase DEV-environment],
  [Yoris Kucera],
  [GitHub Action minutes are scarce. We should search for a better solution.],
  [Test Concept],
  [Yoris Kucera],
  [Positive: The idea of Integration Tests was adapted to the domain to use Playmode tests.],
  [C4 Architecture],
  [Yoris Kucera],
  [Well-documented and focuses on high-level topics.],
  [Techstack & Patterns],
  [Yoris Kucera],
  [Use of component-based architecture makes sense. Unity inspector variables are not really dependency injection.],
  [External Dependencies],
  [Thomas Bocek],
  [Missing specification of Steam as an external dependency.],
  [Alpha Demo],
  [Nathanael Fässler],
  [Core game loop is working and playable. Player usability has to be improved.],
  [Extensibility Demo],
  [Nathanael Fässler],
  [Demonstration to show how a new Node can be added without writing code.],
  [Savefile Structure],
  [Yoris Kucera],
  [Could easily be changed to support generated worlds by including the seed in the header.],
))

==== Todos
#todo_table((
  [Research solution to reduce GitHub Runner minutes],
  [Yoris Kucera],
  [28.04.2026],
  [Document Steam as a external dependency],
  [Dominik Wyss],
  [28.04.2026],
))

==== Next dates
#next_dates((
  [Review 4 - Quality],
  [28.04.2026 15:00],
))

#pagebreak()
