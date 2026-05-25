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

## 📅 Development Roadmap & Project Progress

The project scope has been deliberately optimized to focus on a highly polished, mechanically sound **Vertical Slice**, prioritizing game feel and iterative development over sheer size.

### 🟩 Phase 1: Core Loop, Mining Mechanics & Economy (Current Week)
*Goal: Establish the absolute fundamental gameplay loop (Mine → Inventory → Refine → Sell).*

- [x] **Player Movement**: Basic grid-based/smooth character movement and control setup.
- [ ] **Tilemap & Mining Logic**: Base environment setup using tilemaps, connecting player interaction to tile/ore destruction and resource drops.
- [ ] **Data Layer & Inventory**: Implementation of a structured inventory system (`ItemData` structs) to handle asset collection properly.
- [ ] **Economy Systems (Forge & Shop)**: 
  - Resource processing logic (converting Raw Ore $\rightarrow$ Refined Ingot).
  - A consolidated NPC Merchant interface located within the main scene for buying and selling items, maximizing stability.

### 🟨 Phase 2: Game Feel & Progression Mechanics (Sprint Week 2)
*Goal: Address gameplay repetitiveness by adding sensory feedback, tool scaling, and dynamic risk/reward variables.*

- [ ] **Juice & Game Feel (Visual/Audio Feedback)**: Implementation of screen shake, particle effects (flying block debris), and hit animations upon mining.
- [ ] **Tool Tier Progression**: Configuration of modular tool levels where dynamic efficiency multipliers apply based on the ore metadata (e.g., Iron Pickaxe vs. Copper Ore).
- [ ] **Mitigating Repetition (Dynamic Spawning)**: Simple, lightweight algorithms to spawn rare high-value veins or explosive/hazard blocks instead of complex, bug-prone combat AI.
- [ ] **UI Responsiveness**: Implementation of progress bars for refining/forging and runtime popup alerts (e.g., Gold gained, Inventory Full notifications).

### 🟦 Phase 3: Quality Assurance, Polishing & Evaluation (Sprint Week 3)
*Goal: Strict stabilization, edge-case debugging, code refactoring, and preparing submission materials. No new features.*

- [ ] **Rigorous Playtesting & Debugging**: Systematic testing of border cases (e.g., overflow limits, boundary checks) and logging bug fixes for the final development report.
- [ ] **UI/UX Polish**: Refining menus, localizing HUD layouts, and smoothing out user interaction states.
- [ ] **Repository Optimization**: Cleaning up file architecture, ensuring proper asset licensing attribution, and optimizing build size.
- [ ] **Project Retrospective**: Finalizing documentation detailing architectural choices, solutions to development roadblocks, and design iterations based on user feedback.

---

## 🚀 Getting Started

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/2003-40/GameProgramming.git
    ```
2.  **Open in Unity:**
    Add the project to Unity Hub (recommended version 2022.3).
3.  **Run the game:**
    Open `MainScene.unity` in the `Assets/Scenes` folder and click Play.

---
## 📼 Screenshots
![FirstLevel](image.png)

---

## 🤝 Acknowledgements & Resources

*   Art Assets: [Kenney.nl](https://kenney.nl/) / [Itch.io Free Assets]
*   Sound Effects: [Bfxr]
*   Special Thanks: [Your Professor/TA's Name] for guidance

--- 

### 📝 Author
-   **Name:** Sihan Wang
-   **University:** Dundee International Institute of Central South University
-   **Contact:** 2617378@dundee.ac.uk

---

## Workflow Automation

- GitHub Kanban auto-management and commit message automation guide:
  - docs/github_automation.md

