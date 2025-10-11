# SPDX-FileCopyrightText: 2025 Sector-Vestige contributors
# SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
# SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
# SPDX-FileCopyrightText: 2025 sleepyyapril <flyingkarii@gmail.com>
# SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
#
# SPDX-License-Identifier: MIT

#!/usr/bin/env python3

import subprocess
import os
import sys
import re
import fnmatch
import argparse
import json
from datetime import datetime, timezone
from collections import defaultdict
import re as _re

# Optional TOML support for REUSE.toml
try:
    import tomllib as _tomllib  # Python 3.11+
except Exception:  # pragma: no cover - fallback if running locally on <3.11
    try:
        import tomli as _tomllib  # type: ignore
    except Exception:
        _tomllib = None

# --- Configuration ---
LICENSE_CONFIG = {
    "mit": {"id": "MIT", "path": "LICENSES/MIT.txt"},
    "agpl": {"id": "AGPL-3.0-or-later", "path": "LICENSES/AGPL-3.0-or-later.txt"},
    "mpl": {"id": "MPL-2.0", "path": "LICENSES/MPL-2.0.txt"},
}

# Default fallback license for files that do not match any explicit pattern.
# Core LateStation code (Content.* except special upstream forks) is MIT, so default to MIT.
DEFAULT_LICENSE_LABEL = "mit"

# Dictionary mapping file extensions to comment styles
# Format: {extension: (prefix, suffix)}
# If suffix is None, it's a single-line comment style
COMMENT_STYLES = {
    # C-style single-line comments
    ".cs": ("//", None),
    ".js": ("//", None),
    ".ts": ("//", None),
    ".jsx": ("//", None),
    ".tsx": ("//", None),
    ".c": ("//", None),
    ".cpp": ("//", None),
    ".cc": ("//", None),
    ".h": ("//", None),
    ".hpp": ("//", None),
    ".java": ("//", None),
    ".scala": ("//", None),
    ".kt": ("//", None),
    ".swift": ("//", None),
    ".go": ("//", None),
    ".rs": ("//", None),
    ".dart": ("//", None),
    ".groovy": ("//", None),
    ".php": ("//", None),

    # Hash-style single-line comments
    ".yaml": ("#", None),
    ".yml": ("#", None),
    ".ftl": ("#", None),
    ".py": ("#", None),
    ".rb": ("#", None),
    ".pl": ("#", None),
    ".pm": ("#", None),
    ".sh": ("#", None),
    ".bash": ("#", None),
    ".zsh": ("#", None),
    ".fish": ("#", None),
    ".ps1": ("#", None),
    ".r": ("#", None),
    ".rmd": ("#", None),
    ".jl": ("#", None),  # Julia
    ".tcl": ("#", None),
    ".perl": ("#", None),
    ".conf": ("#", None),
    ".toml": ("#", None),
    ".ini": ("#", None),
    ".cfg": ("#", None),
    ".gitignore": ("#", None),
    ".dockerignore": ("#", None),

    # Other single-line comment styles
    ".bat": ("REM", None),
    ".cmd": ("REM", None),
    ".vb": ("'", None),
    ".vbs": ("'", None),
    ".bas": ("'", None),
    ".asm": (";", None),
    ".s": (";", None),  # Assembly
    ".lisp": (";", None),
    ".clj": (";", None),  # Clojure
    ".f": ("!", None),   # Fortran
    ".f90": ("!", None), # Fortran
    ".m": ("%", None),   # MATLAB/Octave
    ".sql": ("--", None),
    ".ada": ("--", None),
    ".adb": ("--", None),
    ".ads": ("--", None),
    ".hs": ("--", None), # Haskell
    ".lhs": ("--", None),
    ".lua": ("--", None),

    # Multi-line comment styles
    ".xaml": ("<!--", "-->"),
    ".xml": ("<!--", "-->"),
    ".html": ("<!--", "-->"),
    ".htm": ("<!--", "-->"),
    ".svg": ("<!--", "-->"),
    ".css": ("/*", "*/"),
    ".scss": ("/*", "*/"),
    ".sass": ("/*", "*/"),
    ".less": ("/*", "*/"),
    ".md": ("<!--", "-->"),
    ".markdown": ("<!--", "-->"),
}
REPO_PATH = "."

TOKENS = ("github_pat_", "ghp_", "gho_", "ghs_", "ghu_")

def _contains_token(text: str | None) -> bool:
    if not text:
        return False
    t = str(text)
    return any(tok in t for tok in TOKENS)

def is_token(text: str) -> bool:
    # Treat any occurrence of token-like prefixes or monolith user as a bot/token
    t = text or ""
    return _contains_token(t) or t.lower().startswith("monolith")

def is_bot_name(name: str | None) -> bool:
    if not name:
        return False
    n = name.strip().lower()
    return (
        n.endswith("[bot]")
        or n.endswith("-bot")
        or " bot" in n
        or n.startswith("bot ")
        or n == "bot"
        or "github-actions" in n
        or "dependabot" in n
        or "weh-bot" in n
        or "vestige-bot" in n
    )

def _parse_dep5_file(dep5_path: str = ".reuse/dep5") -> list[dict]:
    """
    Parse a .reuse/dep5 file and extract copyright information for file patterns.
    Returns a list of dicts with keys: 'patterns', 'copyrights', 'license'
    """
    if not os.path.exists(dep5_path):
        return []

    entries = []
    current_entry = None
    in_copyright_section = False

    try:
        with open(dep5_path, 'r', encoding='utf-8') as f:
            for line in f:
                line = line.rstrip('\n')

                # Skip comments and empty lines
                if not line.strip() or line.strip().startswith('#'):
                    continue

                # New "Files:" section starts a new entry
                if line.startswith('Files:'):
                    if current_entry:
                        entries.append(current_entry)
                    current_entry = {
                        'patterns': [],
                        'copyrights': [],
                        'license': None
                    }
                    in_copyright_section = False
                    # Extract patterns from the same line
                    patterns_str = line[6:].strip()
                    if patterns_str:
                        current_entry['patterns'].extend([p.strip() for p in patterns_str.split() if p.strip()])

                # Copyright line starts copyright section
                elif line.startswith('Copyright:'):
                    if current_entry:
                        copyright_text = line[10:].strip()
                        if copyright_text:
                            current_entry['copyrights'].append(copyright_text)
                        in_copyright_section = True

                # License line ends copyright section
                elif line.startswith('License:'):
                    if current_entry:
                        current_entry['license'] = line[8:].strip()
                    in_copyright_section = False

                # Indented lines - could be pattern or copyright continuation
                elif current_entry and line.startswith((' ', '\t')):
                    stripped = line.strip()
                    if stripped.startswith('License:'):
                        current_entry['license'] = stripped[8:].strip()
                        in_copyright_section = False
                    elif in_copyright_section:
                        # This is a copyright continuation
                        if stripped:
                            current_entry['copyrights'].append(stripped)
                    else:
                        # This is a pattern continuation
                        patterns = [p.strip() for p in line.split() if p.strip()]
                        current_entry['patterns'].extend(patterns)

        # Add the last entry
        if current_entry:
            entries.append(current_entry)

    except Exception as e:
        print(f"Warning: Failed to parse dep5 file: {e}", file=sys.stderr)
        return []

    return entries

def _get_upstream_copyright_from_dep5(file_path: str) -> list[str]:
    """
    Get the upstream copyright holder(s) from dep5 file for a given file path.
    Returns a list of copyright holders (extracted from copyright lines), or empty list.
    Prefers the most specific pattern match.
    """
    dep5_entries = _parse_dep5_file()
    if not dep5_entries:
        return []

    # Normalize the file path
    normalized_path = file_path.replace('\\', '/').lstrip('./')

    # Find matching entries (prefer most specific pattern)
    matches = []
    for entry in dep5_entries:
        for pattern in entry['patterns']:
            # Convert dep5 glob pattern to fnmatch pattern
            pattern = pattern.replace('\\', '/')
            if fnmatch.fnmatch(normalized_path, pattern):
                # Use pattern specificity: longer pattern = more specific
                # Also count directory depth (more slashes = more specific)
                specificity = len(pattern) + pattern.count('/') * 10
                matches.append((specificity, entry))
                break

    if not matches:
        return []

    # Get the most specific match (highest specificity score)
    _, best_entry = max(matches, key=lambda x: x[0])

    # Extract all copyright holders from the copyright lines
    copyright_holders = []
    if best_entry['copyrights']:
        for copyright_line in best_entry['copyrights']:
            # Extract just the copyright holder part (after the year range)
            # Format is typically: "2020-2025 Upstream Name contributors"
            # We want to extract "Upstream Name contributors"
            import re
            match = re.search(r'\d{4}(?:-\d{4})?\s+(.+)$', copyright_line)
            if match:
                holder = match.group(1).strip()
                # Only add unique copyright holders
                if holder not in copyright_holders:
                    copyright_holders.append(holder)

    return copyright_holders

# Project name used in fallback copyright text
DEFAULT_PROJECT_NAME = (
    os.environ.get("REUSE_PROJECT_NAME")
    or os.environ.get("GITHUB_REPOSITORY", "").split("/")[-1]
    or "Project"
)

def run_git_command(command, cwd=REPO_PATH, check=True):
    """Runs a git command and returns its output."""
    try:
        result = subprocess.run(
            command,
            capture_output=True,
            text=True,
            check=check,
            cwd=cwd,
            encoding='utf-8',
            errors='ignore'
        )
        return result.stdout.strip()
    except subprocess.CalledProcessError as e:
        if check:
            print(f"Error running git command {' '.join(command)}: {e.stderr}", file=sys.stderr)
        return None
    except FileNotFoundError:
        print("FATAL: 'git' command not found. Make sure git is installed and in your PATH.", file=sys.stderr)
        return None

def get_authors_from_git(file_path, cwd=REPO_PATH, pr_base_sha=None, pr_head_sha=None):
    """
    Gets authors and their contribution years for a specific file.
    If pr_base_sha and pr_head_sha are provided, also includes authors from the PR's commits.
    Returns: dict like {"Author Name <email>": (min_year, max_year)}
    """
    author_timestamps = defaultdict(list)

    # Get authors from the PR's commits if base and head SHAs are provided
    if pr_base_sha and pr_head_sha:
        print(f"Getting authors from PR commits for {file_path}")
        print(f"PR base SHA: {pr_base_sha}")
        print(f"PR head SHA: {pr_head_sha}")

        # First, let's log all commits in the PR
        all_commits_command = ["git", "log", f"{pr_base_sha}..{pr_head_sha}", "--pretty=format:%H|%an|%ae", "--", file_path]
        print(f"Running command: {' '.join(all_commits_command)}")
        all_commits_output = run_git_command(all_commits_command, cwd=cwd, check=False)

        if all_commits_output:
            print(f"Commits found in PR for {file_path}:")
            for line in all_commits_output.splitlines():
                print(f"  {line}")
        else:
            print(f"No commits found in PR for {file_path}")

        # Now get the authors with timestamps
        pr_command = ["git", "log", f"{pr_base_sha}..{pr_head_sha}", "--pretty=format:%H|%at|%an|%ae|%b", "--", file_path]
        print(f"Running command: {' '.join(pr_command)}")
        pr_output = run_git_command(pr_command, cwd=cwd, check=False)

        if pr_output:
            # Process PR authors
            print(f"Raw PR output for {file_path}:")
            for line in pr_output.splitlines():
                print(f"  {line}")

            process_git_log_output(pr_output, author_timestamps)
            print(f"Found {len(author_timestamps)} authors in PR commits for {file_path}")

            # Print the authors found
            print(f"Authors found in PR commits for {file_path}:")
            for author, timestamps in author_timestamps.items():
                print(f"  {author}: {timestamps}")
        else:
            print(f"No PR output found for {file_path}")

    # Get all historical authors
    print(f"Getting historical authors for {file_path}")
    command = ["git", "log", "--pretty=format:%H|%at|%an|%ae|%b", "--follow", "--", file_path]
    print(f"Running command: {' '.join(command)}")
    output = run_git_command(command, cwd=cwd, check=False)

    if output:
        # Process historical authors
        print(f"Processing historical authors for {file_path}")
        process_git_log_output(output, author_timestamps)

        # Print the authors found
        print(f"All authors found for {file_path} (after adding historical):")
        for author, timestamps in author_timestamps.items():
            print(f"  {author}: {timestamps}")
    else:
        print(f"No historical output found for {file_path}")

    if not author_timestamps:
        # Try to get the current user from git config as a fallback
        try:
            name_cmd = ["git", "config", "user.name"]
            email_cmd = ["git", "config", "user.email"]
            user_name = run_git_command(name_cmd, cwd=cwd, check=False)
            user_email = run_git_command(email_cmd, cwd=cwd, check=False)

            # Use current year
            current_year = datetime.now(timezone.utc).year
            if user_name and user_email and user_name.strip() != "Unknown" and not user_name.startswith("monolith"):
                return {f"{user_name} <{user_email}>": (current_year, current_year)}
            else:
                print("Warning: Could not get current user from git config or name is 'Unknown'")
                return {}
        except Exception as e:
            print(f"Error getting git user: {e}")
        return {}

    # Convert timestamps to years
    author_years = {}
    for author, timestamps in author_timestamps.items():
        if not timestamps:
            continue
        min_ts = min(timestamps)
        max_ts = max(timestamps)
        min_year = datetime.fromtimestamp(min_ts, timezone.utc).year
        max_year = datetime.fromtimestamp(max_ts, timezone.utc).year
        author_years[author] = (min_year, max_year)

    return author_years

def get_last_editor(file_path: str, cwd: str = REPO_PATH) -> str | None:
    """Return the most recent non-bot author "Name <email>" for the file, if any."""
    cmd = [
        "git", "log", "-1", "--pretty=format:%an <%ae>", "--", file_path
    ]
    out = run_git_command(cmd, cwd=cwd, check=False)
    if out and out.strip() and not is_bot_name(out) and not _contains_token(out):
        return out.strip()
    return None

def process_git_log_output(output, author_timestamps):
    """
    Process git log output and add authors to author_timestamps.
    """
    co_author_regex = re.compile(r"^Co-authored-by:\s*(.*?)\s*<([^>]+)>", re.MULTILINE)

    for line in output.splitlines():
        if not line.strip():
            continue

        parts = line.split('|', 4)
        if len(parts) < 5:
            print(f"Skipping malformed line: {line}")
            continue

        commit_hash, timestamp_str, author_name, author_email, body = parts
        print(f"Processing commit {commit_hash[:8]} by {author_name} <{author_email}>")

        try:
            timestamp = int(timestamp_str)
        except ValueError:
            continue

        # Add main author
        has_token = _contains_token(author_name) or _contains_token(author_email)
        if (
            author_name
            and author_email
            and author_name.strip() != "Unknown"
            and not has_token
            and not is_bot_name(author_name)
        ):
            author_key = f"{author_name.strip()} <{author_email.strip()}>"
            author_timestamps[author_key].append(timestamp)

        # Add co-authors
        for match in co_author_regex.finditer(body):
            co_author_name = match.group(1).strip()
            co_author_email = match.group(2).strip()
            has_token = _contains_token(co_author_name) or _contains_token(co_author_email)

            if (
                co_author_name
                and co_author_email
                and co_author_name.strip() != "Unknown"
                and not has_token
                and not is_bot_name(co_author_name)
            ):
                co_author_key = f"{co_author_name} <{co_author_email}>"
                author_timestamps[co_author_key].append(timestamp)

    # No need to convert timestamps to years here, it's done in get_authors_from_git

def parse_existing_header(content, comment_style):
    """
    Parses an existing REUSE header to extract authors and license.
    Returns: (authors_dict, license_id, header_lines)

    comment_style is a tuple of (prefix, suffix)
    """
    prefix, suffix = comment_style
    lines = content.splitlines()
    authors = {}
    license_id = None
    header_lines = []

    if suffix is None:
        # Single-line comment style (e.g., //, #)
        # Regular expressions for parsing
        # Allow optional space and accidental '//' after prefix (e.g., '# // SPDX-...')
        # REUSE-IgnoreStart
        copyright_regex = re.compile(
            f"^{re.escape(prefix)}\\s*(?:\\/\\/\\s*)?SPDX-FileCopyrightText: (\\d{{4}}) (.+)$"
        )
        license_regex = re.compile(
            f"^{re.escape(prefix)}\\s*(?:\\/\\/\\s*)?SPDX-License-Identifier: (.+)$"
        )
        # REUSE-IgnoreEnd

        # Find the header section
        in_header = True
        for i, line in enumerate(lines):
            if in_header:
                header_lines.append(line)

                # Check for copyright line
                copyright_match = copyright_regex.match(line)
                if copyright_match:
                    year = int(copyright_match.group(1))
                    author = copyright_match.group(2).strip()
                    authors[author] = (year, year)
                    continue

                # Check for license line
                license_match = license_regex.match(line)
                if license_match:
                    license_id = license_match.group(1).strip()
                    continue

                # Empty comment line or separator
                if line.strip() == prefix:
                    continue

                # If we get here, we've reached the end of the header
                if i > 0:  # Only if we've processed at least one line
                    header_lines.pop()  # Remove the non-header line
                    in_header = False
            else:
                break
    else:
        # Multi-line comment style (e.g., <!-- -->)
        # Regular expressions for parsing
        # REUSE-IgnoreStart
        copyright_regex = re.compile(r"^SPDX-FileCopyrightText: (\d{4}) (.+)$")
        license_regex = re.compile(r"^SPDX-License-Identifier: (.+)$")
        # REUSE-IgnoreEnd

        # Find the header section
        in_comment = False
        for i, line in enumerate(lines):
            stripped_line = line.strip()

            # Start of comment
            if stripped_line == prefix:
                in_comment = True
                header_lines.append(line)
                continue

            # End of comment
            if stripped_line == suffix and in_comment:
                header_lines.append(line)
                break

            if in_comment:
                header_lines.append(line)

                # Check for copyright line
                copyright_match = copyright_regex.match(stripped_line)
                if copyright_match:
                    year = int(copyright_match.group(1))
                    author = copyright_match.group(2).strip()
                    authors[author] = (year, year)
                    continue

                # Check for license line
                license_match = license_regex.match(stripped_line)
                if license_match:
                    license_id = license_match.group(1).strip()
                    continue

    return authors, license_id, header_lines


def remove_existing_header(content: str, comment_style: tuple[str, str | None]) -> tuple[str, bool]:
    """Remove one or more SPDX header blocks from the start of the file.
    Returns (stripped_content, removed_any).
    """
    prefix, suffix = comment_style
    lines = content.splitlines()
    i = 0
    removed = False

    if suffix is None:
        # Single-line comments: remove leading lines that start with prefix and include SPDX tokens
        while i < len(lines):
            line = lines[i]
            stripped = line.lstrip()
            if not stripped.startswith(prefix):
                break
            body = stripped[len(prefix):].lstrip()
            # tolerate accidental '//' after prefix
            if body.startswith('//'):
                body = body[2:].lstrip()
            if body.startswith('SPDX-') or body == '':
                i += 1
                removed = True
                continue
            # encountered a comment line without SPDX after header area
            break
        # Also trim following blank lines
        while i < len(lines) and lines[i].strip() == '':
            i += 1
    else:
        # Multi-line: remove consecutive comment blocks starting with prefix and containing SPDX lines
        j = 0
        while j < len(lines):
            if lines[j].strip() != prefix:
                break
            k = j + 1
            saw_spdx = False
            while k < len(lines) and lines[k].strip() != suffix:
                if 'SPDX-' in lines[k]:
                    saw_spdx = True
                k += 1
            if k < len(lines) and lines[k].strip() == suffix and saw_spdx:
                # remove this block
                j = k + 1
                removed = True
                # consume following blank lines
                while j < len(lines) and lines[j].strip() == '':
                    j += 1
            else:
                break
        i = j

    if removed:
        return "\n".join(lines[i:]) + ("\n" if content.endswith("\n") else ''), True
    return content, False

def create_header(authors, license_id, comment_style, last_author: str | None = None, last_year: int | None = None, file_path: str | None = None):
    """
    Creates a REUSE header with the given authors and license.
    Returns: header string

    comment_style is a tuple of (prefix, suffix)
    file_path is used to detect upstream source for the broader copyright line from dep5
    """
    prefix, suffix = comment_style
    lines = []

    # Get current year for the broader copyright line
    current_year = datetime.now(timezone.utc).year

    # Get upstream copyright holders from dep5 file
    upstream_copyrights = _get_upstream_copyright_from_dep5(file_path) if file_path else []
    if not upstream_copyrights:
        upstream_copyrights = [f"{DEFAULT_PROJECT_NAME} contributors"]

    # Track copyright holders we've already added to avoid duplicates
    added_copyrights = set()

    if suffix is None:
        # Single-line comment style (e.g., //, #)
        # Add broader copyright lines first (from dep5)
        for copyright_holder in upstream_copyrights:
            normalized = copyright_holder.lower().strip()
            if normalized not in added_copyrights:
                lines.append(f"{prefix} SPDX-FileCopyrightText: {current_year} {copyright_holder}")
                added_copyrights.add(normalized)

        # Build ordered list of authors
        ordered = []
        if authors:
            for author, (_, year) in sorted(authors.items(), key=lambda x: (x[1][1], x[0])):
                if author and not is_token(author) and not author.lower().startswith("unknown"):
                    # Check if this author is already covered by broader copyright
                    author_normalized = author.lower().strip()
                    # Skip if it's a duplicate of a broader copyright
                    is_duplicate = False
                    for added in added_copyrights:
                        if author_normalized in added or added in author_normalized:
                            is_duplicate = True
                            break
                    if not is_duplicate:
                        ordered.append((author, year))

        # Move last_author to the end if present
        if last_author:
            ordered = [(a, y) for (a, y) in ordered if a != last_author]
            if last_author in authors:
                ordered.append((last_author, authors[last_author][1]))
            else:
                ordered.append((last_author, last_year or datetime.now(timezone.utc).year))

        # Write individual authors
        if ordered:
            for a, y in ordered:
                lines.append(f"{prefix} SPDX-FileCopyrightText: {y} {a}")

        # Add separator
        lines.append(f"{prefix}")

        # Add license line
        # REUSE-IgnoreStart
        lines.append(f"{prefix} SPDX-License-Identifier: {license_id}")
        # REUSE-IgnoreEnd
    else:
        # Multi-line comment style (e.g., <!-- -->)
        # Start comment
        lines.append(f"{prefix}")

        # Add broader copyright lines first (from dep5)
        for copyright_holder in upstream_copyrights:
            normalized = copyright_holder.lower().strip()
            if normalized not in added_copyrights:
                lines.append(f"SPDX-FileCopyrightText: {current_year} {copyright_holder}")
                added_copyrights.add(normalized)

        # Add copyright lines
        ordered = []
        if authors:
            for author, (_, year) in sorted(authors.items(), key=lambda x: (x[1][1], x[0])):
                if author and not is_token(author) and not author.lower().startswith("unknown"):
                    # Check if this author is already covered by broader copyright
                    author_normalized = author.lower().strip()
                    # Skip if it's a duplicate of a broader copyright
                    is_duplicate = False
                    for added in added_copyrights:
                        if author_normalized in added or added in author_normalized:
                            is_duplicate = True
                            break
                    if not is_duplicate:
                        ordered.append((author, year))

        if last_author:
            ordered = [(a, y) for (a, y) in ordered if a != last_author]
            if last_author in authors:
                ordered.append((last_author, authors[last_author][1]))
            else:
                ordered.append((last_author, last_year or datetime.now(timezone.utc).year))

        if ordered:
            for a, y in ordered:
                lines.append(f"SPDX-FileCopyrightText: {y} {a}")

        # Add separator
        lines.append("")

        # Add license line
        # REUSE-IgnoreStart
        lines.append(f"SPDX-License-Identifier: {license_id}")
        # REUSE-IgnoreEnd

        # End comment
        lines.append(f"{suffix}")

    return "\n".join(lines)

def process_file(file_path, default_license_id, pr_base_sha=None, pr_head_sha=None, pr_author_login: str | None = None):
    """
    Processes a file to add or update REUSE headers.
    Returns: True if file was modified, False otherwise
    """
    # Check file extension
    _, ext = os.path.splitext(file_path)
    comment_style = COMMENT_STYLES.get(ext)
    if not comment_style:
        print(f"Skipping unsupported file type: {file_path}")
        return False

    # Check if file exists
    full_path = os.path.join(REPO_PATH, file_path)
    if not os.path.exists(full_path):
        print(f"File not found: {file_path}")
        return False

    # Read file content
    with open(full_path, 'r', encoding='utf-8-sig', errors='ignore') as f:
        content = f.read()

    # Parse existing header if any
    existing_authors, existing_license, header_lines = parse_existing_header(content, comment_style)

    # Get all authors from git
    git_authors = get_authors_from_git(file_path, REPO_PATH, pr_base_sha, pr_head_sha)

    # Add current user to authors (skip on bots/CI)
    try:
        name_cmd = ["git", "config", "user.name"]
        email_cmd = ["git", "config", "user.email"]
        user_name = run_git_command(name_cmd, check=False)
        user_email = run_git_command(email_cmd, check=False)

        skip_add_current = os.environ.get("REUSE_SKIP_ADD_CURRENT", "").lower() in ("1", "true", "yes")
        if (
            not skip_add_current
            and user_name
            and user_email
            and user_name.strip() != "Unknown"
            and not is_bot_name(user_name)
            and not _contains_token(user_name)
            and not _contains_token(user_email)
        ):
            # Use current year
            current_year = datetime.now(timezone.utc).year
            current_user = f"{user_name} <{user_email}>"

            # Add current user if not already present
            if current_user not in git_authors:
                git_authors[current_user] = (current_year, current_year)
                print(f"  Added current user: {current_user}")
            else:
                # Update year if necessary
                min_year, max_year = git_authors[current_user]
                git_authors[current_user] = (min(min_year, current_year), max(max_year, current_year))
        else:
            # Silent when skipping for bots or explicitly disabled
            if not skip_add_current:
                print("Warning: Skipping add of current user (unknown/bot/invalid)")
    except Exception as e:
        print(f"Error getting git user: {e}")

    # Optional email stripping (default: keep emails in SPDX headers)
    # Enable by setting REUSE_STRIP_EMAILS=true if you want names only.
    if os.environ.get("REUSE_STRIP_EMAILS", "").lower() in ("1", "true", "yes"):
        email_removal_pattern = re.compile(r"^(.+) (<\S+@\S+>)$")
        for author in list(git_authors.keys()):
            match = email_removal_pattern.match(author)
            if match:
                author_name = match.group(1).strip()
                git_authors[author_name] = git_authors.pop(author)
                print(f"Removed email from: {author_name}")

        for author in list(existing_authors.keys()):
            match = email_removal_pattern.match(author)
            if match:
                author_name = match.group(1).strip()
                existing_authors[author_name] = existing_authors.pop(author)
                print(f"Removed email from: {author_name}")

    # Determine what to do based on existing header
    last_editor = get_last_editor(file_path, REPO_PATH)
    if existing_license:
        print(f"Updating existing header for {file_path} (License: {existing_license})")

        # Optionally override existing license with the provided default_license_id
        force_license = os.environ.get("REUSE_FORCE_LICENSE", "").lower() in ("1", "true", "yes")

    # Combine existing and git authors, but filter out old upstream/fork attributions
        # These end with "contributors" and will be replaced by dep5 data
        combined_authors = {}
        for author, years in existing_authors.items():
            # Skip old upstream/fork attributions (e.g., "Wizards Den contributors")
            # These will be replaced by the dep5 file data
            if author.lower().endswith("contributors") or author.lower().endswith("contributors (modifications)"):
                continue
            combined_authors[author] = years

        for author, (git_min, git_max) in git_authors.items():
            has_token = is_token(author)
            if author.lower().startswith("unknown") or has_token:
                continue
            if author in combined_authors:
                existing_min, existing_max = combined_authors[author]
                combined_authors[author] = (min(existing_min, git_min), max(existing_max, git_max))
            else:
                combined_authors[author] = (git_min, git_max)
                print(f"  Adding new author: {author}")

        # Optionally ensure PR author is listed
        if pr_author_login:
            pl = pr_author_login.strip().lower()
            if pl and not is_bot_name(pl):
                found = False
                for a in combined_authors.keys():
                    if pl in a.strip().lower():
                        found = True
                        break
                if not found:
                    # Add a minimal GitHub-based author label
                    current_year = datetime.now(timezone.utc).year
                    combined_authors[f"{pr_author_login} (GitHub)"] = (current_year, current_year)

        # Choose license id
        license_to_use = default_license_id if force_license else existing_license

        # Remove any existing header blocks to avoid duplication
        stripped_content, _ = remove_existing_header(content, comment_style)
        new_header = create_header(
            combined_authors,
            license_to_use,
            comment_style,
            last_author=last_editor,
            last_year=(git_authors.get(last_editor, (None, None))[1] if last_editor else None),
            file_path=file_path,
        )

        # Always rebuild from stripped content for idempotency
        new_content = new_header + "\n\n" + stripped_content
    else:
        print(f"Adding new header to {file_path} (License: {default_license_id})")

        # Create new header with default license
        stripped_content, _ = remove_existing_header(content, comment_style)
        # Start with git authors
        new_authors = git_authors.copy()
        # Optionally ensure PR author is listed
        if pr_author_login:
            pl = pr_author_login.strip().lower()
            if pl and not is_bot_name(pl):
                found = False
                for a in new_authors.keys():
                    if pl in a.strip().lower():
                        found = True
                        break
                if not found:
                    current_year = datetime.now(timezone.utc).year
                    new_authors[f"{pr_author_login} (GitHub)"] = (current_year, current_year)

        new_header = create_header(
            new_authors,
            default_license_id,
            comment_style,
            last_author=last_editor,
            last_year=(git_authors.get(last_editor, (None, None))[1] if last_editor else None),
            file_path=file_path,
        )

        # Add header to file
        if content.strip():
            # For XML files, add the header after the XML declaration if present
            prefix, suffix = comment_style
            if suffix and stripped_content.lstrip().startswith("<?xml"):
                # Find the end of the XML declaration
                xml_decl_end = stripped_content.find("?>") + 2
                xml_declaration = stripped_content[:xml_decl_end]
                rest_of_content = stripped_content[xml_decl_end:].lstrip()
                new_content = xml_declaration + "\n" + new_header + "\n\n" + rest_of_content
            else:
                new_content = new_header + "\n\n" + stripped_content
        else:
            new_content = new_header + "\n"

    # Check if content changed
    if new_content == content:
        print(f"No changes needed for {file_path}")
        return False

    # Write updated content
    with open(full_path, 'w', encoding='utf-8', newline='\n') as f:
        f.write(new_content)

    print(f"Updated {file_path}")
    return True

def _resolve_license_id(license_label: str) -> str:
    """Resolve a license label or a combined list into a REUSE license ID string.
    Supports:
      - shorthand labels: 'agpl', 'mit', 'mpl'
      - SPDX IDs: 'AGPL-3.0-or-later', 'MIT', 'CC-BY-SA-3.0', etc.
      - combiners: '+', ',', ';', '&', 'OR', 'AND' (case-insensitive)
    If the expression contains 'AND' (and not 'OR'), we'll join with AND; otherwise, we
    default to OR for dual-licensing.
    """
    original = (license_label or "").strip()
    if not original:
        original = DEFAULT_LICENSE_LABEL

    # Determine preferred logical operator
    use_and = bool(_re.search(r"\bAND\b", original, flags=_re.IGNORECASE) and not _re.search(r"\bOR\b", original, flags=_re.IGNORECASE))
    joiner = " AND " if use_and else " OR "

    # Split tokens preserving case
    parts = [p.strip() for p in _re.split(r"\s+(?:OR|AND)\s+|[,;&+]+", original, flags=_re.IGNORECASE) if p and p.strip()]
    if not parts:
        parts = [DEFAULT_LICENSE_LABEL]

    # Build map of known SPDX IDs from config
    known_ids_lc = {v["id"].lower(): v["id"] for v in LICENSE_CONFIG.values()}

    ids: list[str] = []
    for token in parts:
        key = token.strip().lower()
        if key in LICENSE_CONFIG:
            ids.append(LICENSE_CONFIG[key]["id"])  # map label to SPDX ID
        elif key in known_ids_lc:
            ids.append(known_ids_lc[key])  # accept exact SPDX ID from config
        else:
            # Assume token is already an SPDX-compatible identifier; keep as-is
            ids.append(token.strip())

    if not ids:
        ids = [LICENSE_CONFIG[DEFAULT_LICENSE_LABEL]["id"]]

    return joiner.join(ids)


def main():
    parser = argparse.ArgumentParser(description="Update REUSE headers for PR files")
    parser.add_argument("--files-added", nargs="*", default=[], help="List of added files")
    parser.add_argument("--files-modified", nargs="*", default=[], help="List of modified files")
    parser.add_argument("--pr-license", default=DEFAULT_LICENSE_LABEL, help="License to use for new files")
    parser.add_argument("--pr-base-sha", help="Base SHA of the PR")
    parser.add_argument("--pr-head-sha", help="Head SHA of the PR")
    parser.add_argument("--pr-author", help="Login of the PR author (GitHub username)")

    args = parser.parse_args()

    # Resolve license id (supports combined labels like "mit+agpl")
    license_id = _resolve_license_id(args.pr_license)
    # REUSE-IgnoreStart
    print(f"Using license for new files: {license_id}")
    # REUSE-IgnoreEnd

    # Optional: load per-path license map
    def _load_license_map(path: str | None):
        data = None
        # Try env override first (JSON string)
        env_json = os.environ.get("REUSE_LICENSE_MAP_JSON")
        if env_json:
            try:
                data = json.loads(env_json)
            except Exception as ex:
                print(f"Warning: Failed to parse REUSE_LICENSE_MAP_JSON: {ex}", file=sys.stderr)

        # Then optional file path
        if data is None and path and os.path.exists(path):
            try:
                with open(path, 'r', encoding='utf-8') as f:
                    data = json.load(f)
            except Exception as ex:
                print(f"Warning: Failed to load license map from {path}: {ex}", file=sys.stderr)

        # Then conventional locations
        if data is None:
            for candidate in (".reuse/path-licenses.json", ".github/reuse-license-map.json"):
                if os.path.exists(candidate):
                    try:
                        with open(candidate, 'r', encoding='utf-8') as f:
                            data = json.load(f)
                        print(f"Loaded license map from {candidate}")
                        break
                    except Exception as ex:
                        print(f"Warning: Failed to load license map from {candidate}: {ex}", file=sys.stderr)

        # Normalize to list of {pattern, license}
        rules: list[dict] = []
        if isinstance(data, dict) and "rules" in data and isinstance(data["rules"], list):
            for r in data["rules"]:
                if isinstance(r, dict) and "pattern" in r and "license" in r:
                    rules.append({"pattern": str(r["pattern"]), "license": str(r["license"])})
        elif isinstance(data, dict):
            for k, v in data.items():
                rules.append({"pattern": str(k), "license": str(v)})
        elif isinstance(data, list):
            for r in data:
                if isinstance(r, dict) and "pattern" in r and "license" in r:
                    rules.append({"pattern": str(r["pattern"]), "license": str(r["license"])})

        # Augment with REUSE.toml if available
        try:
            if _tomllib is not None and os.path.exists("REUSE.toml"):
                with open("REUSE.toml", "rb") as f:
                    toml_data = _tomllib.load(f)
                files_rules = toml_data.get("files")
                if isinstance(files_rules, list):
                    for entry in files_rules:
                        if not isinstance(entry, dict):
                            continue
                        p = str(entry.get("path", "")).strip()
                        lic = str(entry.get("license", "")).strip()
                        if not p or not lic:
                            continue
                        p = p.replace("\\", "/")
                        # Convert a directory path to a glob pattern
                        patterns: list[str] = []
                        if p.endswith("/"):
                            patterns.append(f"{p}**")
                        else:
                            patterns.append(p)
                            patterns.append(f"{p}/**")
                        for pat in patterns:
                            rules.append({"pattern": pat, "license": lic})
                print("Loaded license rules from REUSE.toml")
        except Exception as ex:
            print(f"Warning: Failed to load REUSE.toml: {ex}", file=sys.stderr)
        return rules

    def _license_for_path(path: str, default_id: str, rules: list[dict]) -> str:
        if not rules:
            return default_id
        p = path.replace('\\', '/').lstrip('./')
        matches: list[tuple[int, str]] = []
        for r in rules:
            pat = r.get("pattern", "")
            lic = r.get("license", "")
            if not pat or not lic:
                continue
            if fnmatch.fnmatchcase(p, pat):
                matches.append((len(pat), lic))
        if not matches:
            return default_id
        # Prefer the longest pattern (most specific)
        _, lic_label = max(matches, key=lambda t: t[0])
        return _resolve_license_id(lic_label)

    license_map_path = os.environ.get("REUSE_LICENSE_MAP_PATH")
    license_rules = _load_license_map(license_map_path)

    # Process files
    files_changed = False

    # Normalize file lists in case they were passed as a single whitespace-separated string
    def _normalize_files(items):
        out = []
        for item in (items or []):
            if not item:
                continue
            # Split on any whitespace to expand combined lists
            out.extend(item.split())
        return out

    added_files = _normalize_files(args.files_added)
    modified_files = _normalize_files(args.files_modified)

    # Print the PR base and head SHAs
    print(f"\nPR Base SHA: {args.pr_base_sha}")
    print(f"PR Head SHA: {args.pr_head_sha}")

    print("\n--- Processing Added Files ---")
    for file in added_files:
        print(f"\nProcessing added file: {file}")
        file_license_id = _license_for_path(file, license_id, license_rules)
        if process_file(file, file_license_id, args.pr_base_sha, args.pr_head_sha, args.pr_author):
            files_changed = True

    print("\n--- Processing Modified Files ---")
    for file in modified_files:
        print(f"\nProcessing modified file: {file}")
        file_license_id = _license_for_path(file, license_id, license_rules)
        if process_file(file, file_license_id, args.pr_base_sha, args.pr_head_sha, args.pr_author):
            files_changed = True

    print("\n--- Summary ---")
    if files_changed:
        print("Files were modified")
    else:
        print("No files needed changes")

if __name__ == "__main__":
    main()
