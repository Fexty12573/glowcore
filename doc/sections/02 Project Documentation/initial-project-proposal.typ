== Initial Project Proposal

*Project name:* #h(0.5em) GlowCore \ 
*Date: * #h(4em) 19.12.2025

#v(1em)

=== Team Members

+ Cedric Cathomas (#link("mailto:Cedric.Cathomas@ost.ch")[Cedric.Cathomas\@ost.ch])
+ Dominik Wyss (#link("mailto:Dominik.Wyss1@ost.ch")[Dominik.Wyss1\@ost.ch])
+ Nathanael Fässler (#link("mailto:Nathanael.Faessler@ost.ch")[Nathanael.Faessler\@ost.ch])
+ Yoris Kucera (#link("mailto:Yoris.Kucera@ost.ch")[Yoris.Kucera\@ost.ch])

#v(2em)

=== Availabilities
// XO = Available online | XR = Available in Rapperswil | XS = Available in St. Gallen
// (XO)/(XR)/(XS) = Available but not optimal | - = Not available
// Minimum 5 slots during regular working hours (08h-12h, 13h-17h) required.
// 
#v(2em)

#figure(
  table(
    columns: (auto, auto, auto, auto, auto, auto),
    align: center,
    fill: (x, y) => if (y == 5 or y == 10 or y == 11) { luma(230) },
    [*Time slot*], [*Mon*], [*Tue*], [*Wed*], [*Thu*], [*Fri*],
    [08h00–09h00], [-], [(XR)], [-], [-], [(XO)],
    [09h00–10h00], [-], [(XR)], [-], [-], [(XO)],
    [10h00–11h00], [(XR)], [-], [-], [-], [(XO)],
    [11h00–12h00], [(XR)], [-], [-], [-], [(XO)],
    [12h00–13h00], [-], [-], [-], [-], [(XO)],
    [13h00–14h00], [XR], [-], [-], [-], [(XO)],
    [14h00–15h00], [XR], [-], [-], [-], [(XO)],
    [15h00–16h00], [(XR)], [XR], [-], [-], [(XO)],
    [16h00–17h00], [(XR)], [XR], [-], [-], [(XO)],
    [17h00–18h00], [-], [(XR)], [-], [-], [(XO)],
    [18h00–19h00], [-], [(XR)], [-], [-], [-],
  ),
  caption: [Team availability for review meetings],
  supplement: [Table]
)

#v(4em)
=== Vision

GlowCore is a singleplayer progression game focused on resource gathering and gradual
world expansion. At the start, the player can only access a small, limited area of the game
world, while the rest remains hidden and unreachable. At the center of the world is a
special entity that can be upgraded using collected materials. Each upgrade expands the
accessible area, revealing new parts of the world and enabling further progression. The
core gameplay loop is built around exploring newly unlocked areas, gathering resources,
and using them to continuously improve the central entity and push the boundaries of
the world outward.

As the world expands, additional gameplay systems deepen the experience. The player
can obtain and use different materials, interact with tools, craft items, and face enemies
within the expanding world. More advanced mechanics such as machines for processing resources, farming systems, and specialized equipment can be introduced over time to
increase complexity and strategic depth.

#v(1em)

=== Proposed Realisation

The project will be implemented using Unity as the primary game engine, using its
integrated systems for rendering, physics, input handling, animation, audio, and crossplatform deployment. Development will be done in C\# using Unity's component-based
architecture (GameObjects, MonoBehaviours, and ScriptableObjects) to structure gameplay logic and systems. Unity's built in physics engine and animation system (Animator)
will be used to realise core gameplay and user interaction. Visual assets will be rendered
using Unity's Universal Render Pipeline (URP). Individual systems will be tested for robustness using unit tests by method of Unity's own testing framework, based on NUnit.
Version control will be handled with Git. The project will be developed within the Unity
Editor.


