# Mineral Odyssey

Mineral Odyssey is a playable 2D top-down pixel-art mining roguelite vertical slice built in Unity. The project focuses on a short, repeatable loop where every mining action spends stamina, deeper levels add risk through lightweight monsters and hazards, and permanent upgrades expand what the player can access between runs.

---

## Core Gameplay Loop

1. **Menu and Map Hall:** The player starts from the menu, then enters a hub screen with an S-shaped mine-cart route: Start -> Level 1 -> Level 2 -> Level 3. Each level node shows its ticket cost and supports level entry.
2. **Shop:** The hub UI includes a shop panel for spending saved gold on tool and weapon upgrades.
3. **Run Cards:** During a mining run, active mining progress unlocks a temporary card offer. The selected card only affects the current run.
4. **Mining and Combat:** Stamina is the run budget. Mining spends stamina based on ore hardness and depth, while monster hits also reduce stamina. Planning and movement are mostly calm, with only light depth-pressure exceptions in deeper levels.
5. **Reward and Progression:** The run ends when stamina is depleted or the player exits. Earned gold is saved and spent on level tickets plus permanent tool and weapon upgrades.

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
  - **Lightweight Enemy Logic:** Simple patrol/chase/contact behaviors for layer-based monster pressure.
  - **PlayerPrefs:** Simple save/load support for gold and permanent tool/weapon progression.

---

## Implemented Vertical Slice

The project scope is focused on a stable solo vertical slice rather than a complete roguelite system. The current implementation emphasizes the playable mining loop, stamina economy, map hall, shop upgrades, temporary run cards, and clear UI feedback.

### Completed Core Features

- [x] **Player movement:** Basic top-down movement and animation setup.
- [x] **Tilemap mining:** Tilemap-based ore hit detection, ore damage, destruction, and rewards.
- [x] **Item and economy data:** ScriptableObject item data, pickup rewards, and saved gold.
- [x] **Stamina budget:** Stamina UI, mining stamina cost, monster/hazard stamina damage, and run-end handling when stamina reaches zero.
- [x] **Static Map Hall:** Hub screen with Start/Level 1/Level 2/Level 3 nodes, ticket costs, level entry buttons, and a cart marker.
- [x] **Basic Shop:** UI shop for permanent tool and weapon upgrades using saved gold.
- [x] **Tool and ore gating:** Tool level and ore hardness determine whether an ore can be mined and how much stamina it costs.
- [x] **Temporary cards:** Run-only card choices include positive and negative effects such as stamina reduction, instant gold, double drops, stamina restoration, and curse-style tradeoffs.
- [x] **Minimal monster pressure:** Simple monster health, movement, attacks, and stamina contact damage support Level 2/Level 3 pressure.
- [x] **Rare veins:** Higher-value ore/gem rewards are present through the ore data and deeper level layouts.
- [x] **Single hazard block:** Explosive / unstable block behavior is supported through the mining controller with credited Pixabay explosion audio.
- [x] **Essential UI feedback:** Stamina, gold, ticket cost, shop purchase result, selected run card, and run-end gold summary are shown through UI.

### Optional / Cut Features

These features are not complete systems in the current vertical slice and should not be described as implemented:

- [ ] **Full guide panel:** A small first-level tutorial guide exists, but a full hub objective/upgrade guide system is not implemented.
- [ ] **Second distinct Level 3 monster type:** Level 3 reuses lightweight monster pressure rather than a fully distinct enemy system.
- [ ] **Refinement multiplier:** Passive refinement/sell-value upgrades are out of scope.
- [ ] **Run modifier before card selection:** The current card offer appears during the run after activity, not before level entry.
- [ ] **Full forge/crafting chain:** Cut from the vertical slice.
- [ ] **Complex enemy AI or A* pathfinding:** Cut from the vertical slice.
- [ ] **Card upgrade / card pool upgrade system:** Cut from the vertical slice.

---

## Getting Started

1. **Clone the repository:**
   ```bash
   git clone https://github.com/2003-40/GameProgramming.git
   ```
2. **Open in Unity:**
   Add `project/Mineral Odyssey` to Unity Hub using Unity 2022.3.62f3c1 or another compatible Unity 2022.3 LTS editor.
3. **Run in the Unity Editor:**
   Open `Assets/Scenes/_Menu.unity` and click Play. The enabled build scenes are `_Menu`, `MapHall`, `FirstFlour`, `SecondFlour`, and `ThirdFlour`.

### Controls

- Move with WASD / arrow-key style input through Unity input axes.
- Aim mining or weapon direction with the mouse.
- Use the configured attack/mining input to swing the current tool.
- Use UI buttons for map level selection, shop upgrades, card choices, ending a run, and returning to the map hall.

### Windows Build

The current Windows build folder is present locally at:

```text
project/Mineral Odyssey/2617378_CW2_FinalGameBuild/
```

The executable is:

```text
project/Mineral Odyssey/2617378_CW2_FinalGameBuild/Mineral Odyssey.exe
```

To run the Windows build, keep `Mineral Odyssey.exe`, `UnityPlayer.dll`, `UnityCrashHandler64.exe`, `Mineral Odyssey_Data/`, and `MonoBleedingEdge/` together in the same folder, then launch `Mineral Odyssey.exe`.

For coursework submission, zip the playable build folder as `2617378_CW2_FinalGameBuild.zip` or the exact filename requested by the submission form. Do not include Unity editor/generated folders such as `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/`, build cache folders, or `Mineral Odyssey_BurstDebugInformation_DoNotShip/`.

At the time of this documentation pass, a separate final `.zip` file has not been committed to the repository and should be created after the final hands-on build test. The build executable files are ignored by Git, so do not rely on the GitHub repository alone as the submitted build package.

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
- Generated UI sprites: `project/Mineral Odyssey/Assets/GameArts/UI/Generated/`
- Sound effects and music: `project/Mineral Odyssey/Assets/Sounds/`

If a replacement uses a new filename, import it into Unity and manually reassign the Sprite, Texture, Tile, prefab, AudioClip, or UI Image reference in the Inspector. For sprite sheets, also check Sprite Mode, Pixels Per Unit, slicing, and animation bindings after replacement.

---

## Known Issues and Limitations

- A final hands-on Play Mode and external build test is still required before submission.
- Monster AI is intentionally simple and exists mainly as stamina pressure.
- Card UI is functional but minimal.
- The vertical slice uses a small card pool and only three mine levels.
- The project does not include a full crafting/refining economy, complex enemy AI, card upgrade system, or full guide/objective system.
- The background music source should be described from the download/source notes honestly if the original page cannot be reopened before submission.

---

## Acknowledgements and Resources

The following third-party resources, tools, references, and assistance were used or considered during development. Assets that were only used during early prototyping or imported but not currently referenced are marked in the notes.

| Category | Resource / Tool | Link | Licence / Permission | Used For | Notes |
| --- | --- | --- | --- | --- | --- |
| Font | m5x7 by Daniel Linssen / Managore | https://managore.itch.io/m5x7 | Creative Commons Zero v1.0 Universal (CC0); attribution appreciated | Project font / TextMeshPro font asset named `Fun` | Used for pixel-style UI text. |
| Character Art | Small 8-direction Characters by AxulArt | https://axulart.itch.io/small-8-direction-characters | Creative Commons Attribution 4.0 International (CC BY 4.0); credit required | Player character sprites and animation source | Credited as AxulArt. |
| Environment / Ore Art | Petixel Mines Asset Pack - Biome Tileset by Ardonie | https://ardonie.itch.io/petixel-mines | Custom itch.io asset terms: commercial and non-commercial use allowed, editing allowed, redistribution/resale prohibited, AI training/NFT/crypto use prohibited; credit appreciated | Mine tiles, ore/crystal visuals, TNT/explosive visuals, borders, cave decorations, bats | Main source for mine environment and mining visuals. Credited as Ardonie. |
| Prototype Art / Tool Asset | Items pack (x16) by Glionox | https://glionox.itch.io/items16 | Custom itch.io asset terms: free and commercial use allowed, modification allowed, credit appreciated, redistribution/resale prohibited | Early prototype item/tool visuals | Used during early development, then removed from the final game assets. Credited for transparency. |
| UI Asset | Generated UI elements | Self-created for this project | Original project work | Map hall, shop, stamina, card UI, menu UI, tool/weapon icons, card frames, HUD, and run-end panel | Created and integrated by the project author. |
| Audio | "Stone dropping" by alegemaate / Allan Legemaate | https://freesound.org/s/364711/ | Creative Commons 0 (CC0) | Stone drop / ore destruction feedback | Referenced by the player prefab as the ore destroy clip. |
| Audio | "stone.flac" by Hedmarking | https://freesound.org/s/191887/ | Creative Commons 0 (CC0) | Stone / ore hit feedback | Referenced by the player prefab as the mining hit clip. |
| Audio | "Explosion" by FlashTrauma (Freesound), uploaded through freesound_community | https://pixabay.com/sound-effects/film-special-effects-explosion-6055/ | Pixabay Content License; free use allowed, attribution not required but appreciated, modification allowed, standalone redistribution prohibited | Explosive / unstable block feedback | Imported as `Assets/Sounds/freesound_community-explosion-6055.mp3` and referenced by the player prefab as `explosionClip`. |
| Background Music | `RPG游戏背景音乐合集（持续更新）` by 萌珑 / Menglong | NetEase Cloud Music album page, published 2019-06-21 | Author statement allows use of all music in the collection for people making RPG games, as long as the author is credited | Scene background music: `bgm002` for Menu, `bgm003` for Level 1, `BGM1-2` for Map Hall, and `BGM1-3` for mining scenes through the player prefab | Credited as music by 萌珑 / Menglong. Permission evidence is saved in `docs/bgm_permission_netease.png`. |
| Tutorial / Reference | Unity Tilemap documentation | https://docs.unity3d.com/2022.3/Documentation/Manual/Tilemap.html | Unity documentation reference | Tilemap setup and mining scene workflow | Used as technical reference, not copied as game content. |
| Tutorial / Reference | Unity UI / uGUI documentation | https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/index.html | Unity package documentation reference | Canvas, Button, Image, layout, and event-system UI construction | Used as technical reference, not copied as game content. |
| Tutorial / Reference | TextMeshPro documentation | https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html | Unity package documentation reference | Text rendering and UI labels | Used as technical reference, not copied as game content. |
| AI Coding Assistance | ChatGPT / Codex | Declared in this README and supporting documentation | AI assistance declared; final code reviewed, edited, tested, and integrated by author | Coding suggestions, debugging ideas, documentation structure, comments | Used as support, not as a replacement for author understanding or final review. |
| AI Design Assistance | ChatGPT / Codex | Declared in this README and supporting documentation | AI assistance declared; final design decisions by author | Scope planning, feature prioritization, report/contribution outline | Used to refine planning and wording; final project scope and implementation decisions remained author-controlled. |

See also:

- `docs/ContributionList.md`
- `docs/SubmissionChecklist.md`
- `docs/FinalGameplayTesting.md`

---

## Author

- **Name:** Sihan Wang
- **University:** Dundee International Institute of Central South University
- **Contact:** 2617378@dundee.ac.uk

---
