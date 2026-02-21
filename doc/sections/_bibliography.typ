// Citations: use @key to cite a source, e.g. @larman2004

#show outline.where(target: figure.where(kind: image)).or(outline.where(target: figure.where(kind: table))): it => {
  text(
    top-edge: 0mm,
    it
  )
}

= Bibliography
#bibliography(title: none, "../references.bib", style: "apa")

#pagebreak()

= List of Figures
#outline(title: none, target: figure.where(kind: image))

#pagebreak()

= List of Tables
#outline(title: none, target: figure.where(kind: table))

#pagebreak()

= Code Listings
#outline(title: none, target: figure.where(kind: raw))
