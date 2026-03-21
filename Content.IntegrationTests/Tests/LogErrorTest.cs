using Robust.Shared.Configuration;
using Robust.Shared.Log;
using Robust.UnitTesting;

namespace Content.IntegrationTests.Tests;

public sealed class LogErrorTest
{
    /// <summary>
    ///     This test ensures that error logs cause tests to fail.
    /// </summary>
    [Test]
    public async Task TestLogErrorCausesTestFailure()
    {
        await using var pair = await PoolManager.GetServerClient(new PoolSettings { Connected = true });
        var server = pair.Server;

        var cfg = server.ResolveDependency<IConfigurationManager>();
        var serverLogmill = server.ResolveDependency<ILogManager>().RootSawmill;
        var clientLogmill = pair.Client.ResolveDependency<ILogManager>().RootSawmill;

        // Default cvar is properly configured
        Assert.That(cfg.GetCVar(RTCVars.FailureLogLevel), Is.EqualTo(LogLevel.Error));

        // Errors don't throw immediately...
        Assert.DoesNotThrow(() => serverLogmill.Error("test"));
        Assert.DoesNotThrow(() => clientLogmill.Error("test"));

        // ...but do cause CleanReturnAsync to fail.
        Assert.ThrowsAsync<MultipleAssertException>(async () => await pair.CleanReturnAsync());
    }
}
