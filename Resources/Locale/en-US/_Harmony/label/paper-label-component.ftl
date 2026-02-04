# SPDX-FileCopyrightText: 2026 Wizards Den contributors
# SPDX-FileCopyrightText: 2026 Sector Vestige contributors
# SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
# SPDX-FileCopyrightText: 2025 TheSecondLord <88201625+TheSecondLord@users.noreply.github.com>
#
# SPDX-License-Identifier: AGPL-3.0-or-later

comp-lanyard-has-lanyard = { CAPITALIZE(SUBJECT($user)) } { CONJUGATE-BE($user) } wearing a lanyard, it reads:
comp-lanyard-has-lanyard-blank = { CAPITALIZE(SUBJECT($user)) } { CONJUGATE-BE($user) } wearing a lanyard, but it's blank.
comp-lanyard-has-lanyard-cant-read = { CAPITALIZE(SUBJECT($user)) } { CONJUGATE-BE($user) } wearing a lanyard, but you can't read it from this distance.
# For listing stamps on all labels, not just lanyards
comp-label-examine-detail-stamped-by = The label has been stamped by: {$stamps}.
comp-lanyard-examine-detail-stamped-by = The lanyard has been stamped by: {$stamps}.
