#import "lib.typ": *


#show: article.with(
  lang: "en",
  top-right-logo: image("resources/OST-Logo.png", width: auto, height: 15mm, fit: "cover"),
)

#show link: set text(fill: blue.darken(20%))




// ── Cover Page ──
#set document(
  title: "GlowCore",
  author: ("Cedric Cathomas", "Yoris Kucera", "Dominik Wyss", "Nathanael Fässler"),
)

#v(1fr)

// Project mark, document title and type.
#align(center)[
  #image("resources/Logo.png", width: 115mm)
  #v(9mm)
  #text(titel_size_depth_1_fist, weight: title_weight)[GlowCore]
  #v(2mm)
  #text(titel_size_depth_3, fill: luma(30%))[SE Project Documentation]
  #v(5mm)
  #line(length: 38%, stroke: 0.5pt + luma(55%))
]

#v(1fr)

// Project metadata, formally aligned.
#align(center)[
  #set text(size: text_size)
  #grid(
    columns: (auto, auto),
    column-gutter: 1.6em,
    row-gutter: 0.85em,
    align: (right + top, left + top),
    text(weight: 600)[Authors],
    [Cedric Cathomas, Yoris Kucera, \ Dominik Wyss, Nathanael Fässler],
    text(weight: 600)[Advisor], [Thomas Bocek],
    text(weight: 600)[Date], datetime.today().display("[day].[month].[year]"),
    text(weight: 600)[Version], text("__VERSION__"),
    text(weight: 600)[Git Version], text("__GIT_VERSION__"),
  )
]

#v(16mm)

// Institutional footer.
#align(center, text(fill: luma(40%))[
  OST -- Eastern Switzerland University of Applied Sciences \
  School of Computer Science
])

// ── Table of Contents ──
#pagebreak()
#context { counter(page).update(1) }
#set page(
  header: context [
    #text("")
  ],
  footer: context [
    #table(
      columns: (1fr, auto, 1fr),
      stroke: none,
      inset: 0mm,
      [],
      [
        #counter(page).display(
          "i",
          both: false,
        )
      ],
      [],
    )
  ],
)

#title_without_numbering(title: "Contents")
#outline(
  title: none,
  indent: auto,
)

// ── Main Content ──
#pagebreak()
#context { counter(page).update(1) }
#set page(
  header: context [
    #text("GlowCore | SE Project ")
  ],
  footer: context [
    #table(
      columns: (1fr, auto, 1fr),
      stroke: none,
      inset: 0mm,
      [],
      [
        #counter(page).display(
          "1",
          both: false,
        )
      ],
      [],
    )
  ],
)

// ── Part I: Management Summary ──
#include "sections/_management-summary.typ"
#pagebreak()

// ── Part II: Product Documentation ──
#include "sections/_product-documentation.typ"
#pagebreak()

// ── Part III: Project Documentation ──
#include "sections/_project-documentation.typ"
#pagebreak()

// ── Bibliography & Indexes ──
#include "sections/_bibliography.typ"

// ── Optional sections (uncomment if needed) ──
// #pagebreak()
// #include "sections/_glossary.typ"
// #pagebreak()
// #include "sections/_appendix.typ"
