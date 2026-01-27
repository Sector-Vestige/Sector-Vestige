
### Interaction Messages

# Shown when player tries to replace light, but there is no lights left
comp-light-replacer-missing-light = No lights left in {THE($light-replacer)}.

# Shown when player inserts light bulb inside light replacer
comp-light-replacer-insert-light = You insert {$bulb} into {THE($light-replacer)}.

# Shown when player tries to insert in light replacer brolen light bulb
comp-light-replacer-insert-broken-light = You can't insert broken lights!

# Shown when player refill light from light box
comp-light-replacer-refill-from-storage = You refill {THE($light-replacer)}.

# Displays the progress towards a new light
comp-light-replacer-recycle-progress = It is {$num}% of the way to a new light tube.

### Examine

comp-light-replacer-no-lights = It's empty.
comp-light-replacer-has-lights = It contains the following:
comp-light-replacer-light-listing = {$amount ->
    [one] [color=yellow]{$amount}[/color] [color=gray]{$name}[/color]
    *[other] [color=yellow]{$amount}[/color] [color=gray]{$name}s[/color]
}
