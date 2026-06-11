# Mineral Odyssey - List of Contribution

This document is a working contribution record for the final report. It is not the final report itself. It separates personal contribution from AI-assisted support so the final submission can explain the work honestly and clearly.

## 1. Game Idea and Design Decisions

- I designed **Mineral Odyssey** as a 2D top-down pixel-art mining roguelite focused on short, repeatable mining runs.
- I changed the project direction from a broader crafting/forge concept into a more achievable vertical slice built around mining, stamina, gold, upgrades, and level progression.
- I chose an **action-based stamina system** instead of a real-time countdown, so the player can plan calmly while every mining swing still has a cost.
- I designed the progression as **map hall -> choose level -> mine -> collect gold -> return -> upgrade -> unlock deeper levels**.
- I decided to keep combat lightweight. Monsters are used as stamina pressure and positioning risk, not as a full combat system.
- I cut or reduced large systems such as a full forge/crafting chain, complex monster AI, large inventory economy, and heavy narrative content to protect the quality of the playable vertical slice.
- I designed the level structure around Level 1, Level 2, and Level 3, with increasing cost, risk, and reward.
- I planned the card system as a temporary run modifier system, including both helpful cards and curse-style bad cards.

## 2. Core Mechanics

- I implemented the core mining loop where the player mines ore tiles, damages them over several hits, destroys them, and receives collectable rewards.
- I designed ore properties such as health, hardness, required tool level, stamina multiplier, drop prefab, and hit particle feedback.
- I implemented stamina as the main run budget. Mining consumes stamina based on ore hardness and tool efficiency.
- I connected monster and hazard contact to stamina damage, so enemies affect the same resource budget as mining.
- I implemented run-end behavior when stamina reaches zero, including a run-end UI panel and return-to-map flow.
- I implemented permanent gold as the main saved progression currency.
- I implemented ticket costs for entering deeper levels.
- I implemented tool and weapon upgrades so gold has a clear purpose after each run.

## 3. Unity Scene Setup and Level Flow

- I set up the Unity project around the **Mineral Odyssey** playable scenes and scripts.
- I configured mining scenes with Tilemap-based ore layouts and destructible ore behavior.
- I configured scene transitions between the main menu, map hall, and mining levels.
- I built or repaired the **Map Hall** flow with Start, Level 1, Level 2, and Level 3 nodes.
- I positioned the cart marker and level nodes so the route is visually readable as an S-shaped mine path.
- I connected level buttons to scene loading and ticket cost checks.
- I added runtime bootstrap scripts to keep important UI elements working even if scene references are missing during Unity iteration.
- I set up run-end UI objects such as `RunEndPanel`, `EndTurn`, `ReturnToMapButton`, `GoldEarnedText`, and `TotalGoldText`.

## 4. Scripts Created or Modified

### Player and Interaction

- `Player.cs`: movement, facing direction, animation parameter updates, stamina damage from enemies/hazards, and animation-event bridge for tool hits.
- `ToolController.cs`: mouse/tool input, attack direction, mining hit area, tool visual repair, and simple monster hit detection.
- `MonsterHealth.cs`: simple monster damage and death/disable behavior.

### Mining

- `MiningController.cs`: tile hit detection, tool gate checks, stamina cost calculation, ore health tracking, damage tinting, wobble feedback, item drops, particles, and audio feedback.
- `MiningTile.cs`: ore data such as health, hardness, required tool level, stamina multiplier, particle prefab, and drop prefab.
- `OreGenerator.cs`: bounded ore generation with global spawn density and weighted ore selection.
- `MiningParticlePool.cs`: reusable particle system pool for mining hit and destruction feedback.
- `OreGeneratorEditor.cs`: editor visualization for ore generation bounds.

### Items and Inventory

- `ItemData.cs`: ScriptableObject item definition for item identity, icon, type, and gold value.
- `ItemType.cs`: item category enum for ores, tools, consumables, quest items, and miscellaneous items.
- `ItemPickup.cs`: pickup collection and reward granting.
- `ItemMagnet.cs`: delayed item attraction toward the player.
- `ItemStack.cs`: item stack data for inventory storage.
- `InventoryManager.cs`: persistent inventory manager with stack add/remove events.

### Economy and Upgrades

- `GoldManager.cs`: persistent gold storage, spending, adding, removing, and change events.
- `GoldDisplay.cs`: UI display for gold.
- `ItemRewardService.cs`: reward routing between gold conversion and inventory storage.
- `ItemRewardMode.cs`: reward mode selection.
- `PlayerUpgradeState.cs`: permanent tool and weapon upgrade levels, costs, saved state, and monster damage reduction.
- `ShopUIController.cs`: shop panel behavior, upgrade purchase logic, messages, and runtime UI binding/repair.
- `MiningGoldUIBootstrap.cs`: automatic gold display creation in mining scenes.

### Stamina and Run End

- `StaminaManager.cs`: stamina tracking, stamina restoration, stamina consumption, run-end event, and reset behavior on mining scene entry.
- `StaminaDisplay.cs`: stamina text, fill amount, and low/high stamina color feedback.
- `StaminaDamageOnContact.cs`: enemy/hazard stamina damage data.
- `MiningStaminaUIBootstrap.cs`: automatic stamina UI creation in mining scenes.
- `MiningReturnButtonBootstrap.cs`: end-run button, run-end panel binding, earned-gold summary, and return-to-map behavior.

### Cards

- `RunCardManager.cs`: run-card timer, draw offer, card selection, card effects, good/bad card pools, and per-run reset logic.
- `RunCardData.cs`: card data model, polarity, and effect type definitions.
- `RunCardChoiceUI.cs`: draw prompt, choice panel, card buttons, pause behavior, and status text.
- `RunCardChoiceUIBootstrap.cs`: generated runtime UI for the card system when no manual UI is present.

### Map Hall and Menu

- `MapHallBootstrap.cs`: generated or repaired map hall UI, route, nodes, marker, event system, shop controller attachment, and manual UI binding.
- `MapHallController.cs`: level selection, cart marker movement, ticket checks, message text, and scene loading.
- `MapHallLevelOption.cs`: serializable level node data.
- `MenuSceneBootstrap.cs`: main menu start/exit button wiring.

## 5. Player Interaction

- I connected WASD/arrow-style axis movement through Unity input axes.
- I preserved the player's last valid facing direction for idle animation and mining direction.
- I made mining attacks use the current mouse direction converted into cardinal directions, matching the 2D character animation style.
- I connected player animation events to tool hit detection so mining hits occur at the correct swing moment.
- I added item magnet behavior so dropped ore moves toward the player after a short delay.
- I added monster contact damage and weapon hits so movement around enemies matters.
- I added buttons for map level selection, shop purchases, card choices, ending the run, and returning to the map hall.

## 6. UI and Feedback

- I built or refined the gold display, stamina display, map hall UI, shop UI, card UI, and run-end panel.
- I improved UI feedback for insufficient gold, successful upgrades, max-level upgrades, selected level, ticket cost, selected card, skipped card, and run-end earnings.
- I added stamina color feedback that moves from green to red as stamina decreases.
- I used generated UI bootstraps to make the project more robust when scene references are missing.
- I improved map hall readability by using clear level nodes, ticket text, route segments, and a cart marker.
- I improved shop readability by organizing tool and weapon upgrades into a scrollable upgrade list.
- I improved card readability by showing three card choices with titles, descriptions, and good/bad card states.

## 7. Rules and Balance Settings

- Mining consumes stamina only when a valid mining hit is attempted.
- Ore hardness and stamina multiplier increase the stamina cost.
- Tool level and tool efficiency reduce mining cost and gate harder ores.
- Tool upgrades increase mining power and stamina efficiency.
- Weapon upgrades increase damage and reduce monster contact stamina loss.
- Level 1 is free, Level 2 costs gold, and Level 3 costs more gold.
- Run-card effects reset after the current mining run.
- The player receives one card opportunity after enough active mining time.
- Accepting the card draw can create either a good-card set or a bad-card set.
- Stamina reaching zero triggers the end of the current run.
- Gold is saved persistently through `PlayerPrefs`.

## 8. Unity Operations and Polish Work

- I configured Tilemap-based mining areas and ore placement.
- I configured ore assets with health, hardness, drops, and feedback values.
- I assigned prefabs for ore drops and pickup behavior.
- I configured UI canvases, buttons, TextMeshPro labels, panels, and scrollable shop content.
- I adjusted UI positions such as stamina at the bottom-right and gold at the top-left for quick readability.
- I repaired scene UI raycast settings so decorative objects do not block important buttons.
- I connected scene names for map hall level entry and return flow.
- I used runtime initialization methods to make important systems initialize after scene load.
- I added or refined audio/particle feedback for mining hit and ore destruction.
- I documented code with English comments so the implementation can be explained during assessment.

## 9. Testing and Debugging Contribution

- I tested mining interaction by checking whether tool swings hit the correct ore tile.
- I tested stamina cost behavior with different ore hardness, tool levels, and card modifiers.
- I tested that the player cannot mine ores above the current required tool level.
- I tested item drops, pickup magnet behavior, and gold reward conversion.
- I tested that gold saves and updates in UI after collection and shop purchases.
- I tested map hall selection, ticket cost messages, and scene loading.
- I tested that shop buttons correctly handle successful purchase, insufficient gold, and max-level states.
- I tested card timing, draw offer, skip behavior, selected card effects, and pause/resume behavior.
- I tested run-end behavior from stamina depletion and manual end-run button.
- I debugged UI click problems by disabling decorative raycast targets and ensuring an EventSystem exists.
- I added logging for mining blocks, stamina costs, reward collection, and run-card selection to make debugging clearer.

## 10. Improvements After Feedback

- After feedback that objectives and progression needed to be clearer, I added the map hall and level-route structure.
- After feedback that the project scope was too large, I simplified the design into a focused mining vertical slice.
- After feedback around progression clarity, I added ticket costs, level buttons, and shop upgrades.
- After feedback around player motivation, I made gold the central reward and upgrade currency.
- After feedback around the forge/crafting system being too much for the time available, I removed it from the core loop.
- After feedback around needing stronger moment-to-moment decisions, I added stamina cost decisions and run-card variation.
- After UI issues during iteration, I added runtime UI repair/bootstrap scripts to keep the demo playable.
- After playability checks, I added more feedback for stamina, gold, shop messages, selected card state, and run-end rewards.

## 11. AI-Assisted Support

AI tools such as ChatGPT / Codex supported the project as an assistant, not as a replacement for my own design and integration work.

AI helped with:

- Suggesting ways to reduce the project scope into a realistic solo vertical slice.
- Suggesting code structure for manager-style systems such as stamina, gold, cards, and UI bootstraps.
- Suggesting debugging approaches for Unity UI references, button clicks, and scene initialization.
- Helping draft or reorganize documentation, README sections, contribution notes, and code comments.
- Helping explain how to separate personal contribution from external assets and AI assistance for the final report.
- Helping identify that resource links, audio credits, asset credits, tutorials, and AI declarations need to be recorded.

My own contribution remained:

- Final game idea, feature selection, scope decisions, and design direction.
- Unity scene setup, object placement, UI arrangement, asset assignment, and visual polish decisions.
- Deciding which suggested code or documentation changes to use.
- Integrating scripts into the Unity project.
- Testing the game loop, checking behavior in Unity, and making gameplay/UI adjustments.
- Preparing the final project structure and submission evidence.

## 12. Items To Fill Before Final Submission

- Add final external asset links and licences in `README.md`.
- Add exact audio source links and licences.
- Add AI conversation/tool declaration details required by the coursework form.
- Add final screenshots or short testing evidence.
- Add known issues honestly after the final playtest.
- Add final commit hash and build link after creating the final build.
