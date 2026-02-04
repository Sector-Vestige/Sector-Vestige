# SPDX-FileCopyrightText: 2026 Wizards Den contributors
# SPDX-FileCopyrightText: 2026 Sector Vestige contributors
# SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
# SPDX-FileCopyrightText: 2025 youtissoum <51883137+youtissoum@users.noreply.github.com>
#
# SPDX-License-Identifier: AGPL-3.0-or-later

## Applymapmodification command loc.

cmd-applymapmodification-desc = Applies a map modification to the given grid.
cmd-applymapmodification-help = applymapmodification <map modification ID> <grid ID>
cmd-applymapmodification-failure-integer = { $arg } is not a valid integer.
cmd-applymapmodification-modification-not-found = No map modification exists with name { $modification }.
cmd-applymapmodification-grid-not-found = No grid exists with ID { $grid }.
cmd-applymapmodification-success = Applied map modification { $modification } to grid { $grid }.
cmd-applymapmodification-grid-hint = <grid ID>
