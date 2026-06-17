# Mineral Odyssey Final Presentation Prep

Use this as the last-night rehearsal sheet for a 5-minute English live demo plus Q&A.

## Main Goal

Show a stable playable vertical slice:

Map Hall -> choose level -> temporary card -> mine ore -> stamina decreases -> collect gold -> run ends -> return -> shop upgrade -> deeper level risk.

One sentence:

> Mineral Odyssey is a 2D top-down mining roguelite where stamina is the main run budget: every mining swing, monster hit, and deeper-level pressure affects how much value the player can extract before returning to upgrade.

## 5-Minute Live Demo Script

### 0:00-0:40 - Opening and Map Hall

Action: start the game, enter the Map Hall, point at the S-shaped route and level nodes.

Say:

> Hello, welcome to Mineral Odyssey. This is a 2D top-down pixel-art mining game built in Unity. The core loop is: choose a mine level, spend stamina to mine valuable ore, collect gold, return to the hub, and buy permanent upgrades.
>
> I designed the Map Hall as a clear progression screen. Level 1 is free, while deeper levels use ticket costs, so the player has to earn gold before reaching higher-risk, higher-reward areas.

### 0:40-1:45 - Core Mining Loop

Action: enter Level 1, follow the beginner guide, move with WASD, mine several ore tiles, show stamina and gold changes.

Say:

> I will start with Level 1, which is designed as the safe tutorial level. There are no monsters here, so the player can learn the basic controls and mining rhythm without pressure.
>
> The beginner guide introduces movement and mining step by step. The player can move around, face ore tiles, mine them, and immediately see stamina decreasing and rewards appearing.
>
> The important design point is that stamina replaces a timer. The game does not rush the player every second. Instead, stamina decreases when the player takes meaningful actions, especially mining. This makes Level 1 calm and readable, while still teaching that every swing has a cost.

### 1:45-3:00 - Risk, Cards, and Deeper Levels

Action: enter Level 2 or Level 3 if possible. Show monster pressure or explain it if time is tight. Trigger or point to card UI if available.

Say:

> Deeper levels add pressure, but I kept combat intentionally lightweight. Monsters do not create a separate health system. They damage the same stamina budget, so combat and mining compete for one resource.
>
> Level 2 slightly slows movement, and Level 3 adds a small idle stamina drain if the player stands still too long. This creates depth pressure without making the game too punishing.
>
> The run-card system adds variety. RunCardManager waits until the player has been active in a mining run, then offers a risky card draw. Cards can reduce stamina cost, restore stamina, double ore drops, or create negative effects such as higher stamina cost.

### 3:00-4:05 - Run End, Gold, and Shop

Action: end the run manually or by stamina depletion, return to Map Hall, open shop, show tool and weapon upgrades.

Say:

> When stamina reaches zero, the run ends and the UI summarizes the result. Gold is saved permanently using PlayerPrefs through GoldManager.
>
> In the shop, tool upgrades improve mining power and stamina efficiency, while weapon upgrades make monster pressure easier to manage. This closes the progression loop: mining creates gold, gold creates upgrades, and upgrades allow the player to handle harder ores and deeper levels.
>
> A major design decision was scope control. I originally considered a larger crafting and forging chain, but I cut it and focused on a polished vertical slice with mining, stamina, gold, upgrades, cards, and simple enemies.

### 4:05-5:00 - Technical Architecture, Testing, and Closing

Action: stay in the game. Do not exit Play Mode just to show code unless the teacher specifically asks.

Say:

> Since this is a live gameplay demo, I am keeping the technical code walk-through short during the five minutes. If there are questions afterward, I can explain the important scripts such as Player, ToolController, MiningController, StaminaManager, GoldManager, RunCardManager, and ShopUIController.
>
> At a high level, the project is data-driven where possible. MiningTile, ToolData, ItemData, and RunCardData are ScriptableObjects, so balance values can be edited in Unity without changing code.
>
> I also used persistent managers and runtime bootstrap scripts. GoldManager and StaminaManager persist across scenes, while UI bootstrap scripts repair or create important UI elements at runtime. This made the demo more stable during iteration.
>
> For testing, I checked mining hit detection, stamina costs, tool gating, item pickup, gold saving, shop purchases, card effects, run-end flow, and UI click issues. One important bug was decorative UI elements blocking buttons, which I fixed by checking raycast targets and ensuring an EventSystem exists.
>
> External assets and AI assistance are declared in the documentation. AI helped with planning and debugging suggestions, but I made the final design decisions, integrated the code, tested the project in Unity, and prepared the final playable build. Thank you, I am ready for questions.

## Demo Order Checklist

1. Main Menu -> Map Hall.
2. Select Level 1 and enter.
3. Mine ore and point out stamina loss.
4. Collect drops and point out gold reward.
5. Show tool-gated or harder ore if possible.
6. Show card prompt or explain it from the UI/status.
7. Show Level 2/3 monster or depth pressure if available.
8. End run or reach zero stamina.
9. Return to Map Hall.
10. Open shop and explain tool/weapon upgrades.

If time is short, skip Level 3 and explain it verbally. Do not get stuck trying to force a rare card moment.

## Code Architecture You Should Be Able To Explain

Core gameplay chain:

`Player` reads movement -> `ToolController` reads mouse attacks -> `MiningController` checks ore and spends stamina -> `MiningTile` supplies ore data -> `StaminaManager` tracks run budget -> ore drops use `ItemPickup` and `ItemRewardService` -> `GoldManager` saves gold -> `ShopUIController` spends gold through `PlayerUpgradeState` -> upgrades affect tool and weapon strength.

Scene/UI chain:

`MenuSceneBootstrap` wires menu buttons -> `MapHallBootstrap` repairs map hall UI -> `MapHallController` selects and loads levels -> mining UI bootstraps create stamina/gold/run-end/card UI -> `MiningReturnButtonBootstrap` handles end-run and return-to-map flow.

Data-driven chain:

`MiningTile`, `ToolData`, `ItemData`, and `RunCardData` are ScriptableObjects. The code reads their values, while Unity assets hold balance data.

## Script Cheat Sheet

### Player and Combat

- `Player.cs`: movement, facing direction, animator parameters, Level 2 speed slowdown, Level 3 idle stamina drain, stamina damage from monsters/hazards.
- `ToolController.cs`: mouse input, aiming, mining swing, weapon swing, hit radius checks, tool/weapon visuals, upgrade-based sprites.
- `SimpleMonsterMovement.cs`: simple patrol and chase movement using Rigidbody2D.
- `MonsterHealth.cs`: monster health and death/disable behavior.
- `StaminaDamageOnContact.cs`: configurable stamina damage and cooldown for enemies or hazards.
- `FirstLevelTutorialGuide.cs`: first-level tutorial prompts for movement, mining, and combat preview.

Important functions:

- `Player.Update()`: reads movement axes and updates facing/animation.
- `Player.FixedUpdate()`: moves the Rigidbody2D with scene-based speed multiplier.
- `Player.TryTakeStaminaDamage()`: shared collision/trigger stamina damage path.
- `Player.ResolveStaminaDamage()`: prefers explicit `StaminaDamageOnContact`, then tag fallback.
- `Player.ApplyLevelThreeIdlePressure()`: drains stamina after standing still in Level 3.
- `ToolController.TriggerAttack()`: starts mining or weapon action and records aim direction.
- `ToolController.PerformActionHit()`: sends mining hits to `MiningController` or weapon hits to monsters.
- `ToolController.CheckMonsterHit()`: damages each monster once per swing.
- `SimpleMonsterMovement.FixedUpdate()`: chooses chase or patrol.

### Mining

- `MiningController.cs`: central mining logic, stamina cost, tool gate, ore damage, particles, audio, drops, explosive hazard.
- `MiningTile.cs`: tile asset data: ore name, health, hardness, required tool level, stamina multiplier, drop prefab, particle prefab.
- `OreGenerator.cs`: fills bounded tilemap cells using spawn chance and weighted ore pool.
- `MiningParticlePool.cs`: reuses particle systems instead of repeatedly instantiating them.
- `MiningToolRequirementHint.cs`: shows a UI hint when tool level is too low.

Important functions:

- `TryMineNearPosition()`: finds the best ore tile near the swing hit area.
- `TryMineAtGridPosition()`: validates ore, tool level, stamina, card effects, and applies damage.
- `CalculateStaminaCost()`: hardness and ore multiplier increase cost; tool level and efficiency reduce it.
- `HandleDamage()`: tracks per-cell health, tints damaged ore, plays feedback.
- `ExecuteDestruction()`: removes ore or triggers explosion, then spawns rewards.
- `TriggerExplosion()`: clears nearby tiles and deals stamina damage.
- `SpawnOreDrops()`: uses card effects to decide drop count.
- `OreGenerator.GenerateMine()`: clears old ore and generates a fresh weighted layout.

### Stamina and Run End

- `StaminaManager.cs`: persistent singleton for run stamina, reset, consume, restore, run-end events.
- `StaminaDisplay.cs`: stamina text/fill UI and color feedback.
- `MiningStaminaUIBootstrap.cs`: creates or repairs stamina UI in mining scenes.
- `MiningReturnButtonBootstrap.cs`: binds end-run button, run-end panel, earned-gold text, and return-to-map button.

Important functions:

- `ResetRunStamina()`: starts each mining scene with full stamina.
- `ConsumeStamina()`: subtracts stamina and schedules run end at zero.
- `ScheduleEndCurrentRun()`: prevents duplicate run-end events.
- `EndCurrentRunAfterFrame()`: delays run end one frame so the current action finishes cleanly.
- `MiningReturnButtonBootstrap.ShowRunEndPanel()`: shows the end-run summary.
- `ReturnToMapHall()`: loads the Map Hall scene after a run.

### Economy and Upgrades

- `GoldManager.cs`: persistent gold, add/remove/spend, PlayerPrefs save, gold changed event.
- `GoldDisplay.cs`: UI text for current gold.
- `ItemRewardService.cs`: decides whether a pickup becomes gold or inventory.
- `ItemRewardMode.cs`: reward mode enum.
- `PlayerUpgradeState.cs`: static permanent upgrade state for tool and weapon levels.
- `ShopUIController.cs`: shop open/close, purchase logic, upgrade text, runtime UI binding.
- `MiningGoldUIBootstrap.cs`: creates or repairs mining gold UI.

Important functions:

- `GoldManager.AddGold()`: adds saved currency.
- `GoldManager.TrySpendGold()`: checks and spends gold for tickets/upgrades.
- `GoldManager.SetGold()`: writes to PlayerPrefs and broadcasts UI update.
- `PlayerUpgradeState.TryUpgradeTool()`: increases tool level and saves it.
- `PlayerUpgradeState.TryUpgradeWeapon()`: increases weapon level and saves it.
- `PlayerUpgradeState.ReduceMonsterStaminaDamage()`: weapon upgrades reduce contact damage.
- `ShopUIController.BuyToolUpgrade()`: spends gold and upgrades mining.
- `ShopUIController.BuyWeaponUpgrade()`: spends gold and upgrades combat.
- `ShopUIController.Refresh()`: updates all shop labels and button states.

### Cards

- `RunCardManager.cs`: one temporary card opportunity per run, timer, draw/skip/select flow, active effects.
- `RunCardData.cs`: ScriptableObject card identity, polarity, effect type, values.
- `RunCardChoiceUI.cs`: prompt, choice panel, card buttons, pause/resume, status text.
- `RunCardChoiceUIBootstrap.cs`: creates or repairs card UI in mining scenes.

Important functions:

- `RegisterCardTimerStartAction()`: starts the timer after real run activity.
- `Update()`: advances active mining seconds until the draw offer appears.
- `AcceptDrawOffer()`: randomly chooses good or bad card pool.
- `SelectCard()`: applies one selected card and resolves the opportunity.
- `ApplyCard()`: changes stamina cost, gold, drop chance, free hit chance, or fail chance.
- `ModifyStaminaCost()`: lets cards adjust mining stamina cost.
- `GetOreDropCount()`: lets cards double or cancel ore drops.

### Items and Inventory

- `ItemData.cs`: ScriptableObject item identity, icon, type, and gold value.
- `ItemType.cs`: item category enum.
- `ItemPickup.cs`: collectable item that grants reward and destroys itself.
- `ItemMagnet.cs`: pulls drops toward the player after a short delay.
- `ItemStack.cs`: item plus quantity.
- `InventoryManager.cs`: persistent inventory with add/remove events, used when rewards are not auto-converted.

Important functions:

- `ItemPickup.TryCollect()`: grants reward, starts card timer for ore, destroys pickup.
- `ItemMagnet.Update()`: waits, attracts pickup, then collects near player.
- `InventoryManager.AddItem()`: stacks items or creates a new stack.
- `InventoryManager.RemoveItem()`: removes one item and notifies listeners.

### Map Hall and Menu

- `MenuSceneBootstrap.cs`: finds and wires Start/Exit/story buttons.
- `MenuStoryIntroController.cs`: optional story intro animation and skip/open-map flow.
- `MapHallBootstrap.cs`: builds or repairs Map Hall UI, level nodes, marker, EventSystem, shop controller.
- `MapHallController.cs`: selected level, ticket cost, cart marker, gold checks, scene loading.
- `MapHallLevelOption.cs`: serializable level option data.

Important functions:

- `MapHallController.Initialize()`: receives generated/manual UI references.
- `BindLevelOptions()`: connects node buttons and enter button.
- `SelectOption()`: moves cart marker and updates selected level.
- `EnterSelectedLevel()`: spends ticket cost and loads the selected scene.
- `UpdateSelectionState()`: updates messages based on current gold.
- `DisableMarkerRaycasts()`: prevents decorative marker from blocking clicks.

## High-Probability Teacher Questions

### What is the core mechanic?

> The core mechanic is stamina as a run budget. Mining, monsters, and deeper-level pressure all consume stamina, so every action competes for the same limited resource.

### Why did you choose stamina instead of a timer?

> A timer creates real-time stress. I wanted calmer planning, so stamina only changes when the player acts or takes risk. It still creates pressure, but the player can stop and think.

### How does mining work in code?

> ToolController receives mouse input and calculates a hit area. MiningController finds the ore tile, checks the required tool level, calculates stamina cost from the MiningTile data, consumes stamina through StaminaManager, applies damage, and spawns drops when the tile breaks.

### How is ore data stored?

> Ore data is stored in MiningTile ScriptableObject-style tile assets. Each tile has health, hardness, required tool level, stamina multiplier, drop prefab, and particle feedback.

### How do upgrades affect gameplay?

> PlayerUpgradeState stores permanent tool and weapon levels in PlayerPrefs. Tool upgrades increase mining power and stamina efficiency. Weapon upgrades increase damage and reduce monster contact stamina loss.

### Why are monsters simple?

> The project is a vertical slice, so monsters are designed as pressure systems rather than full combat AI. They patrol, chase nearby players, and deal stamina damage. This supports the core loop without expanding scope too much.

### What is the most important technical decision?

> I separated persistent state, data, and UI. GoldManager and StaminaManager manage state, ScriptableObjects hold balance data, and UI scripts only display or trigger actions. This made the project easier to test and adjust.

### What does the card system add?

> It adds run-to-run variation. RunCardManager gives one temporary card opportunity after active mining time. Cards modify existing hooks like stamina cost, drop count, or immediate gold, so the system is flexible without rewriting mining logic.

### How did you save progress?

> I used PlayerPrefs for gold and upgrade levels. GoldManager saves gold immediately after changes, and PlayerUpgradeState saves tool and weapon levels.

### What bugs did you fix?

> I fixed UI click problems caused by decorative elements blocking raycasts. I also added runtime bootstrap scripts to repair missing UI references and ensure important managers or EventSystems exist.

### What did AI help with?

> AI helped as a technical assistant for planning, debugging ideas, code structure suggestions, and documentation wording. I still made the final design decisions, integrated the code, tested behavior in Unity, and reviewed the final project.

### What are the limitations?

> The main limitations are simple monster AI, a small number of levels and cards, and a focused shop instead of a full crafting system. I chose these limits deliberately to deliver a stable playable vertical slice.

### What would you improve next?

> I would add more card variety, better level balancing, clearer enemy types, more audio feedback, and possibly a refined crafting layer after the core loop is fully stable.

## Emergency Answers

If asked about a script you forgot:

> This script supports the main loop rather than defining it alone. At a high level, it either stores data, manages persistent state, handles UI binding, or connects Unity scene objects to the core mining-stamina-gold loop.

If a demo bug happens:

> This is a live Unity demo issue, but the intended flow is documented and the system was tested through the final checklist. The relevant code path is still clear: the player action goes through ToolController, MiningController, StaminaManager, and then UI feedback.

If asked why a feature is missing:

> I cut that feature because the coursework required a stable playable vertical slice. I prioritized the core loop, feedback, testing, and understandable code over adding a larger but less reliable system.

## Tonight Practice Plan

1. Read the 5-minute script out loud twice without Unity.
2. Run the game and rehearse only the actions, without speaking.
3. Run the game and speak the script while playing.
4. Open `MiningController.cs`, `ToolController.cs`, `Player.cs`, `StaminaManager.cs`, `RunCardManager.cs`, and `ShopUIController.cs`; explain each in one sentence.
5. Practice the high-probability questions in random order.
6. Before sleeping, memorize the core chain:

`ToolController -> MiningController -> StaminaManager -> ItemPickup/GoldManager -> ShopUIController/PlayerUpgradeState -> MapHallController`
