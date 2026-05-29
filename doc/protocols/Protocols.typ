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
#include "08_Sprint_Review_(Advisor)_24-03-2026.typ"
#include "09_Weekly_Scrum_27-03-2026.typ"
#include "10_Sprint_Retrospective_03-04-2026.typ"
#include "11_Sprint_Planning_03-04-2026.typ"
#include "12_Sprint_Review_(Advisor)_14-04-2026.typ"
#include "13_Sprint_Retrospective_18-04-2026.typ"
#include "14_Sprint_Planning_18-04-2026.typ"
#include "15_Weekly_Scrum_24-04-2026.typ"
#include "16_Sprint_Retrospective_30-04-2026.typ"
#include "17_Sprint_Planning_01-05-2026.typ"
#include "18_Sprint_Review_(Advisor)_05-05-2026.typ"
#include "19_Weekly_Scrum_08-05-2026.typ"
#include "20_Sprint_Retrospective_15-05-2026.typ"
#include "21_Sprint_Planning_15-05-2026.typ"
#include "22_Sprint_Review_(Advisor)_19-05-2026.typ"
#include "23_Weekly_Scrum_22-05-2026.typ"
#include "24_Sprint_Retrospective_29-05-2026.typ"
#include "25_Weekly_Scrum_29-05-2026.typ"
