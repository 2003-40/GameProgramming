# Mineral Odyssey Final Gameplay Testing

Date: 2026-06-12

This document records the final stabilization pass for the current Mineral Odyssey vertical slice.

## Test Environment

- Project: `project/Mineral Odyssey`
- Unity version from project settings: 2022.3.62f3c1
- Static/code verification completed from the repository files.
- Unity batchmode smoke test was attempted with `D:\UnityEditor\2022.3.62f3c1\Editor\Unity.exe`, but the process did not produce the requested log and did not exit within the timeout. Do not count that attempt as a passed Play Mode test.

## Final Demo Loop

Target loop:

MapHall -> select level -> enter mine -> mine ores -> collect gold -> use stamina -> encounter card/monster/hazard -> end run -> return to MapHall -> buy upgrades -> access deeper levels.

## Static Verification Completed

- Build Settings include `_Menu`, `MapHall`, `FirstFlour`, `SecondFlour`, and `ThirdFlour`.
- `MapHallController` spends saved gold for level tickets before loading the selected mine scene.
- `MapHall.unity` level ticket costs are now aligned with the runtime bootstrap defaults:
  - Level 1: 0 Gold
  - Level 2: 25 Gold
  - Level 3: 60 Gold
- All three mining scenes reference `MiningUI.prefab`, which contains `RunEndPanel`, `EndTurn`, `ReturnToMapButton`, `GoldEarnedText`, and `TotalGoldText`.
- `GoldManager` persists gold immediately through `PlayerPrefs`.
- Ore pickups use `ItemRewardService` and `ItemPickup` to convert ore values into gold.
- `StaminaManager` is shared by mining, monster contact, hazard contact, and explosive hazard damage.
- `MiningReturnButtonBootstrap` listens for run end, shows the run-end panel, pauses gameplay, and returns to `MapHall`.
- `RunCardManager` starts the card timer from mining, attacking, or ore collection, then exposes draw, skip, and selection states.
- `RunCardChoiceUIBootstrap` can generate card prompt/choice UI if a mining scene does not already contain one.
- `ShopUIController` handles tool upgrade purchase, weapon upgrade purchase, insufficient gold feedback, and max-level state.
- `Player` applies light depth-pressure rules by scene: Level 2 movement is slightly slower, and Level 3 drains a small amount of stamina only after the player stands still briefly.
- No missing script or empty prefab GUID references were found in the target Mineral Odyssey scenes/prefabs during static search.

## Bugs Found and Fixed

- **Fixed:** Level 2 ticket cost was inconsistent. `MapHall.unity` used 20 Gold while `MapHallBootstrap` used 25 Gold. The scene value is now 25 Gold.
- **Changed after peer feedback:** Added subtle Level 2 movement slowdown and subtle Level 3 idle stamina drain to make deeper levels feel different without making the vertical slice much harder.

## Manual Playtest Checklist

Use a fresh PlayerPrefs state before this pass if possible.

- [ ] Fresh save starts in or reaches `MapHall` cleanly.
- [ ] Level 1 can be selected and entered for free.
- [ ] Level 1 mining consumes stamina and produces collectible ore drops.
- [ ] Collecting ore drops increases the visible gold total.
- [ ] Manual `EndTurn` shows the run-end panel.
- [ ] Reducing stamina to zero shows the run-end panel.
- [ ] `ReturnToMapButton` returns to `MapHall` and restores `Time.timeScale` to 1.
- [ ] Gold persists after returning to `MapHall`.
- [ ] Shop tool upgrade succeeds with enough gold.
- [ ] Shop tool upgrade shows an insufficient gold message when gold is too low.
- [ ] Shop tool upgrade reaches max level and disables/labels the max state correctly.
- [ ] Shop weapon upgrade succeeds with enough gold.
- [ ] Shop weapon upgrade shows an insufficient gold message when gold is too low.
- [ ] Shop weapon upgrade reaches max level and disables/labels the max state correctly.
- [ ] Level 2 requires 25 Gold and deducts the ticket cost on entry.
- [ ] Level 2 player movement feels only slightly slower, not frustrating or stuck.
- [ ] Level 2 monster contact reduces stamina and does not block the return flow.
- [ ] Level 2 mining and return flow remain stable.
- [ ] Level 3 requires 60 Gold and deducts the ticket cost on entry.
- [ ] Level 3 standing still for a short time drains stamina slowly, while brief pauses remain playable.
- [ ] Level 3 contains higher-risk ore layout and monster pressure.
- [ ] Level 3 hazard/rare ore behavior is reachable and does not break the run-end flow.
- [ ] Card timer starts after mining, attacking, or ore collection.
- [ ] Card prompt appears after the timer reaches the required duration.
- [ ] Skip closes the card prompt and records the skipped state.
- [ ] Draw opens three card choices.
- [ ] Selecting a card applies the card effect and closes the choice UI.

## Current Balance Values

- Ticket costs: Level 1 free, Level 2 25 Gold, Level 3 60 Gold.
- Ore values:
  - Yellow Gemstone: 5 Gold
  - White Gemstone: 8 Gold
  - Blue Gemstone: 12 Gold
  - Red Gemstone: 17 Gold
  - Purple Gemstone: 25 Gold
- Tool upgrade costs: 75, 150.
- Weapon upgrade costs: 50, 100.
- Default monster contact stamina damage: 10.
- Explosive hazard stamina damage: 10.
- Level 2 movement multiplier: 0.85.
- Level 3 idle stamina drain: 1 stamina after 2.5 seconds idle, then every 4 seconds while idle.

## Known Issues / Limitations

- A final hands-on Play Mode pass is still required because the batchmode run did not complete with a usable log in this environment.
- Monster AI is intentionally simple and exists mainly as stamina pressure.
- Card UI is functional but minimal.
- The vertical slice uses a small card pool and only three mine levels.
