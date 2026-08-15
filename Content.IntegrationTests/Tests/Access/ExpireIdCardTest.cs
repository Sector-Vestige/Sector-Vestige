#nullable enable
using Content.IntegrationTests.Fixtures;
using Content.IntegrationTests.Fixtures.Attributes;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

namespace Content.IntegrationTests.Tests.Access
{
    [TestOf(typeof(ExpireIdCardComponent))]
    public sealed class ExpireIdCardTest : GameTest
    {
        private const string TestExpireIdCard = "TestExpireIdCard";
        private static readonly ProtoId<AccessLevelPrototype> GenpopEnter = "GenpopEnter";
        private static readonly ProtoId<AccessLevelPrototype> GenpopLeave = "GenpopLeave";

        [TestPrototypes]
        private const string Prototypes = $@"
- type: entity
  id: {TestExpireIdCard}
  name: Expire ID Card
  components:
  - type: Access
    tags:
    - GenpopEnter
  - type: ExpireIdCard
    expireMessage: genpop-prisoner-id-expire
    expiredAccess:
    - GenpopLeave
";

        [SidedDependency(Side.Server)] private readonly SharedIdCardSystem _sharedIdCardSystem = null!;

        [Test]
        public async Task TestExpireIdCardResetsAccessTagsWhenExpiring()
        {
            EntityUid ent = default;
            ExpireIdCardComponent expireComp = default!;
            AccessComponent accessComp = default!;
            var expirationTimeInSeconds = 2.0f;
            // SV: ExpireTime is an absolute timestamp, not a duration. Test pairs are pooled and
            // recycled, so CurTime is already well past a bare TimeSpan.FromSeconds() and the
            // card expires on the first tick. Anchor to the server clock instead. (issue #368)
            var expireTime = TimeSpan.Zero;

            await Pair.Server.WaitPost(() =>
            {
                ent = SSpawn(TestExpireIdCard);
                expireComp = SComp<ExpireIdCardComponent>(ent);
                accessComp = SComp<AccessComponent>(ent);
                expireTime = SGameTiming.CurTime + TimeSpan.FromSeconds(expirationTimeInSeconds);
            });

            // Check that default component values are all correct
            using (Assert.EnterMultipleScope())
            {
                Assert.That(expireComp.Expired, Is.False);
                Assert.That(expireComp.Permanent, Is.False);
                Assert.That(expireComp.ExpireTime, Is.EqualTo(TimeSpan.Zero));
                Assert.That(accessComp.Tags, Is.EqualTo(new HashSet<ProtoId<AccessLevelPrototype>> { GenpopEnter }));
                Assert.That(expireComp.ExpiredAccess, Is.EqualTo(new HashSet<ProtoId<AccessLevelPrototype>> { GenpopLeave }));
                Assert.That(expireComp.ExpireMessage, Is.EqualTo(new LocId("genpop-prisoner-id-expire")));
            }

            // Set the expire time to the future
            _sharedIdCardSystem.SetExpireTime(ent, expireTime);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(expireComp.Expired, Is.False);
                Assert.That(expireComp.Permanent, Is.False);
                Assert.That(expireComp.ExpireTime, Is.EqualTo(expireTime));
                Assert.That(accessComp.Tags, Is.EqualTo(new HashSet<ProtoId<AccessLevelPrototype>> { GenpopEnter }));
            }

            // Ensure that after just before expiry, the card has not yet expired and the access has not been replaced
            await Pair.RunSeconds(1.0f);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(expireComp.Expired, Is.False);
                Assert.That(accessComp.Tags, Is.EqualTo(new HashSet<ProtoId<AccessLevelPrototype>> { GenpopEnter }));
            }

            // Ensure that after expiry, the card has expired and the access has been replaced
            // SV: overshoot the deadline rather than landing exactly on it, so the assertion does
            // not sit on a tick boundary.
            await Pair.RunSeconds(expirationTimeInSeconds);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(expireComp.Expired, Is.True);
                Assert.That(expireComp.Permanent, Is.False);
                Assert.That(accessComp.Tags, Is.EqualTo(new HashSet<ProtoId<AccessLevelPrototype>> { GenpopLeave }));
            }
        }
    }
}
