// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared._CD.Engraving;

/// <summary>
///     Allows an items' description to be modified with an engraving
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedEngraveableSystem))]
public sealed partial class EngraveableComponent : Component
{
    /// <summary>
    ///     Message given to user to notify them a message was sent
    /// </summary>
    [DataField, AutoNetworkedField]
    public string EngravedMessage = string.Empty;

    /// <summary>
    ///     The inspect text to use when there is no engraving
    /// </summary>
    [DataField]
    public LocId NoEngravingText = "engraving-dogtags-no-message";

    /// <summary>
    ///     The message to use when successfully engraving the item
    /// </summary>
    [DataField]
    public LocId EngraveSuccessMessage = "engraving-dogtags-succeed";

    /// <summary>
    ///     The inspect text to use when there is an engraving. The message will be shown seperately afterwards.
    /// </summary>
    [DataField]
    public LocId HasEngravingText = "engraving-dogtags-has-message";
}
