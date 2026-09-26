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

Sector Vestige is a fork of [Space Station 14](https://github.com/space-wizards/space-station-14) with its own maps, mechanics and assets. We track upstream closely and keep our own changes in `_SV/` folders so merges stay manageable.

Space Station 14 is a remake of SS13 that runs on [RobustToolbox](https://github.com/space-wizards/RobustToolbox), an open-source C# engine for multiplayer tile-based games.

## Links

Sector Vestige
- Builds: https://cdn.sector-vestige.space:8443/fork/sector-vestige

Space Station 14
- Website: https://spacestation14.io/
- Standalone download: https://spacestation14.io/about/nightlies/
- Steam: https://store.steampowered.com/app/1255460/Space_Station_14/
- Developer docs: https://docs.spacestation14.com/

## Building

You need the .NET 10 SDK (see `global.json`) and Python 3.5 or newer.

```bash
git clone https://github.com/Sector-Vestige/Sector-Vestige.git
cd Sector-Vestige

# Fetches the RobustToolbox submodule
python RUN_THIS.py

dotnet build

# Run the server and client in two separate terminals
dotnet run --project Content.Server
dotnet run --project Content.Client
```

The [SS14 setup guide](https://docs.spacestation14.com/en/general-development/setup.html) covers IDE configuration and the rest.

## Contributing

Read [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request. It covers how we handle licensing, porting from other forks and map changes.

## License

### Code

- Original Sector Vestige code (in `_SV/` folders) is licensed under AGPL-3.0-or-later.
- Upstream Space Station 14 code stays under the MIT License. Our edits to upstream files are marked with `SV:` comments and stay MIT, so they can be sent back upstream. See [CONTRIBUTING.md](CONTRIBUTING.md).
- `REUSE.toml` records which folder has which license. We do not use per-file SPDX headers.

### Ported code

Code ported from other forks lives in its own folder and keeps its original license:

| Fork | Folder | License | Repository |
|------|--------|---------|------------|
| LateStation | `_LateStation/` | AGPL-3.0-or-later | [GitHub](https://github.com/LateStation14/Late-station-14) |
| Axolotl MRP | `_AXOLOTL/` | MIT | [GitHub](https://github.com/Axolotl-MRP/axolotl-mrp-14) |
| Cosmatic Drift | `_CD/` | MIT | [GitHub](https://github.com/cosmatic-drift-14/cosmatic-drift) |
| Delta-V | `_DV/` | AGPL-3.0-or-later | [GitHub](https://github.com/DeltaV-Station/Delta-v) |
| Frontier | `_NF/` | AGPL-3.0-or-later | [GitHub](https://github.com/new-frontiers-14/frontier-station-14) |
| Goob | `_Goobstation/` | AGPL-3.0-or-later | [GitHub](https://github.com/Goob-Station/Goob-Station) |
| Harmony | `_Harmony/` | AGPL-3.0-or-later | [GitHub](https://github.com/ss14-harmony/ss14-harmony) |
| Umbra | `_Umbra/` | MIT | [GitHub](https://github.com/Sector-Umbra/Sector-Umbra) |
| FloofStation | `_Floofstation/` | AGPL-3.0-or-later | [GitHub](https://github.com/Floof-Station/Floof-Station) |
| Impstation | `_Impstation/` | AGPL-3.0-or-later | [GitHub](https://github.com/impstation/imp-station-14) |
| Einstein Engines | `_EE/` | AGPL-3.0-or-later | [GitHub](https://github.com/Simple-Station/Einstein-Engines) |
| Funkystation | `_Funkystation/` | AGPL-3.0-or-later | [GitHub](https://github.com/funky-station/funky-station) |
| Moffstation | `_Moffstation/` | MIT | [GitHub](https://github.com/moff-station/moff-station-14) |
| RMC14 | `_RMC14/` | MIT | [GitHub](https://github.com/RMC-14/RMC-14) |

`REUSE.toml` has the full per-folder mapping. CI runs `reuse lint` plus `Tools/reuse_check.py`, which verifies that the license REUSE computes for every `_SV/` and fork file is the one intended.

### Assets

Most textures, sprites and audio are licensed under [CC BY-SA 3.0](https://creativecommons.org/licenses/by-sa/3.0/). Every asset folder has a `meta.json` with the author and license, for example `Resources/Textures/_SV/Tools/rpd.rsi/meta.json`.

Around a hundred asset folders use [CC BY-NC-SA 3.0](https://creativecommons.org/licenses/by-nc-sa/3.0/) instead. Those cannot be used commercially. `Tools/check_nc_assets.py` lists them, and CI fails when a new one is added without being recorded in `Tools/nc_assets_baseline.txt`.

See the [REUSE specification](https://reuse.software/) and `REUSE.toml` for the full licensing details.
