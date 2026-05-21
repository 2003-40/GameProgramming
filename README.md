# Game Name：Mineral Odyssey (矿石奇遇记)
This is a 2D top-down pixel-art adventure and management game developed in Unity. This project aims to showcase core development skills such as dungeon generation, inventory systems, AI combat, and data persistence through a complete "gather-process-trade-upgrade" loop.

---

## 🎮 Core Gameplay Loop

1.  **Mining:** Enter a multi-layered mine, using tools to excavate different grades of ore (implemented using the Tilemap system).
2.  **Survival:** The first two layers are safe zones; starting from the third layer, players will encounter monsters with varying AI behaviors.
3.  **Processing:** Bring raw ores back to the resource house and transform them into refined products using a forge.
4.  **Trading:** Sell refined products at the shop to earn gold.
5.  **Progression:** Use gold to purchase larger inventory space or more powerful weapons to explore deeper mine layers.

---
## 🛠 Tech Stack

*   **Engine:** Unity 2022.3 LTS
*   **Rendering:** Universal Render Pipeline (URP) 2D
*   **Programming Language:** C#
*   **Key Components / Technologies:**
    *   **Tilemap:** For dynamic and destructible map environments.
    *   **ScriptableObjects:** To decouple item data (e.g., name, price, sprite) from game logic.
    *   **Singleton Pattern:** For managing global game states (e.g., player assets, level status).
    *   **A\* Pathfinding:** Basic AI navigation for enemies.
    *   **JSON / PlayerPrefs:** For simple save/load functionality.

---

## 📅 Development Roadmap

This project is planned to be completed within 4 weeks:

- [ ] **Phase 1: Core Mechanics Prototype (Week 1)**
    - Player movement and Tilemap destruction logic
    - Basic inventory system (data layer)
- [ ] **Phase 2: System Logic & Core Loop (Week 2)**
    - Resource processing logic (forge)
    - Shop system for buying and selling
    - Scene transitions and data persistence across scenes
- [ ] **Phase 3: Combat & AI (Week 3)**
    - Enemy spawning algorithms
    - Player attack and damage detection system
    - Mine layer difficulty balancing
- [ ] **Phase 4: Polishing & Testing (Week 4)**
    - UI/UX interface optimization
    - Sound effects and visual particle effects
    - Bug fixing and performance optimization

---

## 🚀 Getting Started

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/2003-40/GameProgramming.git
    ```
2.  **Open in Unity:**
    Add the project to Unity Hub (recommended version 2022.3.x).
3.  **Run the game:**
    Open `MainScene.unity` in the `Assets/Scenes` folder and click Play.

---

## 🖼 Screenshots

> [Coming Soon...]

---

## 🤝 Acknowledgements & Resources

*   Art Assets: [Kenney.nl](https://kenney.nl/) / [Itch.io Free Assets]
*   Sound Effects: [Bfxr]
*   Special Thanks: [Your Professor/TA's Name] for guidance

--- 

### 📝 Author
-   **Name:** Sihan Wang
-   **University:** Dundee International Institute of Central South University
-   **Contact:** 2617378@mydundee.ac.uk

---

## Workflow Automation

- GitHub Kanban auto-management and commit message automation guide:
  - docs/github_automation.md

