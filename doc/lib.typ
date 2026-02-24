// © 2025 Vanessa Alves & Marco Schnider
// Mitwirkende: [...]

#import "@preview/codelst:2.0.1": sourcecode
#import "@preview/tablex:0.0.8": tablex

#let text_size = 10pt

#let titel_size_depth_1_fist = 32pt
#let titel_size_depth_1_second = 31pt
#let titel_size_depth_2 = 30pt
#let titel_size_depth_3 = 18pt
#let titel_size_depth_4 = 16pt
#let titel_size_depth_5 = 12pt

#let title_weight = 600
#let title_top_edge = 3mm
#let title_bottom_edge = -3mm

#let main_color_table = rgb("#ffe4ee")
#let main_color_link_background = rgb("#fff7fa")
#let main_color_link_underline = rgb("#d62863")

// Colors and Cells for Project Plan Table
#let lightgreen = green.lighten(40%)
#let lightred = red.lighten(40%)
#let lightblue = blue.lighten(40%)
#let lightorange = orange.lighten(40%)
#let lightpurple = purple.lighten(40%)

#let lightgreenCell(name) = table.cell(fill: lightgreen, name)
#let lightredCell(name) = table.cell(fill: lightred, name)
#let lightblueCell(name) = table.cell(fill: lightblue, name)
#let lightorangeCell(name) = table.cell(fill: lightorange, name)
#let lightpurpleCell(name) = table.cell(fill: lightpurple, name)

// Template based on LaTeX Article Class
#let article(
  lang:"en",
  eq-numbering:none,
  text-size:text_size,
  page-numbering: none,
  page-numbering-align: center,
  heading-numbering: "1.1  ",
  top-right-logo: none,
  body) = {
  
  // Document's basic properties
  set page(
    margin: (left: 20mm, right: 15mm, top: 25mm, bottom: 15mm),
    numbering: page-numbering,
    number-align: page-numbering-align,
    foreground: context [
      #place(
        top + right,
        dx: -15mm,
        dy: 6mm,
        top-right-logo
      )
    ],
  )
  set text(font: ("Calibri", "Libertinus Serif"), lang:lang, size: text_size)

  set math.equation(numbering: eq-numbering)
  show math.equation: set text(weight: 400)

  set heading(numbering: (..ns) => {
    let ns = ns.pos()
    if ns.len() == 1 {
      return numbering("I", ..ns)
    } else {
      ns.remove(0)
      return numbering("1.1", ..ns)
    }
  })

  show heading.where(level: 1): it => {
    let level1_selector = selector(heading).before(here())
    let level1_counter = counter(level1_selector)

    // Continue level 2 counter in new main chapter
    let level2_selector = selector(heading.where(level: 2)).before(here())
    let level2_counter =  counter(level2_selector)
    let current_outline_num = level2_counter.final()
    current_outline_num.at(0) = level1_counter.final()
    counter(heading).update(current_outline_num.flatten())

    text(
      titel_size_depth_1_fist, 
      weight: title_weight, 
      bottom-edge: -10mm, 
      it.body
    )
  }
  show heading.where(level: 2): set text(
    titel_size_depth_2, 
    weight: title_weight, 
    top-edge: title_top_edge,
    bottom-edge: title_bottom_edge
  )
  show heading.where(level: 3): set text(
    titel_size_depth_3, 
    weight: title_weight, 
    top-edge: title_top_edge,
    bottom-edge: title_bottom_edge
  )
  show heading.where(level: 4): set text(
    titel_size_depth_4, 
    weight: title_weight, 
    top-edge: title_top_edge,
    bottom-edge: title_bottom_edge
  )
  show heading.where(level: 5): set text(
    titel_size_depth_5, 
    weight: title_weight, 
    top-edge: title_top_edge,
    bottom-edge: 0mm
  )

  set outline(
    indent: auto
  )
  show outline.entry.where(level: 1): it => {
    v(20pt, weak: true)
    text(size: titel_size_depth_5, bottom-edge: -1mm, strong(it))
  }

  // Referencing Figures
  show figure.where(kind: table): set figure(supplement:[Tab.], numbering: "1") if lang == "en" 
  show figure.where(kind: image): set figure(supplement:[Abb.], numbering: "1",) if lang == "en"
  show figure.where(kind: raw): set figure(supplement:[Listing], numbering: "1",) if lang == "en"

  show figure.where(kind: table): set figure(supplement:[Tab.], numbering: "1") if lang == "en"
  show figure.where(kind: image): set figure(supplement:[Figure.], numbering: "1",) if lang == "en"
  show figure.where(kind: raw): set figure(supplement:[Listing], numbering: "1",) if lang == "en"

  // Table style
  set table(
    stroke: black,
    gutter: auto,
    fill: none,
    inset: (right: 1.5em)
  )

  // Configure figures (tables)
  show figure.where(kind: table): it => {
    set align(center)
    v(titel_size_depth_5, weak: true)
    it.body
    if it.has("caption") {
      v(if it.has("gap") { it.gap } else { 16pt }, weak: true)
      strong(it.supplement)
      if it.numbering != none {
        [ ]
        strong(it.counter.display(it.numbering))
      }
      [*: *]
      it.caption.body
    }
    v(14pt, weak: true)
  }

  // Configure figures (images)
  show figure.where(kind: image): it => {
    show: pad.with(x: 24pt)
    set align(center)
    v(titel_size_depth_5, weak: true)
    it.body
    if it.has("caption") {
      v(if it.has("gap") { it.gap } else { 16pt }, weak: true)
      strong(it.supplement)
      if it.numbering != none {
        [ ]
        strong(it.counter.display(it.numbering))
      }
      [*: *]
      it.caption.body
    }
    v(14pt, weak: true)
  }

  // Configure code (raw)
  show figure.where(kind: raw): it => {
    show: pad.with(x: 24pt)
    set align(center)
    v(titel_size_depth_5, weak: true)
    block(
      radius: 4pt,
      fill: luma(250),
      stroke: luma(230),
      inset: (
        x: 1em,
        y: 1em
      ),
      it.body
    )    
    if it.has("caption") {
      v(if it.has("gap") { it.gap } else { 16pt }, weak: true)
      strong(it.supplement)
      if it.numbering != none {
        [ ]
        strong(it.counter.display(it.numbering))
      }
      [*: *]
      it.caption.body
    }
    v(14pt, weak: true)
  }

  show link: it => {
    highlight([
        #set text(weight: 500)
        #underline(offset: 0.7mm, stroke: main_color_link_underline, it)
      ], 
      fill: main_color_link_background
    )
  }

  show ref: it => {
    highlight([
        #set text(weight: 500)
        #underline(offset: 0.7mm, stroke: main_color_link_underline, it)
      ], 
      fill: main_color_link_background
    )
  }

  // Main body
  set par(justify: true)
  body
}

#let maketitle(
  title: "", 
  sub-title: "",
  version: "", 
  gitVersion: "",
  date: none, 
  authors: (),
  advisor: "",
  background: none,
  overlay: none
) = {
  set document(author: authors, title: title)
  let authors-text = {authors.join("\n"+[#h(7em)])}
  place(top + left, dx: -35mm, dy: 5mm, background)
  place(top + left, dx: -20mm, dy: -40mm, overlay)

  place(bottom + right, dy: 0mm,
    table(
      stroke: none,
      align: left,
      columns: 50%,
      text(weight: title_weight, titel_size_depth_1_fist, title),
      text(titel_size_depth_3, sub-title),
      v(0.5em),

      text("Version: " + version ),
      text("Date: " + date ),
      text("Git Version: " + gitVersion),
      v(0.5em),
      text([*Project Team:* #h(1em)] + authors-text),
      v(0.5em),
      text([*Project Advisor:*#h(0.4em)] + advisor),
      v(0.5em),
      align(left)[School of Computer Science \ OST Eastern Switzerland University of Applied Sciences]

    )
  )
}

// Textbox to highlight Text
#let textbox(content) = box(
  width: 100%,
  fill: rgb(225, 255, 184),
  inset: 11pt,
  stroke: none,
)[
  #content
]

// Code Styling
#let sourcecode = sourcecode.with(frame: (code) => block(
  radius: 4pt,
  fill: luma(255),
  stroke: luma(230),
  inset: (
    x: 0.5em,
    y: 0.25em
  ),
  text(size: 5.5pt, code)
  ),
  gutter: 4pt
)

#let title_without_numbering(
  title: "",
  depth: 1,
  top_edge: title_top_edge,
  bottom_edge: title_bottom_edge
) = {
  text(
    title,
    weight: title_weight,
    top-edge: top_edge,
    bottom-edge: (
      if (depth == 5 ) { -1mm 
      } else { title_bottom_edge }
    ),
    size: (
      if depth == 1 { titel_size_depth_1_fist 
      } else if depth == 2 { titel_size_depth_2 
      } else if depth == 3 { titel_size_depth_3 
      } else if depth == 4 { titel_size_depth_4 
      } else if depth == 5 { titel_size_depth_5 }
    )
  )
}

#let fr_table(
  id: [],
  name: [],
  beschreibung: [],
  fr_caption: []
) = {
  set table(
    align: start,
    columns: (auto, 1fr),
    fill: (x, _) =>
      if calc.even(x) { main_color_table }
      else { none 
    }
  )
  figure(
    table(
      [#strong("ID")], strong(id), 
      [#strong("Name")], name, 
      [#strong("Beschreibung")], beschreibung,
    ), 
    caption: fr_caption
  )
}

#let use_case(
  id: [],
  name: [],
  akteur: [],
  beschreibung: [],
  voraussetzung: [],
  standard_verlauf: [],
  alternativ_verlauf: none,
  nachbedingung: [],
  priorität: [],
  resultat: [],
  uc_caption: [],
  status_color: green
) = {
  set table(
    align: start,
    columns: (1fr, 4fr, 0.2fr),
    fill: (x, _) =>
      if calc.even(x) { main_color_table }
      else { none 
    }
  )
  figure(
    table(
      [#strong("ID")], strong(id),  table.cell(fill: status_color, []),
      [#strong("Name")], table.cell(colspan: 2, name), 
      [#strong("Akteur")], table.cell(colspan: 2, akteur), 
      [#strong("Beschreibung")], table.cell(colspan: 2, beschreibung), 
      [#strong("Voraussetzung")], table.cell(colspan: 2, voraussetzung), 
      [#strong("Standard-Verlauf")], table.cell(colspan: 2, standard_verlauf), 
      [#strong("Alternativ-Verlauf")], table.cell(colspan: 2, alternativ_verlauf), 
      [#strong("Nachbedingung")], table.cell(colspan: 2, nachbedingung), 
      [#strong("Priorität")], table.cell(colspan: 2, priorität), 
      [#strong("Resultat")], table.cell(colspan: 2, resultat), 
    ), 
    caption: uc_caption
  )
}

#let nfr_table(
  id: [],
  beschreibung: [],
  anforderungen: [],
  priorität: [],
  messung: [],
  testen: [],
  resultat: [],
  nfr_caption: [],
  status_color: blue,
) = {
  set table(
    align: start,
    columns: (1fr, 4.1fr, 0.2fr),
    fill: (x, _) =>
      if calc.even(x) { main_color_table }
      else { none 
    }
  )
  figure(
    table(
      [#strong("ID")], strong(id), table.cell(fill: status_color, []), 
      [#strong("Beschreibung")], table.cell(colspan: 2, beschreibung), 
      [#strong("Anforderungen")], table.cell(colspan: 2, anforderungen), 
      [#strong("Priorität")], table.cell(colspan: 2, priorität), 
      [#strong("Messung")], table.cell(colspan: 2, messung), 
      [#strong("Testen")], table.cell(colspan: 2, testen), 
      [#strong("Resultat")], table.cell(colspan: 2, resultat), 
    ), 
    caption: nfr_caption
  )
}

#let testAblauf_table(
  id: [],
  name: [],
  beschreibung: [],
  methode: [],
  relevanz: [],
  testumgebung: [],
  intervall: [],
  testAblauf_caption: [],
) = {
  set table(
    align: start,
    columns: (1fr, 4.1fr),
    fill: (x, _) =>
      if calc.even(x) { main_color_table }
      else { none 
    }
  )
  figure(
    table(
      [#strong("ID")], strong(id), 
      [#strong("Name")], name, 
      [#strong("Beschreibung")], beschreibung, 
      [#strong("Methode")], methode, 
      [#strong("Relevant für")], relevanz, 
      [#strong("Testumgebung")], testumgebung, 
      [#strong("Intervall")], intervall, 
    ), 
    caption: testAblauf_caption
  )
}

#let testReport_table(
  id: [],
  date: [],
  input: [],
  expectedOutput: [],
  actualOutput: [],
  testReport_caption: [],
) = {
  set table(
    align: start,
    columns: (1fr, 4.1fr),
    fill: (x, _) =>
      if calc.even(x) { main_color_table }
      else { none 
    }
  )
  figure(
    table(
      [#strong("ID")], strong(id), 
      [#strong("Datum")], date, 
      [#strong("Input")], input, 
      [#strong("Erwartetes Resultat")], expectedOutput, 
      [#strong("Tatsächliches Resultat")], actualOutput, 
    ), 
    caption: testReport_caption
  )
}


// Part title page: centered on its own page, registered in table of contents
#let part_page(number, title) = [
  #align(center + horizon, [
    #text(24pt, weight: title_weight)[Part #number]
    #v(0em)
    #heading(level: 1, title)
  ])
  #pagebreak()
]


// ==============================
// Meeting Minutes
// ==============================

// --- Helpers (reusable tables) ---
#let meeting_info(
  date: "YYYY-MM-DD",
  time: "HH:MM–HH:MM",
  sprint: "-",
  location: "Room/Online",
  participants: "Names",
  excused: "-",
  absent: "-",
  lead: "Name",
  scribe: "Name",
  links: "-"
) = [
  #table(
    columns: (1.2fr, 2.8fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    [*Field*], [*Value*],
    [Date], [#date],
    [Time], [#time],
    [Sprint], [#sprint],
    [Location], [#location],
    [Participants], [#participants],
    [Excused], [#excused],
    [Absent], [#absent],
    [Meeting lead], [#lead],
    [Minutes by], [#scribe],
    [Links], [#links],
  )
]

#let agenda_table(rows) = [
  #table(
    columns: (2fr, 1fr, 2fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    [*Agenda item*], [*Owner*], [*Result / Decision*],
    ..rows
  )
]

#let todo_table(rows) = [
  #table(
    columns: (1.35fr, 1fr, 1fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    [*Todo*], [*Owner*], [*Due*],
    ..rows
  )
]

#let next_dates(rows) = [
  #table(
    columns: (2fr, 1fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    [*Next date*], [*When*],
    ..rows
  )
]

