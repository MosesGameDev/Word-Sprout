# wordPuzzleGame


[![Watch the video](https://img.youtube.com/vi/mFrQAlS25lE/hqdefault.jpg)](https://youtube.com/shorts/mFrQAlS25lE)


## Project Structure

The repository is organized to promote clarity and maintainability:

```
wordPuzzleGame/
├── assets/            # Images, sounds, and other static assets
├── docs/              # Documentation and design notes
├── src/               # Main source code for game logic and UI
│   ├── core/          # Core gameplay systems (puzzles, scoring, state)
│   ├── ui/            # User interface components (menus, screens)
│   ├── utils/         # Utility scripts and helpers
│   └── main.py        # Application entry point
├── tests/             # Unit and integration tests
├── requirements.txt   # Python dependencies
└── README.md          # Project overview and instructions
```

## Code Pattern

The codebase adopts **modular design** with clear separation of concerns:
- **Core systems** (game state, puzzle generation, scoring) are isolated from UI code.
- **Component-based UI**: Each screen and menu is in its own module under `src/ui`.
- **Helper utilities** (string manipulation, validation, etc.) reside in `src/utils`.
- **Tests** follow the Arrange-Act-Assert pattern and cover critical gameplay scenarios.

Major patterns used:
- **Object-Oriented Programming (OOP)**: Classes for game logic state machines, puzzle objects, and player sessions.
- **Event-driven**: UI elements communicate via event emitters and listeners for loose coupling.
- **Configuration-driven**: Game rules and settings are externalized for easy adjustment.

## Gameplay Systems

`wordPuzzleGame` is a themed word puzzle game where players must solve different types of word challenges under time or turn constraints.

**Core Gameplay:**
- **Puzzle Generator**: Dynamically creates puzzles based on selected difficulty or theme; types may include word scrambles, crossword snippets, or fill-in-the-blanks.
- **Game State Manager**: Tracks player progress, time limits, score history, and manages transitions between puzzles and levels.
- **Scoring System**: Awards points based on accuracy, speed, and puzzle complexity.
- **Hint System**: Offers lifelines (hints) at a configurable cost.
- **UI System**: Presents puzzles, timers, and feedback, and accepts player input.

**Flow:**
1. **Player Start:** Upon launch, the player selects game mode or difficulty.
2. **Puzzle Round:** The system generates a puzzle and displays it; the player attempts a solution.
3. **Feedback:** The game provides immediate feedback and updates score/timers accordingly.
4. **Progression:** After each puzzle, the player advances or receives a summary before continuing.

**Extensibility:**
- Easy to add new puzzle types, hint algorithms, or UI screens.
- All major systems are documented within the `/docs` folder.

---

