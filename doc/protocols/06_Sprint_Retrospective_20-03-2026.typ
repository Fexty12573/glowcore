#import "../lib.typ": agenda_table, meeting_info, todo_table

// ==============================
// 3) Sprint Retrospective
// ==============================
=== Weekly Scrum and Sprint Retrospective / 20.03.2026
#meeting_info(
  date: "20.03.2026",
  sprint: "Sprint 2",
  lead: "Yoris Kucera",
  scribe: "Nathanael Fässler",
  time: "13:00–15:00",
  location: "Online",
  participants: "Nathanael Fässler, Dominik Wyss, Yoris Kucera, Cedric Cathomas",
  links: "-",
)


==== Agenda
#agenda_table((
  [Planned Inventory System Architecture],
  [Yoris],
  [
    - ScriptableObject Item represents InventoryItem and WorldItem at the same time.
    - ItemStack ScriptableObject hold Item and amount.
  ],
  [World Grid & GlowCore expansion],
  [Cedric],
  [
    - Showed simple prototype for tree generation on border expansion.
    - Grid represented by 2D Array.
  ],
  [Node Interaction System],
  [Dominik],
  [
    - Setting for Interaction Range for every Node.
    - Problem with InputSystem to track how long a button is held.
  ],
))


==== What went well
- Had fun beginning the work in Unity.
- Good cooperation: Questions by team-members are answered quickly.
- The planning starts to pay off.

==== What didn’t
- Unclear dependencies of tasks.
- CI/CD work can be annoying.

==== Improvements
- Keep Jira Board up to date.
- Better coordinate the task assignment.

#pagebreak()

==== Todos
#todo_table((
  [Send documentation to Advisor],
  [Dominik],
  [21.03.2026],
  [Make Meeting Minutes],
  [Nathanael],
  [23.03.2026],
  [Finish and merge prototype],
  [Yoris],
  [23.03.2026],
))



