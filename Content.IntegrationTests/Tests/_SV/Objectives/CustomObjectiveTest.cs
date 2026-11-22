#nullable enable
using System.Linq;
using Content.IntegrationTests.Fixtures;
using Content.Shared._SV.Objectives;
using Content.Shared.Mind;
using Content.Shared.Objectives.Components;
using Content.Shared.Objectives.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests._SV.Objectives;

public sealed class CustomObjectiveTest : GameTest
{
    private const string DummyUsername = "CustomObjectiveTestUser";
    private static readonly ProtoId<CustomObjectiveIconPrototype> CuffsIconProto = "cuffs";

    public override PoolSettings PoolSettings => new()
    {
        Connected = false
    };

    /// <summary>
    /// Assigns two custom objectives to one mind, which only works if the prototype is not unique,
    /// then completes the first by index and checks the second is left alone.
    /// </summary>
    [Test]
    public async Task AddAndCompleteCustomObjectivesTest()
    {
        var pair = Pair;
        var server = pair.Server;
        var entMan = server.EntMan;
        var playerMan = server.ResolveDependency<ISharedPlayerManager>();
        var protoMan = server.ResolveDependency<IPrototypeManager>();
        var mindSys = server.System<SharedMindSystem>();
        var objectivesSys = server.System<SharedObjectivesSystem>();

        await server.AddDummySession(DummyUsername);
        await server.WaitRunTicks(5);

        var session = playerMan.Sessions.Single();

        Entity<MindComponent>? mindEnt = null;
        await server.WaitPost(() => mindEnt = mindSys.CreateMind(session.UserId));

        Assert.That(mindEnt, Is.Not.Null);
        var mind = mindEnt!.Value.Comp;
        Assert.That(mind.Objectives, Is.Empty, "Dummy player started with objectives.");

        await pair.WaitCommand($"customobjectivecreate {session.Name} \"Find the mole\" \"Someone in engineering is not who they say.\"");

        Assert.That(mind.Objectives, Has.Count.EqualTo(1), "customobjectivecreate failed to add an objective.");

        // the second one only lands if the prototype has unique: false.
        // the bad-icon rejection path can't be tested here, the harness fails any command that WriteErrors.
        await pair.WaitCommand($"customobjectivecreate {session.Name} \"Keep it quiet\" \"Tell nobody what you found.\" {CuffsIconProto} TheSyndicate");

        Assert.That(mind.Objectives, Has.Count.EqualTo(2), "A second custom objective was rejected as a duplicate.");

        var first = objectivesSys.GetInfo(mind.Objectives[0], mindEnt.Value, mind);
        var second = objectivesSys.GetInfo(mind.Objectives[1], mindEnt.Value, mind);
        var cuffsIcon = protoMan.Index(CuffsIconProto).Icon;
        var firstIssuer = entMan.GetComponent<ObjectiveComponent>(mind.Objectives[0]).Issuer;
        var secondIssuer = entMan.GetComponent<ObjectiveComponent>(mind.Objectives[1]).Issuer;

        Assert.Multiple(() =>
        {
            Assert.That(first, Is.Not.Null, "First objective produced no info, it is probably missing an icon.");
            Assert.That(second, Is.Not.Null, "Second objective produced no info, it is probably missing an icon.");
            Assert.That(first!.Value.Title, Is.EqualTo("Find the mole"));
            Assert.That(first.Value.Description, Is.EqualTo("Someone in engineering is not who they say."));
            Assert.That(second!.Value.Title, Is.EqualTo("Keep it quiet"));
            Assert.That(first.Value.Progress, Is.EqualTo(0f), "A new custom objective did not start at zero progress.");

            // one took the prototype default, the other the named icon
            Assert.That(second.Value.Icon, Is.EqualTo(cuffsIcon), "The named icon was not applied.");
            Assert.That(first.Value.Icon, Is.Not.EqualTo(cuffsIcon), "The named icon leaked onto the objective that omitted it.");

            // same story for the issuer, which is what groups them in the character menu
            Assert.That(secondIssuer.Id, Is.EqualTo("TheSyndicate"), "The named issuer was not applied.");
            Assert.That(firstIssuer.Id, Is.EqualTo("Unknown"), "The objective that omitted an issuer did not keep the prototype default.");
        });

        await pair.WaitCommand($"customobjectivecomplete {session.Name} 0");

        Assert.Multiple(() =>
        {
            Assert.That(objectivesSys.GetProgress(mind.Objectives[0], mindEnt.Value), Is.EqualTo(1f),
                "completeobjective did not complete the objective.");
            Assert.That(objectivesSys.GetProgress(mind.Objectives[1], mindEnt.Value), Is.EqualTo(0f),
                "completeobjective completed the wrong objective.");
        });
    }
}
