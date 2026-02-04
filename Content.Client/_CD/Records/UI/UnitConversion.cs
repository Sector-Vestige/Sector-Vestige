// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace Content.Client._CD.Records.UI;

public static class UnitConversion
{
    public static string GetImperialDisplayLength(int lengthCm)
    {
        var heightIn = (int) Math.Round(lengthCm * 0.3937007874 /* cm to in*/);
        return $"({heightIn / 12}'{heightIn % 12}'')";
    }

    public static string GetImperialDisplayMass(int massKg)
    {
        var weightLbs = (int) Math.Round(massKg * 2.2046226218 /* kg to lbs */);
        return $"({weightLbs} lbs)";
    }
}
