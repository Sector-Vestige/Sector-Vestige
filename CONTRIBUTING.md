# Sector Vestige Contributing Guidelines

Sector Vestige is a custom-content fork of Space Station 14 (https://github.com/space-wizards/space-station-14), built to extend and reshape the game with our own mechanics, assets, and content (all in due time).

We follow upstream's PR guidelines:
https://docs.spacestation14.com/en/general-development/codebase-info/pull-request-guidelines.html

> Do **not** use GitHub’s web editor to submit PRs.
> PRs submitted through the web editor may be closed without review.

All contributors are expected to have a working development environment:
https://docs.spacestation14.com/en/general-development/setup/setting-up-a-development-environment.html

By submitting a PR, you agree that your code contributions are licensed under AGPL-3.0-or-later, in line with Sector Vestige’s licensing model for original content.

---

## Table of Contents

- Sector Vestige-specific Content
- Changes to Upstream Files
  - Commenting Example
- Porting (Importing from Other Forks)
- Mapping
- Art and Spriting
- Before You Submit
- Changelogs
- Additional Resources

---

## Sector Vestige-specific Content

When adding new content, create it under `_SV` folders whenever possible.
This keeps our content cleanly separated from upstream code and simplifies merges.

Examples:
- Content.Server/_SV/Speech/EntitySystems/IlleismAccentSystem.cs
- Resources/Prototypes/_SV/game_presets.yml
- Resources/Textures/_SV/Clothing/Shoes/Misc/ducky-galoshes.rsi
- Resources/Locale/en-US/_SV/game-ticking/game-presets/preset-deathmatchpromod.ftl

---

## Changes to Upstream Files

If you need to modify upstream files (C#, YAML, etc.), you must comment your changes.
This helps with resolving future merge conflicts and makes your intent clear.

- Always comment near the lines you change
- Do not delete upstream code — comment it out
- For large additions, use partial classes when appropriate
- Fluent (.ftl) files don’t support inline comments — edit with care

### Commenting Example

YAML – Inline field comment:
```
  hidden: false # Sector Vestige change for various Vox masks
```

YAML – Block comment:
```
  # Begin Sector Vestige Personal trinkets
  - ItemJamesCane
  - ItemAnnabellePlushie
  - ItemPititiRockGorb
  # End Sector Vestige Personal trinkets
```

C# – Inline logic comment:
```
  if (!_actionBlocker.CanSpeak(source, true) && !ignoreActionBlocker) // Sector Vestige: support hypophonia trait
```

C# – Enclosing block:
```
  // Sector Vestige - start of additional statuses (ported from CD)
  SecurityStatus.Monitor => "SecurityIconMonitor",
  SecurityStatus.Search => "SecurityIconSearch",
  // Sector Vestige - end of additional statuses (ported from CD)
```
---

## Porting (Importing from Other Forks)

When porting content (code, YAML, assets) from other SS14 forks (e.g., Delta-V, Harmony, Frontier, etc.):

- Place content in a clearly named subfolder with a `_` prefix.
  Example:
  - _DV/ for Delta-V
  - _NF/ for Frontier
  - _Harmony/ for Harmony

- Do not mix ported code into _SV or upstream folders.
- This separation makes it easy to remove or update entire ports later.

### License Requirements

- MIT: freely portable. (will be labeled by the bot just in case)
- AGPL: allowed, but must be clearly labeled.
  - Add an SPDX license header to each file: (Bot does that on PR for you)
    // SPDX-License-Identifier: AGPL-3.0-or-later
    // Copyright (c) 2025 Delta-V contributors
  - Keep the code in its _ForkName/ folder.
  - AGPL requires source disclosure for servers running AGPL code.

If you are unsure about the license of something you want to port, ask in the Sector Vestige Discord before submitting.

---

## Mapping

- Follow upstream mapping guidelines: https://docs.spacestation14.com/en/space-station-14/mapping.html
- Test maps thoroughly (power, atmos, gravity, lighting).
- Sector Vestige-exclusive maps (e.g., shuttles, wrecks) should fit an immersive MRP style.
- Submit upstream map changes to upstream when possible.

---

## Art and Spriting

- Test sprites in-game, not just in the editor.
- Provide in-game screenshots in your PR.
- We do not enforce a strict palette — consistency is nice, but creativity is welcome.

---

## Before You Submit

- Double-check your diff:
  - Remove unintended changes
  - Avoid whitespace-only diffs

- Confirm your target:
  - Repository: Sector Vestige/Sector Vestige
  - Branch: master

To undo accidental RobustToolbox changes:
  git checkout upstream/master RobustToolbox

(Replace "upstream" with your space-wizards remote.)

---

## Changelogs

Use the :cl: tag in your PR body to describe player-facing changes.

Valid types: add, remove, tweak, fix

Example:
:cl:
- add: Added a new gun to the armory.
- fix: Fixed crew manifest showing dead people.

Only entries after :cl: are read by Weh Bot.
Sector Vestige does not maintain a separate Admin changelog.

---

## Additional Resources

- SS14 Developer Docs: https://docs.spacestation14.io/
