#import "../lib.typ": meeting_info, todo_table, agenda_table

=== Weekly Scrum and Sprint Retrospective / 06.03.2026
#meeting_info(
  date: "06.03.2026",
  sprint: "Sprint 1",
  lead: "Nathanael Fässler",
  scribe: "Cedric Cathomas",
  time: "14:00-16:00",
  location: "Online",
  participants: "Nathanael Fässler, Dominik Wyss, Yoris Kucera, Cedric Cathomas",
  excused: "-",
  links: "-"
)

==== Agenda
#agenda_table((
  [Board Review], [All], [
    - Walked through all current board tasks
    - Updated status on completed tasks
  ],
  [Done Tasks and New Functionalities], [All], [
    - Reviewed completed work from Sprint 1
    - Discussed new functionalities and rules
  ],
  [Open Questions], [All], [
    - Discussed open questions regarding project setup and workflow
  ],
  [Unity Graphics in Repository], [Nathanael Fässler], [
    - Only .fbx files are to be committed to the repository for 3D graphics in Unity
  ],
  [Documentation Versioning], [Cedric Cathomas], [
    - Tags for documentation versions start with "d" (e.g. d1.1)
    - Version is updated automatically on pull request to main via GitHub Action
  ],
  [How do we manage smaller tasks that are not worth creating a new task on jira. (small documentation changes)], [Dominik Wyss], [We create ticket for small backlog tasks that do not need an own Jira item so we can reference it on our pull request and use the name of that ticket for the branch],
  [Decide if we need a seperate pdf for protocols], [Cedric Cathomas], [We decided that theres no use in having a seperate page just for meeting minutes, so we have one thing less to worry about]
))

==== Sprint 1 Retrospective

===== What Went Well
- Good and fast communication
- Good planning on Jira
- Good preplanning visible
- Product Owner did a good job
- Good and understandable selection of planning and working tools
- Pipeline and GitHub are working
- Typst is set up
- General setup went very well

===== What Didn't Go Well
- Problems with GitLab
- Didn't have enough time for some tasks

===== Improvements
- Better time management
- Better definitions of done
- Use GitHub from the beginning

==== Todos
#todo_table((
  [Create ticket for small backlog tasks that do not need an own Jira item], [Nathanael Fässler], [07.03.2026],
  [Create all table references so documentation looks nice and page breaks are clean], [Cedric Cathomas], [07.03.2026],
  [Adapt milestone dates to single dates (e.g. 16.02.2026 instead of a time frame)], [Cedric Cathomas], [07.03.2026],
  [clean up documentation (spaces that are too big], [Cedric Cathomas], [07.03.2026],
  [Change date format in documentation to use dots], [Cedric Cathomas], [07.03.2026],
  [Create task for combined build and documentation pipeline], [Nathanael Fässler], [07.03.2026],
  [Use figures for images so they appear in the list of figures], [Nathanael Fässler], [07.03.2026],
  [remove seperate meeting minutes pdf typst code], [Cedric Cathomas], [07.03.2026],
))

#pagebreak()
