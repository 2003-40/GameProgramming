# In-Class Discussion & Project Feedback Record

This document records the feedback received during the in-class peer review session for my indie game development project, along with the current development progress and an optimized roadmap for the upcoming sprints.

---

## 🎮 Project Overview & Current Progress

### Core Concept
A resource-management loop game centered around **mining and progression**. Players gather raw ores, refine them for sale, earn gold, and spend it on purchasing/upgrading weapons and expanding backpack capacity to unlock more challenging zones.

### Current Implementation Status
* [x] **Visual Foundation**: Basic in-mine background graphics assets are integrated.
* [x] **Character Controller**: Smooth player movement and basic physics/collision detection within the mining area are fully implemented.

---

## 👍 Positive Feedback

* **Clear Core Gameplay Loop**: Peers agreed that the foundational progression loop (Gathering $\rightarrow$ Refining $\rightarrow$ Selling $\rightarrow$ Upgrading $\rightarrow$ Re-gathering) is mathematically intuitive, highly engaging, and easy for players to grasp immediately.
* **Solid Feature Framework**: The proposed architecture separating the **Shop System**, **Mining/Gathering System**, and **Inventory (Backpack) System** forms a robust MVP (Minimum Viable Product) that ensures immediate rewarding feedback loops.

---

## 👎 Constructive Criticism (Negative Feedback)

* **Feedback Received**: 
    *"The core gameplay risks becoming repetitive and monotonous during the mid-to-late game phases."*
    Specifically, if players only mechanically cycle through "mine, refine, sell, upgrade," the sense of novelty will wear off quickly. Pure numerical scaling without dynamic gameplay elements may lead to player fatigue.
* **Design Countermeasures**:
    To enhance strategy and depth, the following mechanics will be researched and implemented:
    1.  **Dynamic Hazards/Random Events**: Introduce cave-ins, rare monster encounters, or hidden treasure pockets during excavation to break the monotony.
    2.  **Weapon Utility & Synergies**: Weapons will serve dual purposes—combat and specialized mining (e.g., specific tool types yield speed bonuses on certain ore structures).
    3.  **Interactive Refining Process**: Move away from a simple progress bar; introduce a lightweight synthesis or purity-matching mechanic to maximize sell value.

---

## 📝 Optimized Action Plan (Next Steps)

Based on our current progress (working graphics and movement) and the peer feedback received, the upcoming development phases have been restructured to prioritize core loop validation before adding heavy visual polish.

### Phase 1: MVP Loop & Mechanics Integration (Sprint 1-2)
* [ ] **Implement Base Systems**: Build the data structures and UI screens for the Inventory (Backpack) and the Merchant (Shop) systems.
* [ ] **Mining Mechanics**: Connect character positioning with tile/ore destruction logic to trigger item drops.
* [ ] **Refining Logic**: Program the basic item transformation function (Raw Ore $\rightarrow$ Refined Ingot).

### Phase 2: Game Feel & Feedback Polish (Sprint 3)
* [ ] **Juice & Game Feel**: Add screen shake, particle effects (flying debris), and hitting animations when the character mines an ore tile.
* [ ] **UI Responsiveness**: Implement smooth progress bars for refining and dynamic pop-ups for gold gained or inventory full alerts.

### Phase 3: Mitigating Repetition (Addressing Negative Feedback) (Sprint 4)
* [ ] **Randomized Spawning**: Implement a simple algorithm to spawn rare veins or minor hostile mobs within the mine background.
* [ ] **Tool Tier Progression**: Configure different tools to have unique efficiency multipliers based on the ore type metadata.