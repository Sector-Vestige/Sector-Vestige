cmd-customobjectivecreate-desc = Gives a player a freeform objective you write yourself. Complete it later with customobjectivecomplete.
cmd-customobjectivecreate-help = customobjectivecreate <username> "<title>" "<description>" [icon] [issuer]

cmd-customobjective-invalid-args = Expected 3 to 5 arguments. Quote the title and description.
cmd-customobjective-player-not-found = Can't find the playerdata.
cmd-customobjective-mind-not-found = Can't find the mind.
cmd-customobjective-empty-title = The title can't be empty.
cmd-customobjective-adding-failed = Failed to create the objective. Check that the SVCustomObjective prototype loaded.
cmd-customobjective-success = Added custom objective #{ $index } to { $player }.
cmd-customobjective-title-hint = <title>
cmd-customobjective-description-hint = <description>
cmd-customobjective-icon-hint = [icon]
cmd-customobjective-icon-not-found = There is no icon named "{ $icon }". Tab-complete the 4th argument to see the list.
cmd-customobjective-issuer-hint = [issuer]
cmd-customobjective-issuer-not-found = There is no issuer named "{ $issuer }". Tab-complete the 5th argument to see the list.

cmd-customobjectivecomplete-desc = Marks one of a player's objectives complete. Only works on objectives nothing else is tracking.
cmd-customobjectivecomplete-help = customobjectivecomplete <username> <index>

cmd-completeobjective-invalid-args = Expected exactly 2 arguments.
cmd-completeobjective-player-not-found = Can't find the playerdata.
cmd-completeobjective-mind-not-found = Can't find the mind.
cmd-completeobjective-invalid-index = "{ $index }" is not a valid index.
cmd-completeobjective-index-out-of-range = Index { $index } is out of range, the player has { $count } objective(s).
cmd-completeobjective-not-manual = "{ $objective }" tracks its own progress and can't be completed by hand.
cmd-completeobjective-success = Marked objective #{ $index } "{ $objective }" complete.
cmd-completeobjective-objective-hint = { $objective } ({ $progress }%)
