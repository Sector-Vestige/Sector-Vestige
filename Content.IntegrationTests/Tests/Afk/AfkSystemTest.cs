// SPDX-FileCopyrightText: 2026 Wizards Den contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

#nullable enable
using System.Reflection;
using System.Linq;
using Content.IntegrationTests.Fixtures;
using Content.IntegrationTests.Fixtures.Attributes;
using Content.Server.Afk;
using Content.Server.GameTicking;
using Content.Shared.Afk;
using Content.Shared.CCVar;
using Content.Shared.Input;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.IntegrationTests.Tests.Afk;

[TestOf(typeof(AFKSystem))]
[TestOf(typeof(AfkConfirmSystem))]
public sealed class AfkSystemTest : GameTest
{
    // Saves having to go through the input API on client and dealing with shenanigans.
    private static readonly MethodInfo HandleInputCmd =
        typeof(AFKSystem).GetMethod("HandleInputCmd", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static readonly MethodInfo OnBoundUiMessageReceived =
        typeof(AFKSystem).GetMethod("OnBoundUiMessageReceived", BindingFlags.Instance | BindingFlags.NonPublic)!;

    [SidedDependency(Side.Server)] private readonly IAfkManager _afkManager = default!;
    [SidedDependency(Side.Server)] private readonly AFKSystem _afkSystem = default!;
    [SidedDependency(Side.Server)] private readonly AfkConfirmSystem _afkConfirm = default!;
    [SidedDependency(Side.Server)] private readonly IPlayerManager _playerManager = default!;
    [SidedDependency(Side.Server)] private readonly IConfigurationManager _cfg = default!;

    public override PoolSettings PoolSettings => new()
    {
        Connected = true,
        // Load bearing for connection I think.
        Dirty = true,
        DummyTicker = false,
    };

    [Test]
    public async Task ConfirmWindowAcknowledgeStopsTimeoutKick()
    {
        ICommonSession? session = null;

        await Server.WaitAssertion(() =>
        {
            session = GetSession();
            _cfg.SetCVar(CCVars.AfkConfirmTimeout, 0.01f);

            Assert.That(_afkConfirm.TryStartConfirmation(session), Is.True);
            Assert.That(_afkConfirm.HasConfirmation(session), Is.True);

            _afkConfirm.Confirm(session);
            Assert.That(_afkConfirm.HasConfirmation(session), Is.False);
        });

        await RunTicksSync(5);

        await Server.WaitAssertion(() =>
        {
            Assert.That(session!.Status, Is.Not.EqualTo(SessionStatus.Disconnected));
        });
    }

    [Test]
    public async Task ConfirmWindowTimeoutDisconnects()
    {
        ICommonSession? session = null;

        await Server.WaitAssertion(() =>
        {
            session = GetSession();

            _afkConfirm.AddConfirmationForTest(session, TimeSpan.Zero);
            Assert.That(_afkConfirm.HasConfirmation(session), Is.True);

            _afkConfirm.Update(0);
        });

        await RunTicksSync(5);

        await Server.WaitAssertion(() =>
        {
            Assert.That(session!.Status, Is.EqualTo(SessionStatus.Disconnected));
        });
    }

    [Test]
    public async Task BoundUiInteractionResetsAfk()
    {
        await MakeAfk();

        await Server.WaitAssertion(() =>
        {
            var session = GetSession();
            var actor = session.AttachedEntity!.Value;
            var ev = new BoundUserInterfaceMessageReceivedEvent(actor, actor, TestUiKey.Key);

            // Sector Vestige - start of AFK assertion that does not race the 1 ms AfkTime
            // OnBoundUiMessageReceived.Invoke(_afkSystem, [ev]);
            // Assert.That(_afkManager.IsAfk(session), Is.False);
            Assert.That(ResetsAfkTimer(session, () => OnBoundUiMessageReceived.Invoke(_afkSystem, [ev])), Is.True);
            // Sector Vestige - end of AFK assertion that does not race the 1 ms AfkTime
        });
    }

    [TestCase("Movement input", nameof(EngineKeyFunctions.MoveUp))]
    [TestCase("UI", nameof(ContentKeyFunctions.OpenInventoryMenu))]
    [TestCase("Using an item", nameof(ContentKeyFunctions.UseItemInHand))]
    public async Task InputResetsAfk(string inputType, string keyFunctionName)
    {
        await MakeAfk();

        await Server.WaitAssertion(() =>
        {
            var session = GetSession();
            var keyFunction = GetKeyFunction(keyFunctionName);
            var message = new FullInputCmdMessage(
                GameTick.Zero,
                0,
                0,
                _playerManager.KeyMap.KeyFunctionID(keyFunction),
                BoundKeyState.Down,
                NetCoordinates.Invalid,
                ScreenCoordinates.Invalid);

            // Sector Vestige - start of AFK assertion that does not race the 1 ms AfkTime
            // HandleInputCmd.Invoke(_afkSystem, [message, new EntitySessionEventArgs(session)]);
            // Assert.That(_afkManager.IsAfk(session), Is.False, inputType);
            var args = new EntitySessionEventArgs(session);
            Assert.That(ResetsAfkTimer(session, () => HandleInputCmd.Invoke(_afkSystem, [message, args])),
                Is.True, inputType);
            // Sector Vestige - end of AFK assertion that does not race the 1 ms AfkTime
        });
    }

    [Test]
    public async Task DisabledAfkTimerPreventsAfkFlagging()
    {
        await Server.WaitAssertion(() =>
        {
            var session = GetSession();
            _cfg.SetCVar(CCVars.AfkTime, 0f);
            _cfg.SetCVar(CCVars.AdminAfkTime, 0f);
            _afkManager.PlayerDidAction(session);
        });

        await RunTicksSync(5);

        await Server.WaitAssertion(() =>
        {
            var session = GetSession();
            _afkSystem.Update(0);

            Assert.Multiple(() =>
            {
                Assert.That(_afkManager.IsAfk(session), Is.True);
                Assert.That(_afkConfirm.HasConfirmation(session), Is.False);
            });
        });
    }

    [Test]
    public async Task DisabledAfkTimerChangeClearsExistingAfkConfirmation()
    {
        await Server.WaitAssertion(() =>
        {
            var session = GetSession();
            _cfg.SetCVar(CCVars.AfkConfirmTimeout, 10f);

            Assert.That(_afkConfirm.TryStartConfirmation(session), Is.True);
            Assert.That(_afkConfirm.HasConfirmation(session), Is.True);

            _cfg.SetCVar(CCVars.AfkTime, 0f);

            Assert.That(_afkConfirm.HasConfirmation(session), Is.False);
        });
    }

    // Sector Vestige - start of shared helper for the two "activity clears AFK" tests
    /// <summary>
    ///     Runs <paramref name="action"/> and reports whether it reset <paramref name="session"/>'s
    ///     AFK timer.
    /// </summary>
    /// <remarks>
    ///     MakeAfk drives AfkTime down to 0.001 seconds, and <see cref="IAfkManager.IsAfk"/> measures
    ///     against wall-clock <see cref="IGameTiming.RealTime"/>. Re-reading IsAfk after triggering the
    ///     activity therefore only passes if under a millisecond of real time elapses in between, so a
    ///     GC pause or a busy CI runner fails the test on timing alone. Watching
    ///     <see cref="IAfkManager.PlayerDidActionEvent"/> asserts the thing actually under test — that
    ///     the handler reported the activity — with no clock involved.
    /// </remarks>
    private bool ResetsAfkTimer(ICommonSession session, Action action)
    {
        var reset = false;

        void OnPlayerDidAction(ICommonSession player)
        {
            if (player == session)
                reset = true;
        }

        _afkManager.PlayerDidActionEvent += OnPlayerDidAction;

        try
        {
            action();
        }
        finally
        {
            _afkManager.PlayerDidActionEvent -= OnPlayerDidAction;
        }

        return reset;
    }
    // Sector Vestige - end of shared helper for the two "activity clears AFK" tests

    private async Task MakeAfk()
    {
        await Server.WaitAssertion(() =>
        {
            var session = GetSession();
            _cfg.SetCVar(CCVars.AfkTime, 0.001f);
            _cfg.SetCVar(CCVars.AdminAfkTime, 0.001f);
            _afkManager.PlayerDidAction(session);
        });

        await RunTicksSync(5);

        await Server.WaitAssertion(() =>
        {
            var session = GetSession();
            Assert.That(_afkManager.IsAfk(session), Is.True);
        });
    }

    private ICommonSession GetSession()
    {
        var session = _playerManager.Sessions.Single();

        Assert.Multiple(() =>
        {
            Assert.That(session.Status, Is.EqualTo(SessionStatus.InGame));
            Assert.That(session.AttachedEntity, Is.Not.Null);
        });

        return session;
    }

    private static BoundKeyFunction GetKeyFunction(string fieldName)
    {
        var field = typeof(EngineKeyFunctions).GetField(fieldName, BindingFlags.Public | BindingFlags.Static)
            ?? typeof(ContentKeyFunctions).GetField(fieldName, BindingFlags.Public | BindingFlags.Static);

        Assert.That(field, Is.Not.Null);
        return (BoundKeyFunction) field!.GetValue(null)!;
    }

    private enum TestUiKey : byte
    {
        Key,
    }
}
