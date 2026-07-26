#nullable enable
using Content.IntegrationTests.Fixtures;
using Robust.Shared.Console;

namespace Content.IntegrationTests.Tests._SV.Objectives;

public sealed class CustomObjectiveCompletionTest : GameTest
{
    public override PoolSettings PoolSettings => new()
    {
        Connected = false
    };

    /// <summary>
    /// Every argument of the custom objective commands should say what it wants when tabbed, and
    /// every offered option should say what picking it means. CompletionHelper.PrototypeIDs gives
    /// bare ids with no hints, so the prototype backed arguments build their own options.
    /// </summary>
    [Test]
    public async Task CompletionHintsTest()
    {
        var server = Pair.Server;
        var host = server.ResolveDependency<IConsoleHost>();

        await server.WaitPost(() =>
        {
            var create = host.AvailableCommands["customobjectivecreate"];
            var complete = host.AvailableCommands["customobjectivecomplete"];
            var shell = host.LocalShell;

            Assert.Multiple(() =>
            {
                // description and help resolve from fluent by command name, so a rename silently breaks them
                foreach (var cmd in new[] { create, complete })
                {
                    Assert.That(cmd.Description, Is.Not.Null.And.Not.Empty, $"{cmd.Command} has no description.");
                    Assert.That(cmd.Description, Does.Not.StartWith("cmd-"), $"{cmd.Command} description is an unresolved loc id.");
                    Assert.That(cmd.Help, Does.Not.StartWith("cmd-"), $"{cmd.Command} help is an unresolved loc id.");
                }

                for (var n = 1; n <= 5; n++)
                {
                    var result = create.GetCompletion(shell, new string[n]);

                    Assert.That(result.Hint, Is.Not.Null.And.Not.Empty, $"Argument {n} has no hint.");
                    Assert.That(result.Hint, Does.Not.StartWith("cmd-"), $"Argument {n} hint is an unresolved loc id.");

                    // the icon and issuer arguments are the prototype backed ones
                    if (n < 4)
                        continue;

                    Assert.That(result.Options, Is.Not.Empty, $"Argument {n} offered no options.");
                    foreach (var option in result.Options)
                    {
                        Assert.That(option.Hint, Is.Not.Null.And.Not.Empty,
                            $"Argument {n} option \"{option.Value}\" has no hint.");
                    }
                }
            });
        });
    }
}
