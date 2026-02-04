// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 qu4drivium <aaronholiver@outlook.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader;
using Content.Shared._CD.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;

namespace Content.Client._CD.CartridgeLoader.Cartridges;

public sealed partial class NanoChatUi : UIFragment
{
    private NanoChatUiFragment? _fragment;

    public override Control GetUIFragmentRoot()
    {
        return _fragment!;
    }

    public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
    {
        _fragment = new NanoChatUiFragment();

        _fragment.OnMessageSent += (type, number, content, job) =>
        {
            SendNanoChatUiMessage(type, number, content, job, userInterface);
        };
    }

    public override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is NanoChatUiState cast)
            _fragment?.UpdateState(cast);
    }

    private static void SendNanoChatUiMessage(NanoChatUiMessageType type,
        uint? number,
        string? content,
        string? job,
        BoundUserInterface userInterface)
    {
        var nanoChatMessage = new NanoChatUiMessageEvent(type, number, content, job);
        var message = new CartridgeUiMessage(nanoChatMessage);
        userInterface.SendMessage(message);
    }
}
