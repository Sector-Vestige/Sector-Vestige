<p align="center">
  <img alt="Sector Vestige" src="https://raw.githubusercontent.com/Sector-Vestige/Sector-Vestige/refs/heads/master/Resources/Textures/Logo/logo.png" />
</p>

# Sector Vestige

[![REUSE status](https://api.reuse.software/badge/github.com/Sector-Vestige/Sector-Vestige)](https://api.reuse.software/info/github.com/Sector-Vestige/Sector-Vestige)
[![Build & Test Debug](https://github.com/Sector-Vestige/Sector-Vestige/actions/workflows/build-test-debug.yml/badge.svg?branch=master)](https://github.com/Sector-Vestige/Sector-Vestige/actions/workflows/build-test-debug.yml)
[![YAML Linter](https://github.com/Sector-Vestige/Sector-Vestige/actions/workflows/yaml-linter.yml/badge.svg?branch=master)](https://github.com/Sector-Vestige/Sector-Vestige/actions/workflows/yaml-linter.yml)
[![YAML map schema validator](https://github.com/Sector-Vestige/Sector-Vestige/actions/workflows/validate_mapfiles.yml/badge.svg?branch=master)](https://github.com/Sector-Vestige/Sector-Vestige/actions/workflows/validate_mapfiles.yml)
[![Validate RSIs](https://github.com/Sector-Vestige/Sector-Vestige/actions/workflows/validate-rsis.yml/badge.svg?branch=master)](https://github.com/Sector-Vestige/Sector-Vestige/actions/workflows/validate-rsis.yml)
[![YAML RGA schema validator](https://github.com/Sector-Vestige/Sector-Vestige/actions/workflows/validate-rgas.yml/badge.svg?branch=master)](https://github.com/Sector-Vestige/Sector-Vestige/actions/workflows/validate-rgas.yml)

**Sector Vestige** is a custom-content fork of [Space Station 14](https://github.com/space-wizards/space-station-14), building its own unique gameplay experience with original mechanics, stations, and assets while staying aligned with the upstream SS14 codebase.

We actively develop new systems, balance changes, and exclusive content to support our long-term vision. While much remains familiar to upstream players, Sector Vestige is steadily establishing its own identity on the SS14 platform.

**Space Station 14** is a remake of SS13, powered by [RobustToolbox](https://github.com/space-wizards/RobustToolbox) — an open-source C# engine purpose-built for multiplayer, tile-based simulation games.

---

## 🔗 Links

**Sector Vestige**
- 🌐 Website: *Coming Soon*
- 📁 Builds: https://cdn.sector-vestige.space:8443/fork/sector-vestige

**Space Station 14**
- 🌐 Website: https://spacestation14.io/
- 🛠️ Standalone Download: https://spacestation14.io/about/nightlies/
- 🎮 Steam: https://store.steampowered.com/app/1255460/Space_Station_14/

---

## 📚 Documentation

- **SS14 Docs**: https://docs.spacestation14.io/ — comprehensive guide covering engine, content, mapping, and modding
- **Contributing**: See [CONTRIBUTING.md](CONTRIBUTING.md) — **read this first** if you want to contribute!

---

## 🤝 Contributing

We welcome all contributions — code, sprites, maps, balance suggestions, and feedback!

**Getting Started:**
1. Read [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines
2. Check existing issues or open a new one
3. Submit a pull request

---

## 🧱 Building the Project

**Quick Start:**

```bash
# Clone the repository
git clone https://github.com/Sector-Vestige/Sector-Vestige.git
cd Sector-Vestige

# Run setup script
python RUN_THIS.py

# Build the solution
dotnet build
```

**For detailed setup:** See the [SS14 Developer Docs](https://docs.spacestation14.com/en/general-development/setup.html) for IDE configuration and advanced build options.

---

## ⚖️ License

### Code

**Sector Vestige Original Code**
- All original Sector Vestige code (in `_SV/` folders) is licensed under **AGPL-3.0-or-later**.
- Contributors agree to license their contributions under AGPL-3.0-or-later when submitting code (see [CONTRIBUTING.md](CONTRIBUTING.md)).

**Upstream Code**
- Code from [Space Station 14](https://github.com/space-wizards/space-station-14) remains under the **MIT License**.
- Modified upstream files include comments to clarify changes and maintain attribution.

**SPDX Headers**
- SPDX license headers are automatically managed by GitHub Actions to ensure clear licensing.
- Files without headers are covered by the project-wide license policy defined in `.reuse/dep5`.

### Ported Code

Sector Vestige includes code ported from other SS14 forks, organized in namespaced folders:

| Fork | Folder | License | Repository |
|------|--------|---------|------------|
| **LateStation** | `_LateStation/` | AGPL-3.0-or-later | [Github](https://github.com/LateStation14/Late-station-14) |
| **Axolotl MRP** | `_AXOLOTL/` | MIT | [GitHub](https://github.com/Axolotl-MRP/axolotl-mrp-14) |
| **Cosmatic Drift** | `_CD/` | MIT | [GitHub](https://github.com/cosmatic-drift-14/cosmatic-drift) |
| **Delta-V** | `_DV/` | AGPL-3.0-or-later | [GitHub](https://github.com/DeltaV-Station/Delta-v) |
| **Frontier** | `_NF/` | AGPL-3.0-or-later | [GitHub](https://github.com/new-frontiers-14/frontier-station-14) |
| **Harmony** | `_Harmony/` | AGPL-3.0-or-later | [GitHub](https://github.com/ss14-harmony/ss14-harmony) |
| **Umbra** | `_Umbra/` | MIT | [GitHub](https://github.com/Sector-Umbra/Sector-Umbra) |
| **FloofStation** | `_Floofstation/` | AGPL-3.0-or-later | [GitHub](https://github.com/Simple-Station/Einstein-Engines) |
| **Impstation** | `_Impstation/` | AGPL-3.0-or-later | [GitHub](https://github.com/impstation/imp-station-14) |
| **Einstein Engines** | `_EE/` | AGPL-3.0-or-later | [GitHub](https://github.com/Simple-Station/Einstein-Engines) |
| **Funkystation** | `_Funkystation/` | AGPL-3.0-or-later | [GitHub](https://github.com/funky-station/funky-station) |
| **Moffstation** | `_Moffstation/` | MIT | [GitHub](https://github.com/moff-station/moff-station-14) |

- Ported code retains its **original license** as specified above.
- All modifications by Sector Vestige contributors are documented in SPDX headers.
- See `.reuse/dep5` for complete licensing details.

### Assets

**Textures, Sprites, and Audio**
- Most assets are licensed under **CC BY-SA 3.0**: https://creativecommons.org/licenses/by-sa/3.0/
  - Requires attribution
  - Requires derivative works to use the same license (share-alike)

- Each asset folder contains a `meta.json` file defining authorship and license.
  - Example: `Resources/Textures/hop_turtle.rsi/meta.json`

⚠️ **Non-Commercial Assets**
- Some assets use **CC BY-NC-SA 3.0** (non-commercial): https://creativecommons.org/licenses/by-nc-sa/3.0/
- These **cannot be used in commercial projects**.
- Review `meta.json` files and replace non-commercial assets if commercial use is intended.

---

For detailed licensing information, see the [REUSE specification](https://reuse.software/) and our `.reuse/dep5` file.
