# Game Design Document (GDD) - Mineral Odyssey

## 1. Executive Summary

- **Game Title:** Mineral Odyssey
- **Genre:** 2D top-down mining roguelite / light management
- **Target Audience:** Casual players who enjoy short runs, resource planning, and permanent progression loops.
- **Scope:** A polished vertical slice built around a few-minute mining run, a hub map, simple level access, stamina budgeting, temporary card choices, and shop-based permanent upgrades.

## 2. One-Sentence Game Idea

Players enter increasingly dangerous mine levels from a hub map, spend limited stamina on mining and survival, collect gold, then return to the shop to unlock deeper access, better tools, weapons, and richer card choices.

## 3. Design Intent & Player Experience

The core design philosophy is **"every mining action spends a limited budget."** Stamina replaces a real-time countdown, allowing the player to stop and plan without pressure while still making each swing meaningful.

- **Level 1:** Entry-level mining with no monsters. It teaches ore values, stamina spending, collection, and gold rewards.
- **Level 2:** Adds one simple monster type. The player must balance mining value against stamina lost from enemy pressure.
- **Level 3:** Adds two simple monster types, harder ores, rare veins, and environmental hazards for higher risk/reward decisions.
- **Map Hall:** A hub screen shows an S-shaped mine-cart route from Start to Level 1, Level 2, and Level 3. Level nodes display ticket costs, unlock states, and expected risk/reward.
- **Guide & Trade UI:** The hub contains a guide panel for the next objective and a trade/shop panel for spending gold on tools, weapons, refinement bonuses, card upgrades, and level access.

## 4. Core Loop

1. Start in the map hall.
2. Read the guide, inspect level nodes, and enter the shop if needed.
3. Choose a level by paying its ticket cost or meeting its unlock condition.
4. View the current run modifier and choose a temporary card.
5. Mine ores and avoid or fight monsters while stamina lasts.
6. Collect gold and ore fragments.
7. End the run when stamina reaches zero or the player exits.
8. Spend saved gold on permanent progression.

## 5. Progression Model

Permanent upgrades should expand structure and access, not simply inflate stats.

- **Tool Upgrades:** Unlock the ability to mine harder ores and improve efficiency against matching ore tiers.
- **Weapon Upgrades:** Make Level 2 and Level 3 monsters manageable without making combat the main game.
- **Level Access:** Use unlock requirements and ticket costs to gate Level 2 and Level 3.
- **Card System Upgrades:** Expand the card pool, improve rare-card odds, and potentially upgrade the draft from three choices to four choices.
- **Refinement Upgrade:** Replace the full forge/crafting chain with a passive sell-value multiplier.
- **Anti-Farming Rule:** Once deeper levels are unlocked, shallow-level gold-per-stamina efficiency should become less attractive than deeper play.

## 6. Temporary Card Examples

Cards only affect the current run and should create situational choices.

- **Slower Stamina Loss:** Mining stamina cost is reduced for the current run.
- **Emergency Funding:** Gain 20 gold immediately.
- **Double Fragments:** Mining has a chance to drop two ore fragments.
- **Monster Bounty:** Defeated monsters drop extra gold.
- **First Hit Shield:** The first monster hit this run deals no stamina damage.
- **Ore Scanner:** Nearby high-value ores are easier to identify.
- **Ticket Refund:** Entering a deeper level refunds part of the ticket cost if the player reaches a target gold amount.
- **Risky Rich Veins:** Rare ore value increases, but monster stamina damage also increases.

## 7. Technical Architecture & Justification

- **Unity Tilemap System:** Supports destructible mining spaces and controlled level layouts.
- **ScriptableObjects for Data Decoupling:** Item, ore, tool, card, level, and upgrade data can be tuned without hardcoding relationships.
- **Manager-Based Runtime State:** Gold, stamina, run modifiers, selected cards, level access, and UI state are managed through dedicated runtime services.
- **Lightweight Enemy Behaviors:** Simple patrol, chase, and contact/attack logic creates pressure while avoiding complex AI scope.
- **PlayerPrefs / JSON Persistence:** Saves gold, level unlocks, shop upgrades, and progression state.
- **Event-Driven UI:** Stamina, gold, notifications, shop purchases, and run results update through events instead of direct UI coupling.

## 8. Scope Management & Risk Control

- **Included:** Stamina budget, map hall, ticketed level access, simple shop, temporary cards, tool progression, lightweight monsters, rare veins, hazard blocks, and notification UI.
- **Cut:** Full forge/crafting chain, complex monster AI, large inventory crafting economy, NPC shopkeeper art, and large multi-scene narrative content.
- **Reasoning:** The vertical slice should prove a coherent mining roguelite loop with clear decisions and stable systems. UI-driven upgrades and simple enemy pressure create depth without requiring major new art or AI complexity.

## 9. Professional Practice & Compliance

### 9.1 Legal & Asset Attribution

- **Art Assets:** Sourced from Kenney.nl, Itch.io free packs, and credited project assets.
- **Audio Assets:** Bfxr and Freesound assets where credited.
- **Compliance Action:** Asset credits and licenses should be documented in the repository and final report.

### 9.2 Accessibility Considerations

- **Planning-Friendly Pace:** Stamina is action-based rather than timer-based, so players can pause and think.
- **Visual Clarity:** Ore tiers should not rely on color alone; shape, texture, label, or UI hints should distinguish them.
- **Controls:** Support both WASD and arrow-key movement where practical.
