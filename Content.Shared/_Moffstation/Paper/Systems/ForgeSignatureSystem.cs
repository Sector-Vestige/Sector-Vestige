// SPDX-FileCopyrightText: 2026 Moffstation contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 qu4drivium <aaronholiver@outlook.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared._Moffstation.Paper.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Robust.Shared.Configuration;

namespace Content.Shared._Moffstation.Paper.Systems;

public sealed class ForgeSignatureSystem : EntitySystem
{
    [Dependency] private readonly ISharedAdminLogManager _adminLogger = default!;
    [Dependency] private readonly IConfigurationManager _cfgManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        Subs.BuiEvents<ForgeSignatureComponent>(ForgeSignatureUiKey.Key,
            subs =>
            {
                subs.Event<ForgedSignatureChangedMessage>(ForgedSignatureLabelChanged);
            });
    }

    private void ForgedSignatureLabelChanged(Entity<ForgeSignatureComponent> ent, ref ForgedSignatureChangedMessage args)
    {
        var signature = args.Signature.Trim();
        ent.Comp.Signature = signature[..Math.Min(_cfgManager.GetCVar(CCVars.MaxNameLength), signature.Length)];
        Dirty(ent);

        // Log signature change
        _adminLogger.Add(
            LogType.Action,
            LogImpact.Low,
     $"{ToPrettyString(args.Actor):user} set {ToPrettyString(ent.Owner):labeler} to have signature \"{ent.Comp.Signature}\"");
    }
}
