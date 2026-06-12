# Mineral Odyssey Submission Checklist

This document reorganizes the requirements from `docs/Submission.md` into a project-specific checklist for the current solo-scope version of Mineral Odyssey.

## 1. Game Concept and Design Document PDF

Submit one Game Concept and Design Document as a PDF.

Recommended source document:

- `docs/GameDesignDocument.md`

The PDF should cover:

- [ ] **Game title:** Mineral Odyssey
- [ ] **One-sentence idea:** A short mining roguelite where the player spends limited stamina to mine, survive deeper levels, earn gold, and buy permanent upgrades.
- [ ] **Intended player experience:** Calm planning, short runs, risk/reward mining, simple monster pressure, and visible progression.
- [ ] **Core mechanic:** Stamina is the run budget; mining and monster hits consume stamina.
- [ ] **Moment-to-moment play:** Choose a level, pick a temporary card, mine valuable ores, avoid/fight monsters, collect gold, return to the hub, upgrade.
- [ ] **Target player:** Casual players who enjoy resource planning, progression loops, and short replayable sessions.
- [ ] **Reference games / inspirations:** Motherload, Stardew Valley mining, light roguelite card-choice systems.
- [ ] **Original / creative element:** Combines action-based stamina budgeting, mining value decisions, simple card effects, and a mine-cart level hub.
- [ ] **Vertical slice plan:** Level 1 mining, Level 2 monster pressure, Level 3 higher risk/reward, shop upgrades, stamina UI, temporary cards.
- [ ] **Must-have / should-have / could-have / cut-first features:** See section 2 of this document.
- [ ] **Unity development plan:** Define systems, implement stamina, add hub, add shop, add cards, add simple monsters, test and polish.
- [ ] **Main systems/scripts:** Gold, stamina, mining, tool/ore data, card selection, shop, level entry, monster behavior, UI feedback.
- [ ] **Asset/resource plan:** Existing pixel art, tilemap assets, ore sprites, simple UI panels, credited sound effects.
- [ ] **Legal / ethical / accessibility / security:** Asset credits, colorblind-friendly ore distinction, no unnecessary personal data, simple controls.
- [ ] **Development schedule:** Two-week solo must-have schedule with polish and report time protected.

## 2. Feature Scope for Solo Delivery

### Must-Have

These are required for a stable playable vertical slice.

- [ ] **Stamina budget:** Right-corner stamina UI, mining stamina cost, monster stamina damage, run end at zero stamina.
- [ ] **Tool and ore gating:** Tool level controls whether harder ores can be mined and how much stamina mining costs.
- [ ] **Static map hall:** Start, Level 1, Level 2, Level 3 nodes with simple ticket costs and level entry buttons.
- [ ] **Basic shop:** Text/menu-based tool and weapon upgrades using gold.
- [ ] **Three temporary cards:** Slower stamina loss, +20 gold, chance for double ore fragments.
- [ ] **Minimal monsters:** No monster in Level 1, one simple monster in Level 2, reused or slightly stronger monster in Level 3.
- [ ] **Essential UI feedback:** Stamina, gold, ticket cost, shop purchase result, and run-end summary.
- [x] **Implemented vertical-slice systems:** Current scripts include stamina, tool/ore gating, map hall entry, shop upgrades, temporary cards, monster stamina pressure, gold UI, stamina UI, and run-end feedback. Keep the unchecked items above as final playtest verification tasks.

### Should-Have

Add only after the must-have loop is playable.

- [ ] One rare vein type with depth-scaled spawn chance.
- [ ] One hazard block type.
- [ ] Guide panel in the map hall.
- [ ] Simple mine-cart movement between nodes.
- [ ] Passive refinement multiplier in the shop.

### Could-Have

Add only if time remains after testing and report work.

- [ ] Second distinct monster type for Level 3.
- [ ] Run modifier before card selection.
- [ ] Card pool expansion upgrade.
- [ ] Rare-card chance upgrade.
- [ ] Four-choice card draft upgrade.
- [ ] Notification queue with fade animation.

### Cut-First

These should not be built for the solo two-week scope.

- [ ] Full forge/crafting chain.
- [ ] Complex monster AI or A* pathfinding.
- [ ] NPC shopkeeper art.
- [ ] Large inventory crafting economy.
- [ ] Fully animated mine-cart route.
- [ ] Large procedural event system with many hazards.

## 3. Final Game Submission Package

Required upload files:

- [ ] `StudentID_CW2_FinalSubmissionForm.pdf`
- [ ] `StudentID_CW2_FinalReport.pdf`
- [ ] `StudentID_CW2_FinalGameBuild.zip`
- [ ] `StudentID_CW2_DemoVideo.txt` if a demo video link is required or the build is too large

Build checklist:

- [ ] Build launches without Unity Editor.
- [ ] README explains how to open/run the Unity project.
- [ ] Build includes the playable vertical slice.
- [ ] Do not upload `Library`, `Temp`, `Obj`, `Logs`, or large build-cache folders.
- [ ] Final commit hash is recorded before submission.
- [x] Known issues are written honestly in `docs/FinalGameplayTesting.md`.

## 4. Final Submission Form Fields

Prepare answers for:

- [ ] **Student name:** Sihan Wang
- [ ] **Student ID:** Add final student ID.
- [ ] **Game title:** Mineral Odyssey
- [ ] **Unity version:** Unity 2022.3 LTS
- [ ] **Build platform:** Add final build target, probably Windows.
- [ ] **GitHub repository link:** `https://github.com/2003-40/GameProgramming`
- [ ] **Final commit hash:** Fill after final commit.
- [ ] **Playable build link:** Fill if not uploaded directly.
- [ ] **How to run the game:** Open build executable or open Unity project and run the main scene.
- [ ] **Controls:** Add final controls, such as WASD/Arrow Keys movement and mining interaction.
- [ ] **Main game objective:** Mine efficiently, manage stamina, survive deeper levels, earn gold, and upgrade.
- [ ] **Win / lose / completion condition:** Run ends when stamina reaches zero; vertical slice completion is reaching and playing Level 3.
- [ ] **Main systems/scripts created:** Mining, gold, stamina, tool/ore data, cards, shop, level entry, monster pressure, UI.
- [ ] **External assets/templates/tutorials/AI used:** Fill `README.md` reference placeholders and use `docs/ContributionList.md` for AI assistance wording.
- [x] **Known issues:** Final limitations such as simple monster AI, limited card variety, and required Play Mode sign-off are recorded in `docs/FinalGameplayTesting.md`.

## 5. Final Report Outline

The report should explain:

- [ ] **Design choices:** Why stamina is action-based, why the game uses short runs, why shop upgrades are menu-based.
- [ ] **Technical decisions:** Tilemap mining, ScriptableObject data, manager-based gold/stamina state, event-driven UI where used.
- [ ] **Problems and limitations:** Solo time constraint, reduced feature scope, simple monster AI, limited level count.
- [x] **Testing and changes:** Final stabilization notes, bugs found, and current balance values are recorded in `docs/FinalGameplayTesting.md`.
- [ ] **Reflection:** How the project changed from gather-process-trade-upgrade into a focused mining roguelite.
- [ ] **Personal contribution:** Clarify which systems and documentation were created personally.
- [ ] **External support:** Declare assets, tutorials, code snippets, and AI assistance.
- [ ] **Contribution source notes:** Use `docs/ContributionList.md` as the source for personal contribution, Unity operations, testing/debugging, improvements after feedback, and AI-assisted support.

## 6. Demo / Presentation Preparation

The live demo should show a clear loop:

- [ ] Start in the map hall.
- [ ] Explain level nodes and ticket cost.
- [ ] Open shop and show tool/weapon upgrade options.
- [ ] Choose a temporary card.
- [ ] Enter a level.
- [ ] Mine ores while stamina decreases.
- [ ] Show monster pressure in Level 2 or Level 3.
- [ ] End the run and show saved gold/progression.

Key explanation points:

- [ ] The game is intentionally scoped as a vertical slice.
- [ ] Stamina replaces a countdown timer to keep planning calm.
- [ ] Monsters are lightweight pressure systems, not full combat AI.
- [ ] The forge/crafting chain was cut to protect project quality.
- [ ] Permanent upgrades focus on access and efficiency instead of unlimited run length.

## 7. Professionalism Portfolio Checklist

Prepare one `StudentID_CW3_ProfessionalismPortfolio.pdf`.

Include:

- [ ] GitHub repository link.
- [ ] Development log showing progress over time.
- [ ] Summary of important commits.
- [ ] Evidence of planning and task management, such as GitHub issues.
- [ ] Evidence of response to feedback, including the scope change to stamina/card/shop systems.
- [x] Testing log and bug-fixing evidence in `docs/Devlog&Testing.md` and `docs/FinalGameplayTesting.md`.
- [ ] Screenshots or short evidence of progress over time.
- [ ] Explanation of how the project changed during development.
- [ ] External assets/templates/tutorials/AI declaration.
- [ ] Credits and licences.
- [ ] Reflection on organisation, time management, independent work, and professionalism.
- [ ] Known limitations and how they were managed.

## 8. External Resources and AI Declaration Template

For each external resource:

- **Name of resource:**
- **Type:** asset / template / tutorial / AI / code snippet / audio / image / model / other
- **Source:**
- **Licence or permission:**
- **What it provided:**
- **What I used unchanged:**
- **What I modified:**
- **What I created myself:**
- **Where it appears in my game:**
- **How it is credited:**

For AI assistance:

- **Tool used:** ChatGPT / Codex
- **What I asked:**
- **What output I used:**
- **What I changed:**
- **How I tested it:**
- **What I understand:**
- **What I still do not fully understand:**
- **Where it appears in the project:**

## 9. Two-Week Practical Schedule

This schedule assumes one developer who also needs time for classes, reports, and documentation.

### Days 1-2

- [ ] Implement stamina manager and stamina UI.
- [ ] Connect mining to stamina cost.
- [ ] Add run-end condition.

### Days 3-4

- [ ] Convert tool/ore progression to hardness and stamina-cost gating.
- [ ] Configure basic ore/tool values.

### Days 5-6

- [ ] Create static map hall.
- [ ] Add level nodes and ticket costs.
- [ ] Add basic level entry flow.

### Days 7-8

- [ ] Add basic shop.
- [ ] Add tool and weapon upgrade purchases.
- [ ] Save upgrade state.

### Days 9-10

- [ ] Add three temporary cards.
- [ ] Apply and clear card effects per run.

### Days 11-12

- [ ] Add minimal monster pressure.
- [ ] Add essential UI feedback.
- [ ] Test Level 1, Level 2, and Level 3 loop in Unity Play Mode using `docs/FinalGameplayTesting.md`.

### Days 13-14

- [ ] Fix bugs only.
- [ ] Balance stamina, ticket, ore value, and monster damage.
- [ ] Prepare final build, report notes, screenshots, and demo script.

## 10. Known Risk Register

- **Risk:** Too many roguelite systems for one person.
  - **Mitigation:** Build must-have only first; cut optional features aggressively.
- **Risk:** Monster AI becomes time-consuming.
  - **Mitigation:** Use simple patrol/chase/contact damage only.
- **Risk:** Map hall animation takes too long.
  - **Mitigation:** Use static route and simple cart position jumps.
- **Risk:** UI scope grows.
  - **Mitigation:** Use text panels and buttons; avoid custom art-heavy UI.
- **Risk:** Final report time is squeezed.
  - **Mitigation:** Keep this checklist updated and record design decisions as they happen.
