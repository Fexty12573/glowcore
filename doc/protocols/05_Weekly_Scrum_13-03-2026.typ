#import "../lib.typ": agenda_table, meeting_info, todo_table

=== Weekly Scrum / 13.03.2026
#meeting_info(
  date: "13.03.2026",
  sprint: "Sprint 2",
  lead: "Dominik Wyss",
  scribe: "Yoris Kucera",
  time: "14:30-15:30",
  location: "Online",
  participants: "Cedric Cathomas, Nathanael Fässler, Dominik Wyss, Yoris Kucera",
  excused: "-",
  links: "-",
)

==== Agenda
#agenda_table((
  [Time Tracking],
  [Dominik Wyss],
  [
    - New time tracking report system integrated in Jira (Worklogs Report)
    - Task does not need to be assigned for time to be tracked
  ],
  [Documentation Build Pipeline],
  [Yoris Kucera],
  [
    - Pipeline for building documentation on GitHub Actions is set up and working
    - Automatically deployed to pages on push to `dev` and `main` branches.
  ],
  [Domain Model],
  [Yoris Kucera],
  [
    - First version of domain model created
    - State machine for enemy created
  ],
  [Functional Requirements],
  [Nathanael Fässler],
  [
    - Documented Functional Requirements, Actors and Personas
    - Documented Use Cases
    - Use Cases formatting could be improved
  ],
  [Player and Camera],
  [Nathanael Fässler],
  [
    - Player controller implemented and working, controls with WASD
    - Camera implemented and working, controls with mouse movement
    - Camera follows player and rotates around it, fixed height and distance
  ],
  [Non-Functional Requirements],
  [Cedric Cathomas],
  [
    - Documented Non-Functional Requirements
    - Priority Matrix for NFRs
    - NFRs grouped by category and numbered (e.g. NFR101, NFR102, NFR403, etc.)
  ],
  [GitHub CI/CD Failing],
  [Dominik Wyss/Yoris Kucera],
  [
    - Pipeline fails because of lack of disk space on the runner
    - Possible solutions:
      - Clean up even more storage
      - Make repository public
      - Use self-hosted runner with more storage
      - Rent server at OST
  ],
  [UI Sketches],
  [Dominik Wyss/Cedric Cathomas],
  [
    - Sketches for Main Menu
      - Scene from the game shown in the background
      - Buttons for New Game, Continue, Save Files, Settings, Exit
    - Sketch for New Game menu
      - Give name to world
    - Sketch for Save File Menu
      - List of save files with name and date
      - Button to delete save file
      - Show glowcore level?
      - Separate list for auto saves and manual saves?
    - Sketch for Settings Menu
      - Sliders for music and sound effects volume
      - Slider for mouse sensitivity?
      - Graphics options?
    - Sketch for Pause Menu
  ],
))

==== Todos
#todo_table((
  [Fix WebGL Build Pipeline],
  [Yoris Kucera],
  [20.03.2026],
  [Unify NFRs, FRs and Domain Model],
  [Yoris Kucera],
  [20.03.2026],
  [Adjust README on GitLab],
  [Dominik Wyss],
  [20.03.2026],
  [Agents.md],
  [Cedric Cathomas],
  [20.03.2026],
  [Explicitly address review feedback in Teams],
  [Dominik Wyss],
  [20.03.2026],
))

