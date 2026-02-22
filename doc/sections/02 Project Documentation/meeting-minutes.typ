

#let main_doc = [
  == Meeting Minutes

  #include "../../protocols/Protokolle.typ"
] 

#let protocols_doc = [
  #import "../../lib.typ": *

  #show: article.with(
    top-right-logo: image("../../resources/OST-Logo.png", width: auto, height: 15mm, fit: "cover"),
  )

  #context { counter(page).update(1) }
  #set page( 
    header: context [
      #text("ProjectName | Meeting Minutes | Yoris, Nathanael, Dominic and Cedric")
    ],
    footer: context [
      #table(
        columns: (1fr, auto, 1fr),
        stroke: none,
        inset: 0mm,
        [], [
          #counter(page).display(
            "1",
            both: false,
          )
        ], []
      )
    ]
  )

  #title_without_numbering(title: "GlowCore")

  This document contains all meeting minutes of the project.

  #title_without_numbering(depth: 2, title: "Meeting Minutes")

  #include "../../protocols/Protokolle.typ"
]
