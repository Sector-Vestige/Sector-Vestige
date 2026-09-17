#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
#
# SPDX-License-Identifier: AGPL-3.0-or-later

"""
Resize Content.MapRenderer output into lobby map-pool preview images.

Previews are 288x192 RGBA PNGs. The source image is scaled to fit inside that
box (aspect ratio preserved) and centred on a transparent canvas, which matches
how the existing previews in Resources/Textures/_SV/MapPreviews were made.

Typical workflow:

    # Render with Content.MapRenderer, then resize. Map names are matched
    # case-insensitively against gameMap prototype IDs.
    python3 Tools/_SV/resize_map_preview.py --render amber

    # Same, for several maps at once
    python3 Tools/_SV/resize_map_preview.py --render amber bagel cluster

Without --render the script only resizes an existing render, so you can also
run MapRenderer yourself (writes Resources/MapImages/<Id>/<Id>-0.png):

    dotnet run --project Content.MapRenderer -- Amber
    python3 Tools/_SV/resize_map_preview.py amber

Other forms:

    # explicit input file, output name derived from the file name
    python3 Tools/_SV/resize_map_preview.py Resources/MapImages/amber/amber-0.png

    # explicit input and output
    python3 Tools/_SV/resize_map_preview.py render.png -o Resources/Textures/_SV/MapPreviews/amber.png

    # several maps at once
    python3 Tools/_SV/resize_map_preview.py amber bagel cluster

    # crop the transparent border MapRenderer leaves around the grid first
    python3 Tools/_SV/resize_map_preview.py amber --trim

    # render with a different build configuration (default: Release)
    python3 Tools/_SV/resize_map_preview.py --render --configuration Debug amber

Requires Pillow (pip install pillow). --render additionally needs the dotnet
SDK and a checked-out RobustToolbox submodule; the first render builds
Content.MapRenderer, which takes a while.
"""

import argparse
import re
import subprocess
import sys
from pathlib import Path

try:
    from PIL import Image
except ImportError:
    sys.exit("This script needs Pillow: pip install pillow")

REPO_ROOT = Path(__file__).resolve().parents[2]
MAP_IMAGES_DIR = REPO_ROOT / "Resources" / "MapImages"
PREVIEW_DIR = REPO_ROOT / "Resources" / "Textures" / "_SV" / "MapPreviews"

PROTOTYPE_DIR = REPO_ROOT / "Resources" / "Prototypes"
MAP_RENDERER_PROJECT = REPO_ROOT / "Content.MapRenderer"

PREVIEW_WIDTH = 288
PREVIEW_HEIGHT = 192

_GAME_MAP_ID_RE = re.compile(r"^-\s*type:\s*gameMap\s*$\n(?:^[ \t]+.*\n)*?^[ \t]+id:\s*['\"]?([^'\"\s#]+)", re.MULTILINE)


def game_map_ids() -> dict[str, str]:
    """Map lowercased gameMap prototype ID -> real ID, scanned from Resources/Prototypes."""
    ids: dict[str, str] = {}
    for yml in PROTOTYPE_DIR.rglob("*.yml"):
        try:
            text = yml.read_text(encoding="utf-8")
        except OSError:
            continue
        if "gameMap" not in text:
            continue
        for match in _GAME_MAP_ID_RE.finditer(text):
            ids.setdefault(match.group(1).lower(), match.group(1))
    return ids


def resolve_map_id(name: str, known: dict[str, str]) -> str:
    """Turn a user-supplied map name into the exact gameMap prototype ID."""
    try:
        return known[name.lower()]
    except KeyError:
        raise LookupError(
            f"No gameMap prototype called '{name}'. Known maps: {', '.join(sorted(known.values()))}"
        ) from None


def render_maps(map_ids: list[str], configuration: str) -> None:
    """Run Content.MapRenderer for the given prototype IDs, writing to Resources/MapImages."""
    cmd = [
        "dotnet", "run",
        "--project", str(MAP_RENDERER_PROJECT),
        "--configuration", configuration,
        "--",
        "-o", str(MAP_IMAGES_DIR),
        *map_ids,
    ]
    print("Rendering:", " ".join(cmd), flush=True)
    result = subprocess.run(cmd, cwd=REPO_ROOT)
    if result.returncode != 0:
        raise RuntimeError(f"Content.MapRenderer exited with code {result.returncode}")


def find_render(name: str) -> Path:
    """Locate a MapRenderer output for a map name (case-insensitive)."""
    for directory in MAP_IMAGES_DIR.iterdir() if MAP_IMAGES_DIR.is_dir() else []:
        if directory.is_dir() and directory.name.lower() == name.lower():
            candidates = sorted(
                p for p in directory.iterdir() if p.suffix.lower() in (".png", ".webp")
            )
            if candidates:
                return candidates[0]
    raise FileNotFoundError(
        f"No render found for '{name}' under {MAP_IMAGES_DIR}. "
        f"Run Content.MapRenderer first, or pass a path to an image file."
    )


def output_name_for(source: Path) -> str:
    """amber-0.png -> amber.png (strip the MapRenderer grid index suffix)."""
    stem = source.stem
    head, sep, tail = stem.rpartition("-")
    if sep and tail.isdigit():
        stem = head
    return stem.lower() + ".png"


def resize(source: Path, dest: Path, trim: bool, width: int, height: int) -> None:
    with Image.open(source) as im:
        im = im.convert("RGBA")
        if trim:
            bbox = im.getchannel("A").getbbox()
            if bbox:
                im = im.crop(bbox)

        scale = min(width / im.width, height / im.height)
        new_size = (max(1, round(im.width * scale)), max(1, round(im.height * scale)))
        scaled = im.resize(new_size, Image.Resampling.LANCZOS)

    canvas = Image.new("RGBA", (width, height), (0, 0, 0, 0))
    offset = ((width - scaled.width) // 2, (height - scaled.height) // 2)
    canvas.paste(scaled, offset)  # no mask: canvas is fully transparent, so copy RGBA as-is

    dest.parent.mkdir(parents=True, exist_ok=True)
    canvas.save(dest, "PNG", optimize=True)
    print(f"{source} -> {dest} ({scaled.width}x{scaled.height} inside {width}x{height})")


def main() -> int:
    parser = argparse.ArgumentParser(
        description="Resize MapRenderer output into 288x192 lobby preview images.",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog=__doc__.split("Typical workflow:", 1)[1],
    )
    parser.add_argument(
        "inputs",
        nargs="+",
        help="map names (looked up in Resources/MapImages) or paths to image files",
    )
    parser.add_argument(
        "-o", "--output",
        type=Path,
        help="output file (single input) or directory. Defaults to Resources/Textures/_SV/MapPreviews/<name>.png",
    )
    parser.add_argument(
        "--trim",
        action="store_true",
        help="crop fully transparent borders from the source before scaling",
    )
    parser.add_argument(
        "--render",
        action="store_true",
        help="run Content.MapRenderer for the given map names first (inputs must be map names, not files)",
    )
    parser.add_argument(
        "--configuration",
        default="Release",
        help="dotnet build configuration used with --render (default: Release)",
    )
    parser.add_argument("--width", type=int, default=PREVIEW_WIDTH, help=f"default {PREVIEW_WIDTH}")
    parser.add_argument("--height", type=int, default=PREVIEW_HEIGHT, help=f"default {PREVIEW_HEIGHT}")
    args = parser.parse_args()

    if args.output and len(args.inputs) > 1 and args.output.suffix:
        parser.error("--output must be a directory when resizing more than one input")

    if args.render:
        known = game_map_ids()
        try:
            map_ids = [resolve_map_id(name, known) for name in args.inputs]
        except LookupError as e:
            print(f"error: {e}", file=sys.stderr)
            return 1
        try:
            render_maps(map_ids, args.configuration)
        except (RuntimeError, OSError) as e:
            print(f"error: {e}", file=sys.stderr)
            return 1
        # Look up the fresh renders by their exact prototype IDs.
        args.inputs = map_ids

    sources = []
    for item in args.inputs:
        path = Path(item)
        try:
            sources.append(path if path.is_file() else find_render(item))
        except FileNotFoundError as e:
            print(f"error: {e}", file=sys.stderr)
            return 1

    for source in sources:
        if args.output and args.output.suffix:
            dest = args.output
        else:
            dest = (args.output or PREVIEW_DIR) / output_name_for(source)
        resize(source, dest, args.trim, args.width, args.height)

    return 0


if __name__ == "__main__":
    sys.exit(main())
