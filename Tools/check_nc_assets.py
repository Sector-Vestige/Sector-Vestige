#!/usr/bin/env python3
"""List assets whose meta.json or attributions.yml declares a non-commercial license.

REUSE.toml declares Resources/Textures as CC-BY-SA-3.0 as a whole, so the
REUSE badge stays green even when an individual asset folder is CC-BY-NC-SA.
The real license lives in each folder's meta.json (RSI sprites) or
attributions.yml (audio and loose images). This script reads those.

Usage:
    python Tools/check_nc_assets.py                      # print every NC asset
    python Tools/check_nc_assets.py --write-baseline     # record the current set
    python Tools/check_nc_assets.py --baseline           # fail only on NC assets
                                                         # that are not in the baseline

The baseline lives in Tools/nc_assets_baseline.txt. CI runs with --baseline so
existing NC assets do not block anything, but no new ones can be merged
without someone consciously adding them to the baseline.
"""

from __future__ import annotations

import json
import re
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parent.parent
RESOURCES = REPO / "Resources"
BASELINE = REPO / "Tools" / "nc_assets_baseline.txt"
NON_COMMERCIAL = re.compile(r"\bNC\b|non-?commercial", re.IGNORECASE)


def scan() -> dict[str, str]:
    """Return {relative path: license string} for every NC asset declaration."""
    found: dict[str, str] = {}

    for meta in RESOURCES.rglob("meta.json"):
        try:
            license_ = json.loads(meta.read_text(encoding="utf-8")).get("license", "")
        except (json.JSONDecodeError, UnicodeDecodeError):
            continue
        if isinstance(license_, str) and NON_COMMERCIAL.search(license_):
            found[meta.parent.relative_to(REPO).as_posix()] = license_

    # attributions.yml is a flat list of {files, license, copyright, source}.
    # A regex is enough here and avoids a YAML dependency.
    for attr in RESOURCES.rglob("attributions.yml"):
        text = attr.read_text(encoding="utf-8", errors="replace")
        for m in re.finditer(r"^\s*license:\s*[\"']?([^\"'\n]+)", text, re.MULTILINE):
            if NON_COMMERCIAL.search(m.group(1)):
                found[attr.relative_to(REPO).as_posix()] = m.group(1).strip()
                break

    return found


def main() -> int:
    args = sys.argv[1:]
    found = scan()

    if "--write-baseline" in args:
        BASELINE.write_text("".join(f"{p}\n" for p in sorted(found)), encoding="utf-8")
        print(f"Wrote {len(found)} entries to {BASELINE.relative_to(REPO)}")
        return 0

    if "--baseline" in args:
        known = set(BASELINE.read_text(encoding="utf-8").split()) if BASELINE.exists() else set()
        new = {p: l for p, l in found.items() if p not in known}
        gone = sorted(known - found.keys())
        if gone:
            print(f"{len(gone)} baseline entries no longer exist (run --write-baseline to prune):")
            for p in gone:
                print("  " + p)
        if new:
            print(f"{len(new)} new non-commercial asset(s) not in the baseline:")
            for p, l in sorted(new.items()):
                print(f"  {p}  ({l})")
            print("\nEither replace the asset with a commercially usable one, or add it to "
                  f"{BASELINE.relative_to(REPO)} on purpose.")
            return 1
        print(f"OK: {len(found)} non-commercial assets, all in the baseline.")
        return 0

    for p, l in sorted(found.items()):
        print(f"{p}  ({l})")
    print(f"\n{len(found)} non-commercial asset declarations.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
