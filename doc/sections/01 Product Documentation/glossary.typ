== Glossary


#figure(
  table(
    columns: (0.4fr, 1fr),
    stroke: 0.5pt + gray,
    fill: (x, y) => if y == 0 { luma(230) },
    [*Term*], [*Definition*],
    [Grid], [Refers to the 2D Grid that divides the GlowCore world into rows and columns.],
    [Tile], [A Tile is a single unit of space in the grid and has the form of a square.],
    [Node],
    [A Node refers to an object that is occupying a Tile. Not every Tile has a Node, but every Node belongs to a Tile.],

    [World Item],
    [World Items are items that can be picked up by the player and physically exist somewhere in the GlowCore world. In contrast to Nodes they do not belong to a Tile and do not have a hitbox.],

    [Inventory Item],
    [Inventory Items refer to the items that the player posseses. Certain Nodes such as Chests can also hold Inventory Items],
  ),
  caption: [Glossary],
  supplement: [Table],
)



