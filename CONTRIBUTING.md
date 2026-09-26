# Sector Vestige Contributing Guidelines

Sector Vestige is a custom-content fork of Space Station 14 (https://github.com/space-wizards/space-station-14), built to extend and reshape the game with our own mechanics, assets, and content (all in due time).

We follow upstream's PR guidelines:
https://docs.spacestation14.com/en/general-development/codebase-info/pull-request-guidelines.html

> Do **not** use GitHub’s web editor to submit PRs.
> PRs submitted through the web editor may be closed without review.

All contributors are expected to have a working development environment:
https://docs.spacestation14.com/en/general-development/setup/setting-up-a-development-environment.html

## Licensing

- New original content goes under a `_SV/` path and is licensed AGPL-3.0-or-later.
- Edits to files outside `_SV/` (upstream code or ported fork code) stay under that file's existing license. Mark each edit with an `SV:` comment (see below).
- By contributing an edit to an MIT-licensed upstream file, you agree that the edit may be redistributed under MIT, including for contribution back to space-wizards/space-station-14.

Which folder has which license is recorded in `REUSE.toml`. CI checks that the license REUSE computes for every `_SV/` and `_Fork/` file matches that file. If you add a new fork folder, add a `**/_Fork/**` table to `REUSE.toml` (at the end, order matters) and a row to the table in README.md.

---

## Table of Contents

- Licensing
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

If you modify an upstream file (C#, YAML, etc.), mark every change with a comment that starts with `SV:`.
This is what makes upstream merge conflicts resolvable, and it is what keeps the edit identifiable as ours inside an MIT file.

- Comment on or right next to the lines you change.
- Do not delete upstream code, comment it out.
- For value changes, write the old and new value: `# SV: 0.25 -> 0.05`.
- For large additions, prefer a partial class or a `_SV/` file over editing the upstream file.
- Fluent (.ftl) files have no inline comments, so put the comment on the line above.

### Commenting Example

YAML, single line:
```
  hidden: false # SV: hidden for the Vox masks
  OverlordLawset: 0.25 # SV: 0.5 -> 0.25
```

YAML, block:
```
  # SV: begin personal trinkets
  - ItemJamesCane
  - ItemAnnabellePlushie
  # SV: end personal trinkets
```

C#, single line:
```
  if (!_actionBlocker.CanSpeak(source, true) && !ignoreActionBlocker) // SV: hypophonia trait
```

C#, block:
```
  // SV: begin additional statuses (ported from CD)
  SecurityStatus.Monitor => "SecurityIconMonitor",
  SecurityStatus.Search => "SecurityIconSearch",
  // SV: end additional statuses
```

Older code uses `// Sector Vestige` or `// SV -` for the same thing. Leave those alone, but use `SV:` for new edits.

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

- Check the fork's own repository for its license before porting. Some forks switched from MIT to AGPL at a specific commit, so older code from them is MIT and newer code is AGPL.
- MIT code: freely portable.
- AGPL code: allowed. Keep it in its `_ForkName/` folder. AGPL requires source disclosure for servers running the code, which we already do.
- We do not use per-file SPDX headers. `REUSE.toml` covers the folder. If ported files come with headers, strip them; if a file's license differs from the rest of its folder, record that file in `REUSE.toml` instead.
- Porting a fork we do not have yet: add a `**/_ForkName/**` table at the end of `REUSE.toml` and a row to the license table in README.md. CI fails if a `_ForkName/` folder has no table.
- Assets: the real license of a sprite or sound is in its `meta.json` or `attributions.yml`. Non-commercial (CC-BY-NC) assets are tracked in `Tools/nc_assets_baseline.txt`; CI fails on a new one unless you add it there on purpose.

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

- SS14 Developer Docs: https://docs.spacestation14.com/
