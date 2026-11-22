# Vote count shown under the name on vote buttons that carry an icon (see mapvotesv).
ui-vote-icon-button-votes = { $votes ->
    [one] { $votes } vote
   *[other] { $votes } votes
}
