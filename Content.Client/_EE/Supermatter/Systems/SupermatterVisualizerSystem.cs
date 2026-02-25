// SPDX-FileCopyrightText: 2026 EinsteinEngines contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 Lachryphage (GitHub)
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
// SPDX-FileCopyrightText: 2025 hivehum <ketchupfaced@gmail.com>
// SPDX-FileCopyrightText: 2025 mqole <113324899+mqole@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 mqole <anactualpanacea@gmail.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Client._EE.Supermatter.Components;
using Content.Shared._EE.Supermatter.Components;
using Robust.Client.GameObjects;

namespace Content.Client._EE.Supermatter.Systems;

public sealed class SupermatterVisualizerSystem : VisualizerSystem<SupermatterVisualsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, SupermatterVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        var crystalLayer = args.Sprite.LayerMapGet(SupermatterVisuals.Crystal);
        var psyLayer = args.Sprite.LayerMapGet(SupermatterVisuals.Psy);

        if (AppearanceSystem.TryGetData(uid, SupermatterVisuals.Crystal, out SupermatterCrystalState crystalState, args.Component) &&
            component.CrystalVisuals.TryGetValue(crystalState, out var crystalData))
        {
            args.Sprite.LayerSetState(crystalLayer, crystalData.State);
        }

        if (AppearanceSystem.TryGetData(uid, SupermatterVisuals.Psy, out float psyState, args.Component))
        {
            var color = new Color(1f, 1f, 1f, psyState);
            args.Sprite.LayerSetColor(psyLayer, color);
        }
    }
}
