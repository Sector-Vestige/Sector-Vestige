// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared._CD.JobSlotsConsole;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Client._CD.JobSlotsConsole;

public sealed class JobSlotsConsoleBoundUserInterface : BoundUserInterface
{

    private JobSlotsConsoleMenu? _menu;

    public JobSlotsConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _menu = new JobSlotsConsoleMenu();
        _menu.OpenCentered();
        _menu.OnClose += Close;
        _menu.OnAdjustPressed += AdjustSlot;
    }

    private void AdjustSlot(ProtoId<JobPrototype> jobId, JobSlotAdjustment adjustment)
    {
        SendMessage(new JobSlotsConsoleAdjustMessage(jobId, adjustment));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not JobSlotsConsoleState cast)
            return;

        _menu?.UpdateState(cast);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;
        _menu?.Close();
        _menu = null;
    }
}
