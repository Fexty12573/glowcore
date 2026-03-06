#import "../lib.typ": meeting_info, todo_table,agenda_table

=== Weekly Scrum / 28.02.2026
#meeting_info(
  date: "28.02.2026",
  sprint: "Sprint 1",
  lead: "Yoris Kucera",
  scribe: "Nathanael Fässler",
  time: "11:00–14:00",
  location: "Online",
  participants: "Nathanael Fässler, Dominik Wyss, Yoris Kucera",
  excused: "Cedric Cathomas",
  links: "-"
)

==== Agenda
#agenda_table((
  [GitHub Setup], [Yoris], [
    - We switched from GitLab to GitHub
    The following rules were discussed:
    - Pull Request requires one approval that is not the same person as the author.
    - Merging Methods are Merge and Squash.
    - Every Jira Task corresponds to one branch.
  ],
  [CICD Pipeline Setup], [Dominik], [
    - Web build and .exe build succeeded.
    - Pages Architecture with main and dev branches decided
  ],
  [Timetracking], [Dominik], [
    - We won't use GitLab timetracking because we switched to GitHub
    - Timetracking is in Jira: Everyone tracks time for individual Tasks/Stories.
  ],
  [Unity/Coding Setup], [Yoris], [
    Coding Guidelines decided
  ],
  [Steam Release], [Nathanael], [
    - Registered on Steamworks
    - Up to 30 days waiting time until application is processed
    - Maybe we need to make a contract with OST.
  ],
  [Testing], [Nathanael], [
    Unit-Test and Integration-Test Strategy discussed.
  ], [Long/Shorttermplan], [Nathanael], [
    - Hierarchy: There are Epics which contain Stories.
    - Everything that is not a game-feature is a task.
    - Longtermplan with epics according to the Jira Timeline
    - Shorttermplan per sprint on the Jira board
    - Priority is defined by the order in the backlog in Jira
    - MVP is defined
    - We will estimate Tasks/Stories with Storypoints
    - On Jira we can create Bug-Tickets which we will not estimate with Storypoints
  ]
))

==== Todos
#todo_table((
  [Finish CICD Pipeline], [Dominik], [06.03.2026],
  [Ask Advisor about contract], [Nathanael], [04.03.2026],
  [Ask Advisor about project plan], [Nathanael], [04.03.2026],
))

#pagebreak()