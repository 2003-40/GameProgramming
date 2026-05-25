# Game Design Document (GDD) - Mineral Odyssey

## 1. Executive Summary
* **Game Title:** Mineral Odyssey
* **Genre:** 2D Top-down Adventure / Management
* **Target Audience:** Casual gamers who enjoy progression loops (e.g., Stardew Valley, Motherload).
* **Scope:** A high-quality, polished Vertical Slice focusing on a stable core gameplay loop, developed over a strict 3-week sprint.

## 2. Design Intent & Player Experience
The core design philosophy of *Mineral Odyssey* focuses on a compelling **"Gather-Process-Trade-Upgrade"** loop. Instead of over-scoping with unstable combat systems, the game drives player engagement through **Risk vs. Reward** environmental mechanics:
* **Safe Zones (Layers 1-2):** Introduces the player to basic mining mechanics and resource gathering without external pressure, establishing a comfortable flow state.
* **Danger Zones (Layers 3+):** Introduces environmental hazards (e.g., explosive/hazard blocks) and rare high-value veins. Players must constantly make tactical decisions: *“Do I risk moving deeper for rare ores, or retreat to safe layers to secure my current inventory?”*

## 3. Technical Architecture & Justification)
To demonstrate sound programming decisions and maintainable code architecture, the technical stack is structured as follows:
* **Unity Tilemap System:** Utilized to achieve dynamic, destructible environments. Grid-based layouts allow clean collision detection and efficient runtime sprite/tile manipulation when mining.
* **ScriptableObjects for Data Decoupling:** Item metadata (ID, Name, Base Price, Icon Sprite) is completely separated from game logic. This ensures high scalability, allowing gameplay balance tuning (e.g., tweaking prices) without modifying C# scripts.
* **Singleton Pattern:** Implemented for global managers (e.g., `GameManager`, `InventoryManager`) to maintain persistent game states (Gold, Player Inventory, Level Status) safely across scenes.
* **Data Persistence (JSON / PlayerPrefs):** Ensures simple yet robust save/load states for gold and inventory progression, proving architectural stability during evaluations.

## 4. Professional Practice & Compliance
### 4.1 Legal & Asset Attribution
All external assets are systematically verified for educational and commercial compliance:
* **Art Assets:** Sourced from Kenney.nl (CC0 License) and verified Itch.io free packs.
* **Audio Assets:** Generated via Bfxr.
* *Compliance Action:* Full credits and asset licenses will be explicitly displayed in the game's Main Menu and documented in the repository.
### 4.2 Accessibility Considerations
* **Visual Accessibility (Colorblind Friendliness):** Different tiers and grades of ore will not rely on color alone; they will feature distinct geometric shapes and internal texture patterns.
* **Control Schemes:** Dual-input support allowing both `WASD` and `Arrow Keys` for comfortable single-handed or multi-handed navigation.

## 5. Scope Management & Risk Control
* **Scope Optimization:** Combat AI has been deliberately cut and replaced with "Hazard Blocks & Rare Veins" to guarantee a fully playable, stable, and glitch-free build under the 3-week constraint.
* **Stretch Goals:** Visual juice (screen shake, debris particles) and dynamic popup alerts are categorized under Phase 2 and will only be refined once the core economic data layer (Phase 1) is completely locked and stable.