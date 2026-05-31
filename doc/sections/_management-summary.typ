
#import "../lib.typ": part_page, title_without_numbering

#part_page("I", "Management Summary")

#v(10em)

#title_without_numbering(title: "Management Summary")

#v(2em)

#title_without_numbering(title: "Starting Point", depth: 3)

GlowCore started as the idea of building a small singleplayer game where the player begins in a tiny lit area surrounded by darkness and has to expand the world by upgrading a central object called the GlowCore. The challenge for us as a team of four students had two sides. We had to design a game that is actually fun to play, and at the same time we had to build it cleanly enough that the code survives growing scope, four people working in parallel, and the usual deadline pressure of a Semester 4 project.

The other half of the goal was applying the software engineering process we learned about in lectures to a real codebase that we cared about. Scrum, CI/CD, automated testing, code reviews, branching, all of it had to work in practice and not just on paper.

#v(1em)

#title_without_numbering(title: "Our Approach", depth: 3)

We picked Unity 6 with the Universal Render Pipeline as our engine. C\# was a language everyone already knew and we used the time before the official project start to get up to speed on Unity, so we could be productive from day one. The code was split into two main layers, gameplay logic and presentation, with clear interfaces between them. ScriptableObjects are used for data like items, recipes and node definitions, which makes it possible to add new content without touching any code.

Work was organised in two-week sprints with a weekly Scrum meeting and a regular review with our advisor. Every change went through a pull request, was reviewed by a teammate, and had to pass our CI pipeline that runs linting, formatting checks and automated tests. Roles were split between the four of us so that everyone had a main area of responsibility but could still help out anywhere in the codebase.

One of the more important decisions we made during the project came after the first few sprints. The original plan was ambitious and included a combat system, multiple automation phases and additional biomes. After looking at our progress in April we decided to drop combat and the later automation phases from the Beta scope on purpose. The reasoning was simple, a smaller game that feels polished beats a bigger game that feels half finished. We documented the updated milestones and went through them with our advisor. Once the core of the game was solid, we picked automation back up and shipped it in the final sprint.

For the game itself we designed a core loop around gathering wood, crafting tools, upgrading the GlowCore and slowly automating the work with the axe machine. The intention was to give the player a steady sense of progress and to make automation feel like a reward rather than something the game hands out from the start.

#v(1em)

#title_without_numbering(title: "Result", depth: 3)

GlowCore covers the full MVP we defined at the start of the project. The player can install the game, save and load progress, break and place nodes, expand the map by feeding the GlowCore, store items in chests, craft items at the workbench and the furnace, and change settings to their taste. All of this runs on Windows, Linux and in the browser through WebGL, with the WebGL build deployed automatically to GitHub Pages on every push to main or dev.

The numbers back this up. The game runs at around 120 FPS on WebGL during active play, well above the 60 FPS target we set ourselves. Worlds load in roughly 1.5 seconds against a 3 second budget, and save files come in at a few kilobytes despite a 5 MB ceiling. We ran two external user tests with players outside the team. Both testers reached the GlowCore upgrade loop on their own within a few minutes and confirmed that the core loop is fun to play. Their feedback on the parts that confused them or needed polish went straight into the next sprint.

On the engineering side the codebase stays consistent thanks to the automated checks, and the architecture proved its value when we added additional crafting stations late in the project without having to rewrite existing systems. The hours we tracked across the team are running close to the 480 hour budget we planned at the start, which tells us that our storypoint estimates and sprint planning have been reasonably accurate.

#v(1em)

#title_without_numbering(title: "Looking Forward", depth: 3)

GlowCore is in a good state to keep growing. The architecture was built with extensibility in mind, so adding new items, nodes, recipes or even entire crafting stations comes down to creating a new ScriptableObject asset and wiring it up in the inspector. Combat and potential future game systems such as farming are documented in the design and could be picked up either as a follow up project or as a continuation outside the semester. The foundation, the deployment pipelines, the testing setup and the documentation are all in place to support that, regardless of who continues the work.

