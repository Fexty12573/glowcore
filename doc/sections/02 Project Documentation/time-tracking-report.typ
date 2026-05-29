== Time Tracking Report
=== Time Tracking
Every member tracks time on Jira on the corresponding task/story. Time spent in meetings is documented in the meeting protocols. The time is logged in 15 minute blocks in the format e.g. 3h 15m.

=== Time Statistics
The time is grouped by member and by sprint. Per sprint and person the time is rounded to the nearest hour.
#figure(
  table(
    columns: (0.6fr, 1fr, 1fr, 1fr, 1fr, 0.6fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    align: left,

    [*Sprint*], [*Cedric\ Cathomas*], [*Nathanael Fässler*], [*Yoris Kucera*], [*Dominik Wyss*], [*Total*],
    [1], [23h], [29h], [21h], [24h], [97h],
    [2], [22h], [20h], [19h], [23h], [84h],
    [3], [18h], [13h], [19h], [15h], [65h],
    [4], [25h], [13h], [15h], [17h], [70h],
    [5], [7h], [14h], [18h], [15h], [54h],
    [6], [20h], [23h], [20h], [19h], [82h],
    [7], [8h], [17h], [10h], [18h], [53h],
    [Total], [123h], [129h], [122h], [131h], [505h],
  ),
  caption: [Time Statistics],
  supplement: [Table],
)
