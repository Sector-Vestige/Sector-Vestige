// SPDX-FileCopyrightText: 2026 Moffstation contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 qu4drivium <aaronholiver@outlook.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Moffstation.Paper.Components;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class ForgeSignatureComponent : Component
{
    /// <summary>
    /// The the signature written when a paper is signed
    /// </summary>
    [DataField, AutoNetworkedField]
    public string Signature = "";
}

[Serializable, NetSerializable]
public enum ForgeSignatureUiKey : byte
{
    Key,
}

[Serializable, NetSerializable]
public sealed class ForgedSignatureChangedMessage(string signature) : BoundUserInterfaceMessage
{
    public string Signature { get; } = signature;
}
