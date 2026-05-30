#import "../lib.typ": agenda_table, meeting_info, next_dates, todo_table

// ==============================
// 1) Sprint Review (Advisor)
// ==============================
=== Sprint Review (Advisor) / 05.05.2026
#meeting_info(
  date: "05.05.2026",
  sprint: "Sprint 6",
  lead: "Dominik Wyss",
  scribe: "Nathanael Fässler",
  time: "15:00–16:00",
  location: "Building 8",
  participants: "Thomas Bocek, Dominik Wyss, Yoris Kucera, Nathanael Fässler, Cedric Cathomas",
  links: "GitHub repository + Jira board (internal team links)",
)

==== Agenda
#agenda_table((
  [GlowCore v0.2.0 demo],
  [Dominik Wyss],
  [Works, but still needs improvements.],
  [GitHub selfhosted runner],
  [Yoris Kucera],
  [Is working. We have to be careful if we make the repository public.],
  [Quality measures],
  [Dominik Wyss],
  [There are files with less than 10 lines of code. In our case it makes sense to keep them separate.],
  [User Tests],
  [Nathanael Fässler],
  [The first two user tests provided valuable feedback.],
  [Code Review],
  [Dominik Wyss],
  [WorldGrid.cs was reviewed. No unclean code found.],
  [Steampage],
  [Dominik Wyss],
  [Looks good.],
))

==== Todos
#todo_table((
  [Document deployment on steam],
  [not assigned],
  [19.05.2026],
))


==== Next dates
#next_dates((
  [Review 5 - Architecture],
  [19.05.2026 15:00],
))

