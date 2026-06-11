# Devlog & Testing

## 2026-05-24
By session 4, I will add a quest/task board system inspired by Stardew Valley-style task posting and reward flow, and I will show evidence by peer feedback recommending that a visible task board should be added to make objectives clearer and progression more guided.

## 2026-05-26
By session 6, I will add a progression path UI that clearly shows the route from Level 1 to Level 3, and I will show evidence by peer feedback suggesting a clear level path so players can understand stage transitions and goals.

## 2026-05-28
By session 7, I will implement a simplified gold economy system and remove forging-based progression from the core loop, and I will show evidence by peer feedback recommending adding a coin/gold system and simplifying gameplay by not using the forging system.

## 2026-06-04
Implemented an action-based stamina budget for mining runs. Mining now consumes stamina based on ore hardness and tool efficiency, monster contact can reduce stamina, and reaching zero stamina ends the current run by returning to the menu/map hall scene.

## 2026-06-11
Organized the current vertical slice around the playable Mineral Odyssey loop. The current project includes mining, stamina, gold rewards, map hall level selection, ticket costs, shop upgrades, temporary run cards, run-end UI, and runtime UI repair scripts.

Testing and debugging focus:

- Checked that mining attacks damage the intended tilemap cell.
- Checked that harder ores and tool levels affect stamina cost.
- Checked that gold updates after ore collection and shop purchases.
- Checked map hall node selection, ticket cost messaging, and scene loading.
- Checked card timer, card draw prompt, card selection, skipped card state, and temporary card effects.
- Checked run-end behavior when stamina reaches zero or the player ends the run manually.
- Fixed or guarded against missing UI references by adding runtime scene binding/bootstrap logic.
- Improved UI readability for stamina, gold, shop messages, card status, and run-end rewards.

Documentation update:

- Added English code comments across the Mineral Odyssey scripts.
- Added README reference placeholders for art assets, audio, tutorials, and AI coding assistance.
- Added `docs/ContributionList.md` as a working list of personal contribution and AI-assisted support.
