cmd-mapvotesv-desc = Starts a map vote between every map in the given map pool.
cmd-mapvotesv-help = Usage: mapvotesv <pool ID>
cmd-mapvotesv-hint = <pool ID>
cmd-mapvotesv-started = Started a map vote between { $count } maps from pool { $pool }.
cmd-mapvotesv-pool-not-found = No map pool exists with ID { $pool }.
cmd-mapvotesv-pool-empty = Map pool { $pool } contains no valid maps.
cmd-mapvotesv-map-not-found = Skipping { $map }: it is listed in pool { $pool } but no such map exists.

ui-vote-mapsv-title = The arrivals shuttle will take us to?
ui-vote-mapsv-title-runoff = Next map (runoff)
ui-vote-mapsv-win = The vote is over!
                    The arrivals shuttle is going to [color={$highlightcolor}]{$winner}[/color]!
ui-vote-mapsv-tie = Tie for map vote, and no runoff left! Picking... { $picked }
ui-vote-mapsv-runoff = Tie for map vote! Running it back between: { $maps }
ui-vote-mapsv-invalid = { $winner } became invalid after the map vote! It will not be selected!
ui-vote-mapsv-notlobby = Voting for maps is only valid in the pre-round lobby!
ui-vote-mapsv-notlobby-time = Voting for maps is only valid in the pre-round lobby with { $time } remaining!
