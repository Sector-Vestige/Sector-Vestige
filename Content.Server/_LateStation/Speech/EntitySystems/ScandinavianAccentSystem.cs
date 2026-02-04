// SPDX-FileCopyrightText: 2026 LateStation14 contributors
// SPDX-FileCopyrightText: 2025 MoonlightHollow <muszynskinicholas8@gmail.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Text;
using Content.Shared.Speech;
using Content.Server.Speech.EntitySystems;
using Robust.Shared.Random;

namespace Content.Server._Latestation.Speech.Components;

public sealed class ScandinavianAccentSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly ReplacementAccentSystem _replacement = default!;

    private static readonly IReadOnlyDictionary<char, char[]> Vowels = new Dictionary<char, char[]>()
    {
        { 'A',  ['Å','Ä','Æ'] },
        { 'a',  ['å','ä','æ'] },
        { 'O',  ['Ö','Ø'] },
        { 'o',  ['ö','ø'] },
    };

    public override void Initialize()
    {
        SubscribeLocalEvent<ScandinavianAccentComponent, AccentGetEvent>(OnAccent);
    }

    public string Accentuate(string message)
    {
        var msg = message;

        // Apply word replacements
        msg = _replacement.ApplyReplacements(msg, "scandinavian");

        // Random Umlaut Time! Happily taken from the German code.
        var msgBuilder = new StringBuilder(msg);
        var umlautCooldown = 0;
        for (var i = 0; i < msgBuilder.Length; i++)
        {
            var tempchar = msgBuilder[i];

            msgBuilder[i] = tempchar switch
            {
                'W' => 'V',
                'w' => 'v',
                'J' => 'Y',
                'j' => 'y',
                _ => msgBuilder[i]
            };

            if (umlautCooldown == 0)
            {
                if (_random.Prob(0.1f)) // 10% of all eligible vowels become umlauts)
                {
                    msgBuilder[i] = tempchar switch
                    {
                        'A' => _random.Pick(Vowels['A']),
                        'a' => _random.Pick(Vowels['a']),
                        'O' => _random.Pick(Vowels['O']),
                        'o' => _random.Pick(Vowels['o']),
                        _ => msgBuilder[i]
                    };
                    umlautCooldown = 4;
                }
            }
            else
            {
                umlautCooldown--;
            }
        }

        return msgBuilder.ToString();
    }

    private void OnAccent(Entity<ScandinavianAccentComponent> ent, ref AccentGetEvent args)
    {
        args.Message = Accentuate(args.Message);
    }
}
