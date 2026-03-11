#import "../lib.typ": main_color_link_underline, main_color_table, text_size, titel_size_depth_5
#import "@preview/cmarker:0.1.2"

#show link: it => {
  highlight(
    [
      #underline(offset: 0.7mm, stroke: main_color_link_underline, it)
    ],
    fill: main_color_table,
  )
}

#show ref: it => {
  highlight(
    [
      #underline(offset: 0.7mm, stroke: main_color_link_underline, it)
    ],
    fill: main_color_table,
  )
}





// include all meeting minutes (weekly scrum and sprint retrospective are combined)

#include "01_Weekly_Scrum_28-02-2026.typ"
#include "02_Sprint_Retrospective_06-03-2026.typ"
#include "03_Sprint_Planning_06-03-2026.typ"
#include "04_Sprint_Review_(Advisor)_10-03-2026.typ"
#include "05_Weekly_Scrum_13-03-2026.typ"
#include "06_Sprint_Retrospective_20-03-2026.typ"
#include "07_Sprint_Planning_20-03-2026.typ"


