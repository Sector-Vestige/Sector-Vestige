#!/usr/bin/env python3
"""Check that REUSE resolves the license we intend for every namespaced folder.

`reuse lint` only proves that every file has *some* license and copyright.
It cannot tell whether that license is the right one. This script fills the
gap: it asks the reuse library what it resolves for each file under a
`_Fork/` or `_SV/` folder and compares that with the intent in REUSE.toml.

Rules:
  * every `[[annotations]]` table with a `**/_Name/**` path in REUSE.toml is
    the intended license for that folder;
  * every license expression REUSE finds for a file in that folder (from
    REUSE.toml or from an in-file SPDX header) must contain the intended
    license, so `AGPL-3.0-or-later AND MIT` passes for an AGPL folder and a
    plain `MIT` header in an AGPL folder fails;
  * files under Resources/Textures may resolve to CC-BY-SA-3.0 instead;
  * every `_Name/` folder in the repo needs a table in REUSE.toml.

The main failure this catches is ordering. REUSE uses only the *last*
matching table in REUSE.toml, so a generic rule placed after a fork rule
silently overrides it, and `reuse lint` stays green.

Needs the `reuse` package importable (`pip install reuse`).

Usage:
    python Tools/reuse_check.py            # exit 1 on any problem
    python Tools/reuse_check.py --verbose  # also print per-folder counts
"""

from __future__ import annotations

import re
import sys
import tomllib
from collections import defaultdict
from pathlib import Path

try:
    from reuse.project import Project
except ImportError:
    sys.exit("The 'reuse' package is not importable. Run: pip install reuse")

REPO = Path(__file__).resolve().parent.parent
FORK_TABLE = re.compile(r"^\*\*/(_[A-Za-z0-9]+)/\*\*$")
FORK_IN_PATH = re.compile(r"(?:^|/)(_[A-Za-z0-9]+)/")
TEXTURE_LICENSE = "CC-BY-SA-3.0"


def intended_licenses() -> dict[str, str]:
    with open(REPO / "REUSE.toml", "rb") as fh:
        data = tomllib.load(fh)
    out: dict[str, str] = {}
    for table in data.get("annotations", []):
        paths = table.get("path", [])
        if isinstance(paths, str):
            paths = [paths]
        for p in paths:
            m = FORK_TABLE.match(p)
            if m:
                out[m.group(1)] = table["SPDX-License-Identifier"]
    return out


def license_ids(expression: str) -> set[str]:
    return {part.strip("() ") for part in re.split(r"\s+(?:AND|OR|WITH)\s+", expression)}


def main() -> int:
    verbose = "--verbose" in sys.argv
    intended = intended_licenses()
    project = Project.from_directory(REPO)

    problems: list[str] = []
    counts: dict[str, int] = defaultdict(int)

    for file in project.all_files():
        rel = file.relative_to(REPO).as_posix()
        m = FORK_IN_PATH.search(rel)
        if not m:
            continue
        folder = m.group(1)
        counts[folder] += 1
        want = intended.get(folder)
        if want is None:
            continue  # reported below as a missing table

        for info in project.reuse_info_of(file):
            for expr in info.spdx_expressions:
                expr = str(expr)
                if rel.startswith("Resources/Textures/") and expr == TEXTURE_LICENSE:
                    continue
                if want not in license_ids(expr):
                    source = Path(info.source_path).name if info.source_path else info.source_type
                    problems.append(f"{rel}: {expr!r} from {source}, REUSE.toml intends {want!r} for {folder}/")

    for folder in sorted(counts.keys() - intended.keys()):
        problems.append(f"{folder}/ has {counts[folder]} files but no `**/{folder}/**` table in REUSE.toml")

    if verbose:
        for folder in sorted(counts):
            print(f"{folder:16} {counts[folder]:6} files  intended: {intended.get(folder, '-')}")
        print()

    if problems:
        print(f"{len(problems)} problem(s):")
        for p in problems:
            print("  " + p)
        return 1

    print(f"OK: {sum(counts.values())} files across {len(counts)} namespaced folders match REUSE.toml.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
