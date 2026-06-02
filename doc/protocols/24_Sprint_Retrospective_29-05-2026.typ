#import "../lib.typ": agenda_table, meeting_info, todo_table

// ==============================
// 3) Sprint Retrospective
// ==============================
=== Weekly Scrum and Sprint Retrospective / 29.05.2026
#meeting_info(
  date: "29.05.2026",
  sprint: "Sprint 7",
  lead: "Dominik Wyss",
  scribe: "Yoris Kucera",
  time: "13:00–13:40",
  location: "Online",
  participants: "Nathanael Fässler, Dominik Wyss, Yoris Kucera, Cedric Cathomas",
  links: "-",
)

==== Agenda
#agenda_table((
  [Improved Fog],
  [Dominik Wyss],
  [
    - World border is more clear now because of improved fog
  ],
  [Complete World],
  [Nathanael Fässler],
  [
    - Game World is now built completely
    - New map item to help navigate
    - There are rivers now
    - New potions
    - Smoother camera and movement
  ],
  [Item Icons],
  [Cedric Cathomas],
  [
    - All items have their own icons now
    - New Unity scene to generate these icons
    - Integrated directly into item prefabs
  ],
))

==== What went well
- Game looks good
- "Mandatory" tasks are almost all completed
- Icons look cool
- Automation is good, game has proper progression

==== What didn't
- Still many open tasks
- Merge conflicts in Unity Scene files
- Pull Requests took a long time to get approved
- Still a few annoying bugs

==== Todos
#todo_table((
  [Turn in final documentation on Moodle],
  [All],
  [05.06.2026],
  [Write personal Reports],
  [All],
  [02.06.2026],
  [Release Game on Steam],
  [Nathanael Fässler],
  [05.06.2026],
))

