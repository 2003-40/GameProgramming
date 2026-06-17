# Mineral Odyssey

Mineral Odyssey is a 2D top-down pixel-art mining roguelite built in Unity. The project focuses on a short, repeatable loop where every mining action spends stamina, deeper levels add risk through lightweight monsters and hazards, and permanent upgrades expand what the player can access between runs.

---

## Core Gameplay Loop

1. **Menu & Map Hall:** The player starts from the menu, then enters a hub screen with an S-shaped mine-cart route: Start -> Level 1 -> Level 2 -> Level 3. Each level node shows its ticket cost and supports level entry.
2. **Shop:** The hub UI includes a shop panel for spending saved gold on tool and weapon upgrades.
3. **Run Cards:** During a mining run, active mining progress unlocks a temporary card offer. The selected card only affects the current run.
4. **Mining & Combat:** Stamina is the run budget. Mining spends stamina based on ore hardness and depth, while monster hits also reduce stamina. Planning and movement are mostly calm, with only light depth-pressure exceptions in deeper levels.
5. **Reward & Progression:** The run ends when stamina is depleted or the player exits. Earned gold is saved and spent on level tickets plus permanent tool and weapon upgrades.

---

## Design Pillars

- **Every swing is a budget decision:** Stamina limits each run without using a timer, so players can plan calmly.
- **Deeper levels are structured progression:** Level entry uses gold ticket costs, while deeper ore access depends on tool capability rather than raw stat inflation.
- **Cards change the current run:** Temporary cards create tactical variation, such as slower stamina loss, instant gold, bonus ore fragments, stamina restoration, or curse-style tradeoffs.
- **Shop upgrades support the loop:** Permanent tool upgrades unlock harder ores and improve mining efficiency, while weapon upgrades make monster pressure more manageable.
- **Lightweight monsters, not complex combat:** Level 2 and Level 3 use simple monster pressure to create stamina and positioning decisions without turning the project into a full combat game.
- **Subtle depth pressure:** Level 2 slightly slows movement, and Level 3 lightly drains stamina after the player stands still for a short time. These are tuned as small feel changes rather than major punishment.
- **No heavy crafting chain:** The forge/refining layer is intentionally out of scope for the current vertical slice, keeping progression focused on gold, tools, weapons, cards, and level access.

---

## Tech Stack

- **Engine:** Unity 2022.3.62f3c1 (Unity 2022.3 LTS)
- **Rendering:** Universal Render Pipeline (URP) 2D
- **Programming Language:** C#
- **Key Components / Technologies:**
  - **Tilemap:** Dynamic and destructible mining environments.
  - **ScriptableObjects / Tile Assets:** Data-driven item, ore tile, tool, and run-card metadata.
  - **Singleton / Manager Pattern:** Global state for gold, stamina, card state, shop progression, and UI updates.
  - **Lightweight Enemy Logic:** Simple patrol/chase/attack behaviors for layer-based monster pressure.
  - **PlayerPrefs:** Simple save/load support for gold and permanent tool/weapon progression.

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
- [x] **Rare Veins:** Add one rare high-value vein type with depth-scaled spawn chance.
- [x] **Single Hazard Block:** Add one explosive or unstable block type.
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
   Add `project/Mineral Odyssey` to Unity Hub using Unity 2022.3.62f3c1 or another compatible Unity 2022.3 LTS editor.
3. **Run the game:**
   Open `Assets/Scenes/_Menu.unity` and click Play. The enabled build scenes are `_Menu`, `MapHall`, `FirstFlour`, `SecondFlour`, and `ThirdFlour`.

### Windows Build

The Windows build is stored in the repository at:

```text
project/Mineral Odyssey/2617378_CW2_FinalGameBuild/Mineral Odyssey.exe
```

For coursework submission, this same build can also be zipped and uploaded separately to MyDundee.

---

## Screenshots

![FirstLevel](first_level.png)

The README screenshot is stored at the repository root as `first_level.png`. To update this screenshot, replace `first_level.png` with a new PNG using the same filename, or add the new image elsewhere and update the Markdown path above.

---

## Replacing Images

For Unity game images, keep the original `.meta` file when replacing an existing asset. Unity references assets through the GUID in the `.meta` file, so replacing the PNG while preserving its `.meta` keeps existing scene, prefab, tile, and UI references intact.

Common image locations:

- README screenshot: `first_level.png`
- Menu background: `project/Mineral Odyssey/Assets/Images/menu_background.png`
- Mine background: `project/Mineral Odyssey/Assets/Images/mine_background.png`
- Player sprite sheet: `project/Mineral Odyssey/Assets/Images/Small-8-Direction-Characters_by_AxulArt.png`
- Mine tiles and ore sprites: `project/Mineral Odyssey/Assets/GameArts/Environments/Mine/`

If a replacement uses a new filename, import it into Unity and manually reassign the Sprite, Texture, Tile, prefab, or UI Image reference in the Inspector. For sprite sheets, also check Sprite Mode, Pixels Per Unit, slicing, and animation bindings after replacement.

---

## Acknowledgements & Resources

The following third-party resources, tools, references, and assistance were used or considered during development. Assets that were only used during early prototyping are marked separately from assets included in the final Unity project.

| Category | Resource / Tool | Link | Licence / Permission | Used For | Notes |
| --- | --- | --- | --- | --- | --- |
| Font | m5x7 by Daniel Linssen / Managore | https://managore.itch.io/m5x7 | Creative Commons Zero v1.0 Universal (CC0); attribution appreciated | Project font / TextMeshPro font asset named `Fun` | Used for pixel-style UI text. |
| Character Art | Small 8-direction Characters by AxulArt | https://axulart.itch.io/small-8-direction-characters | Creative Commons Attribution 4.0 International (CC BY 4.0); credit required | Player character sprites and animation source | Credited as AxulArt. |
| Environment / Ore Art | Petixel Mines Asset Pack - Biome Tileset by Ardonie | https://ardonie.itch.io/petixel-mines | Custom itch.io asset terms: commercial and non-commercial use allowed, editing allowed, redistribution/resale prohibited, AI training/NFT/crypto use prohibited; credit appreciated | Mine tiles, ore/crystal visuals, TNT/explosive visuals, borders, cave decorations, bats | Main source for mine environment and mining visuals. Credited as Ardonie. |
| Prototype Art / Tool Asset | Items pack (x16) by Glionox | https://glionox.itch.io/items16 | Custom itch.io asset terms: free and commercial use allowed, modification allowed, credit appreciated, redistribution/resale prohibited | Early prototype item/tool visuals | Used during early development, then removed from the final game assets. Credited for transparency. |
| UI Asset | Generated UI elements | Self-created for this project | Original project work | Map hall, shop, stamina, card UI, and menu UI | Created and integrated by the project author. |
| Audio | "Stone dropping" by alegemaate / Allan Legemaate | https://freesound.org/s/364711/ | Creative Commons 0 (CC0) | Stone drop / ore feedback sound | Source information also recorded from TaoSound. |
| Audio | "stone.flac" by Hedmarking | https://freesound.org/s/191887/ | Creative Commons 0 (CC0) | Stone / ore sound effect | Source information also recorded from TaoSound. |
| Audio | "Pick striking stone sound" by JJDG | https://freesound.org/s/441787/ | Creative Commons Attribution NonCommercial 3.0 (CC BY-NC 3.0); credit required; non-commercial use only | Pickaxe hitting stone / mining hit feedback | Used only for this non-commercial coursework project. |
| Background Music | RPG game background music collection by 萌珑 | NetEase Cloud Music album: `RPG游戏背景音乐合集（持续更新）`, published 2019-06-21 | Author statement allows use of all music in the collection for RPG game creation as long as the author is credited | Scene background music: `bgm003` for Level 1, `bgm002` for Menu, `BGM1-3` for Level 2 and Level 3, `BGM1-2` for Map Hall | Credited as music by 萌珑. Level 2 and Level 3 use the same track. |
| Tutorial / Reference | Unity Tilemap documentation | https://docs.unity3d.com/2022.3/Documentation/Manual/Tilemap.html | Unity documentation reference | Tilemap setup and mining scene workflow | Used as technical reference, not copied as game content. |
| Tutorial / Reference | Unity UI and TextMeshPro documentation | https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html | Unity package documentation reference | UI construction, text rendering, and polish | Used as technical reference, not copied as game content. |
| AI Coding Assistance | ChatGPT / Codex | Declared in this README and supporting documentation | AI assistance declared; final code reviewed, edited, tested, and integrated by author | Coding suggestions, debugging ideas, documentation structure, comments | Used as support, not as a replacement for author understanding or final review. |
| AI Design Assistance | ChatGPT / Codex | Declared in this README and supporting documentation | AI assistance declared; final design decisions by author | Scope planning, feature prioritization, report/contribution outline | Used to refine planning and wording; final project scope and implementation decisions remained author-controlled. |

See also:

- `docs/ContributionList.md`
- `docs/SubmissionChecklist.md`

---

## Author

- **Name:** Sihan Wang
- **University:** Dundee International Institute of Central South University
- **Contact:** 2617378@dundee.ac.uk

---
