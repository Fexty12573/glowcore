#import "lib.typ": *


#show: article.with(
  lang: "en",
  top-right-logo: image("resources/OST-Logo.png", width: auto, height: 15mm, fit: "cover"),
)



// ── Cover Page ──
#maketitle(
  title: "GlowCore",
  sub-title: "SE Project Documentation",
  version: "__VERSION__",
  gitVersion: "__GIT_VERSION__",
  date: datetime.today().display("[day].[month].[year]"),
  authors: ("Cedric Cathomas", "Yoris Kucera", "Dominik Wyss", "Nathanael Fässler"),
  advisor: "Thomas Bocek",
  background: image("resources/Background.png", width: 245mm, height: auto, fit: "cover"),
  overlay: image("resources/Overlay.png", width: 210mm, height: 300mm, fit: "cover")
)

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
      [], [
        #counter(page).display(
          "i",
          both: false,
        )
      ], []
    )
  ]
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
      [], [
        #counter(page).display(
          "1",
          both: false,
        )
      ], []
    )
  ]
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
