#show outline.where(target: figure.where(kind: image)).or(outline.where(target: figure.where(kind: table))): it => {
  text(
    top-edge: 0mm,
    it,
  )
}

= List of Figures
#outline(title: none, target: figure.where(kind: image))

#pagebreak()

= List of Tables
#outline(title: none, target: figure.where(kind: table))
