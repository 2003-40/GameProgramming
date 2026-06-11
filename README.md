# Mineral Odyssey

Mineral Odyssey is a 2D top-down pixel-art mining roguelite built in Unity. The project focuses on a short, repeatable loop where every mining action spends stamina, deeper levels add risk through lightweight monsters and hazards, and permanent upgrades expand what the player can access between runs.

---

## Core Gameplay Loop

1. **Map Hall:** The player starts in a hub screen with an S-shaped mine-cart route: Start -> Level 1 -> Level 2 -> Level 3. Each level node shows its unlock state, ticket cost, and risk/reward summary.
2. **Guide & Shop:** The hub UI includes a guide panel for current objectives and a trade/shop panel for buying weapons, upgrading tools, improving card rewards, and increasing sell-value bonuses.
3. **Run Setup:** Before entering a level, the game displays the current run modifier and offers a temporary card choice that only affects the current run.
4. **Mining & Combat:** Stamina is the run budget. Mining spends stamina based on ore hardness and depth, while monster hits also reduce stamina. Planning and movement do not spend stamina.
5. **Reward & Progression:** The run ends when stamina is depleted or the player exits. Earned gold is saved and spent on permanent upgrades, deeper level access, tools, weapons, and card-system upgrades.

---

## Design Pillars

- **Every swing is a budget decision:** Stamina limits each run without using a timer, so players can plan calmly.
- **Deeper levels are structured progression:** Level access is gated by tickets, tool capability, and upgrade milestones rather than raw stat inflation.
- **Cards change the current run:** Temporary cards create tactical variation, such as slower stamina loss, instant gold, bonus ore fragments, combat bonuses, or risky high-reward modifiers.
- **Shop upgrades support the loop:** Permanent upgrades unlock harder ores, deeper levels, better weapons, card-pool growth, rare-card odds, and passive refinement value bonuses.
- **Lightweight monsters, not complex combat:** Level 2 introduces one simple monster type. Level 3 introduces two monster types. They create stamina pressure and positioning decisions without turning the project into a full combat game.
- **No heavy crafting chain:** The forge/refining layer is replaced by a passive shop upgrade that increases sell value, keeping the scope focused and UI-driven.

---

## Tech Stack

- **Engine:** Unity 2022.3 LTS
- **Rendering:** Universal Render Pipeline (URP) 2D
- **Programming Language:** C#
- **Key Components / Technologies:**
  - **Tilemap:** Dynamic and destructible mining environments.
  - **ScriptableObjects:** Data-driven item, ore, tool, card, level, and upgrade metadata.
  - **Singleton / Manager Pattern:** Global state for gold, run state, stamina, shop progression, and UI updates.
  - **Lightweight Enemy Logic:** Simple patrol/chase/attack behaviors for layer-based monster pressure.
  - **PlayerPrefs / JSON:** Simple save/load support for gold and permanent progression.

---

## Development Roadmap & Project Progress

The project scope is now focused on a stable solo vertical slice rather than a complete roguelite system. The current implementation emphasizes the playable mining loop, stamina economy, map hall, shop upgrades, temporary run cards, and clear UI feedback.

### Phase 1: Core Loop, Mining Mechanics & Economy

*Goal: Establish the basic loop: mine -> collect -> earn gold.*

- [x] **Player Movement:** Basic grid-based/smooth character movement and control setup.
- [x] **Tilemap & Mining Logic:** Tilemap-based ore destruction and resource drops.
- [x] **Data Layer & Inventory:** Structured item data and inventory support.
- [x] **Economy Systems:**
  - Global gold manager tracks the player's current gold total.
  - Mined pickups read `ItemData.Value` and convert common rewards into gold on collection.
  - Mining UI displays current gold during play.
  - Reward logic remains modular for direct gold, inventory storage, future quests, or collection systems.

### Phase 2: Must-Have Vertical Slice

*Goal: Deliver a playable loop that can be demonstrated confidently.*

- [x] **Stamina Budget:** Right-corner stamina UI, stamina spending on mining, monster/hazard stamina damage, and run-end handling when stamina reaches zero.
- [x] **Static Map Hall:** Hub screen with Start/Level 1/Level 2/Level 3 nodes, ticket costs, level entry buttons, and a cart marker.
- [x] **Basic Shop:** UI-based shop for tool and weapon upgrades using saved gold.
- [x] **Tool & Ore Gating:** Tool level and ore hardness determine whether an ore can be mined and how much stamina it costs.
- [x] **Temporary Cards:** Run-only card choices include positive and negative effects such as stamina reduction, instant gold, double drops, stamina restoration, and curse-style tradeoffs.
- [x] **Minimal Monster Pressure:** Simple monster health and stamina contact damage support Level 2/Level 3 pressure.
- [x] **Essential UI Feedback:** Stamina, gold, ticket cost, shop purchase result, selected run card, and run-end gold summary are shown through UI.

### Optional Features After Must-Have

*Goal: Add only if the vertical slice is already stable.*

- [ ] **Guide Panel:** Show current objective and next recommended upgrade in the hub.
- [ ] **Second Level 3 Monster:** Add a clearly different second monster type for Level 3.
- [ ] **Rare Veins:** Add one rare high-value vein type with depth-scaled spawn chance.
- [ ] **Single Hazard Block:** Add one explosive or unstable block type.
- [ ] **Refinement Multiplier:** Add a passive shop upgrade that increases sell value.
- [ ] **Run Modifier:** Show one random modifier before card selection.

### Phase 3: Quality Assurance, Polishing & Evaluation

*Goal: Stabilize the vertical slice and prepare final submission materials.*

- [ ] **Playtesting & Balancing:** Validate stamina costs, ticket costs, card strength, ore value, monster pressure, and shallow-level anti-farming.
- [ ] **UI/UX Polish:** Improve hub navigation, card readability, shop clarity, and in-run feedback.
- [ ] **Repository Optimization:** Clean project structure, verify asset licensing, and optimize build size.
- [ ] **Project Retrospective:** Document design iterations, technical choices, testing evidence, and known limitations.

---

## Getting Started

1. **Clone the repository:**
   ```bash
   git clone https://github.com/2003-40/GameProgramming.git
   ```
2. **Open in Unity:**
   Add the project to Unity Hub using Unity 2022.3 LTS.
3. **Run the game:**
   Open the main scene in the Mineral Odyssey Unity project and click Play.

---

## Screenshots

![FirstLevel](image.png)

---

## Acknowledgements & Resources

Reference links are intentionally left as placeholders so final source URLs, licences, and credit notes can be filled in before submission.

| Category | Resource / Tool | Link | Licence / Permission | Used For | Notes |
| --- | --- | --- | --- | --- | --- |
| Art Asset | Pixel character / environment assets | TODO: add link | TODO | Player, mine background, tiles, UI art | TODO |
| Art Asset | Mine tile / ore asset pack | TODO: add link | TODO | Mine tilemap, ore tiles, decorations | TODO |
| UI Asset | UI panels / generated UI elements | TODO: add link or "self-created" | TODO | Map hall, shop, stamina, card UI | TODO |
| Audio | Pickaxe hit sound | TODO: add link | TODO | Mining hit feedback | TODO |
| Audio | Stone / ore destruction sound | TODO: add link | TODO | Ore break feedback | TODO |
| Audio | Additional UI or feedback sounds | TODO: add link | TODO | Button or reward feedback | TODO |
| Tutorial / Reference | Unity Tilemap documentation or tutorial | TODO: add link | TODO | Tilemap setup and mining scene workflow | TODO |
| Tutorial / Reference | Unity UI / TextMeshPro reference | TODO: add link | TODO | UI construction and polish | TODO |
| AI Coding Assistance | ChatGPT / Codex | TODO: add conversation or declaration note | AI assistance declared; final code reviewed and integrated by author | Coding suggestions, debugging ideas, documentation structure, comments | TODO |
| AI Design Assistance | ChatGPT / Codex | TODO: add conversation or declaration note | AI assistance declared; final decisions by author | Scope planning, feature prioritization, report/contribution outline | TODO |

See also:

- `docs/ContributionList.md`
- `docs/SubmissionChecklist.md`

---

## Author

- **Name:** Sihan Wang
- **University:** Dundee International Institute of Central South University
- **Contact:** 2617378@dundee.ac.uk

---

## Workflow Automation

- GitHub Kanban auto-management and commit message automation guide:
  - docs/github_automation.md
