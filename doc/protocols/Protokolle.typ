#import "/lib.typ": titel_size_depth_5, text_size, main_color_table, main_color_link_underline
#import "@preview/cmarker:0.1.2"

#show heading: it => {
  text(size: titel_size_depth_5, it.body)
  linebreak()
}

#show heading.where(level: 2): it => {
  text(size: text_size, it.body)
  linebreak()
}

#show heading.where(level: 3): it => {
  text(size: text_size, it.body)
  linebreak()
}

#set heading(outlined: false)

#show strong: it => {
  text(size: titel_size_depth_5, weight: 600, it.body)
}

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

#let protocols = (
  "YYYY_MM_DD.md",
)

#let index = 0
#for file in protocols {
  box(
    pad(5mm, cmarker.render(read(file))),
    fill: main_color_table,
  )
  index += 1
  if index != protocols.len() {
    v(5mm)
  }
}
