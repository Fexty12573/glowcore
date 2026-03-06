#import "../lib.typ": titel_size_depth_5, text_size, main_color_table, main_color_link_underline
#import "@preview/cmarker:0.1.2"

#show link: it => {
  highlight([
      #underline(offset: 0.7mm, stroke: main_color_link_underline, it)
    ],
    fill: main_color_table
  )
}

#show ref: it => {
  highlight([
      #underline(offset: 0.7mm, stroke: main_color_link_underline, it)
    ],
    fill: main_color_table
  )
}





// include all meeting minutes (weekly scrum and sprint retrospective are combined)

#include "01_Weekly_Scrum_28-02-2026.typ"
#include "02_Sprint_Retrospective_06-03-2026.typ"




