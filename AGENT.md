# AGENTS.md

## Project Context

- This repository contains a Unity game named Mineral Odyssey.
- The Unity project root is `project/Mineral Odyssey`.
- Use Unity `2022.3.62f3c1`, as recorded in `project/Mineral Odyssey/ProjectSettings/ProjectVersion.txt`.
- The game is a 2D top-down pixel-art mining roguelite focused on a short vertical slice: mining, stamina, gold economy, map hall, shop upgrades, tools, ore gating, and simple monster pressure.

## Repository Layout

- `README.md` describes the current design, roadmap, and delivery scope.
- `docs/` contains submission, devlog, testing, and project documentation.
- `project/Mineral Odyssey/Assets/Scripts/` contains gameplay scripts.
- Main script areas:
  - `Economy/`: gold, rewards, player upgrades, shop UI.
  - `Inventory/`: inventory state.
  - `Items/`: item data, pickups, item stack/type definitions.
  - `Mining/`: mining controller, mining tiles, ore generation, particles.
  - `Stamina/`: stamina manager, display, contact damage, stamina UI bootstrap.
  - `MapHall/`: hub/map hall controllers and menu bootstrap.
  - `Tools/`: tool data and tool control.
  - `Player/`: player and monster health behavior.

## Working Rules

- Keep changes small and focused on the requested feature or bug.
- Follow existing module boundaries instead of creating unrelated new systems.
- Preserve Unity `.meta` files. Do not delete, regenerate, or rename assets/scripts without a clear reason.
- Do not edit generated Unity folders such as `Library/`, `Temp/`, `Obj/`, `Logs/`, or build output folders.
- Avoid broad scene, prefab, or asset rewrites unless the task explicitly requires Unity asset changes.
- When changing serialized fields, prefer additive changes and preserve existing field names when possible to avoid breaking Unity scene/prefab references.
- Prefer data-driven Unity patterns already used in the project, especially ScriptableObjects for item, ore, tool, card, level, and upgrade metadata.
- Do not introduce heavy architecture or complex combat systems; this project prioritizes a stable solo vertical slice.

## C# Style

- Use clear, direct Unity C#.
- Prefer explicit serialized fields with `[SerializeField] private ...` for inspector-controlled values.
- Keep MonoBehaviour responsibilities narrow.
- Avoid hidden global coupling unless matching an existing manager pattern in the project.
- Validate null references defensively where scene wiring or optional UI references are involved.
- Keep public APIs minimal. Make fields private unless Unity serialization or other scripts require access.

## Gameplay Priorities

- Preserve the core loop: mine -> spend stamina -> collect rewards -> gain gold -> upgrade -> access deeper levels.
- Stamina is the run budget. Mining and monster hits may spend stamina; planning and movement should not.
- Gold and upgrade progression should remain simple and demonstrable.
- Prefer readable UI feedback over complex systems: stamina, gold gain, ticket cost, shop result, and run-end state matter most.
- Keep optional roadmap features secondary unless explicitly requested.

## Verification

- If Unity Editor cannot be run from the current environment, state that clearly.
- For script changes, check compile-sensitive issues manually: namespaces, class names, file names, serialized field names, missing references, and Unity API usage.
- When possible, inspect changed C# files for syntax and reference consistency.
- For scene/prefab-facing changes, mention what should be checked in Unity Play Mode.
- Do not claim a Unity Play Mode test or build succeeded unless it was actually run.

## Documentation

- Update `README.md` or files in `docs/` only when behavior, setup, scope, or submission evidence changes.
- Keep documentation aligned with the current vertical-slice scope.
- Mention known limitations honestly instead of implying incomplete systems are finished.

## Git Safety

- Do not revert user changes unless explicitly asked.
- Do not run destructive git commands such as `git reset --hard` or checkout files to discard changes unless explicitly requested.
- Before large edits, inspect the relevant files and preserve unrelated work.