== Time Tracking Report
=== Time Tracking
Every member tracks time on Jira on the corresponding task/story. Time spent in meetings is documented in the meeting protocols. The time is logged in 15 minute blocks in the format e.g. 3h 15m.

=== Time Statistics
The time is grouped by member and by sprint. Per sprint and person the time is rounded to the nearest hour.
#figure(
  table(
    columns: (0.6fr, 1fr, 1fr, 1fr, 1fr, 1fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    align: left,

    [*Sprint*], [*Cedric Cathomas*], [*Nathanael Fässler*], [*Yoris Kucera*], [*Dominik Wyss*], [*Total*],
    [1], [1h], [1h], [1h], [1h], [1h],
    [2], [], [], [], [], [],
    [3], [], [], [], [], [],
    [4], [], [], [], [], [],
    [5], [], [], [], [], [],
    [6], [], [], [], [], [],
    [7], [], [], [], [], [],
    [Total], [], [], [], [], [],



  ),
  caption: [Time Statistics Sprint 1],
  supplement: [Table]
)
// TODO: The goal is to show stakeholders that the project is on track.
// Cover two topics:
//
// 1. Describe HOW you track time in your project (avoid duplication with Project Plan).
//
// 2. Current time tracking statistics (keep it high-level):
//    - How much time invested in total? How much remaining?
//    - How much time per iteration (1, 2, 3, ...)?
//    - How much time per topic?
//    Diagrams work very well here.
//    Low-level details (e.g. time on task XYZ) are best kept in the tracking tool.
