using Content.Shared.Trigger.Components.Effects;
using Content.Shared.Weather;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Trigger.Systems;

<<<<<<< HEAD
public sealed class WeatherTriggerSystem : EntitySystem
=======
public sealed class WeatherTriggerSystem : XOnTriggerSystem<WeatherOnTriggerComponent>
>>>>>>> upstream/master
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly SharedWeatherSystem _weather = default!;

<<<<<<< HEAD
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WeatherOnTriggerComponent, TriggerEvent>(OnTrigger);
    }

    private void OnTrigger(Entity<WeatherOnTriggerComponent> ent, ref TriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.KeysIn.Contains(args.Key))
            return;

        var target = ent.Comp.TargetUser ? args.User : ent.Owner;

        if (target == null)
            return;

        var xform = Transform(target.Value);
=======
    protected override void OnTrigger(Entity<WeatherOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        var xform = Transform(target);
>>>>>>> upstream/master

        if (ent.Comp.Weather == null) //Clear weather if nothing is set
        {
            _weather.SetWeather(xform.MapID, null, null);
            return;
        }

        var endTime = ent.Comp.Duration == null ? null : ent.Comp.Duration + _timing.CurTime;

        if (_prototypeManager.Resolve(ent.Comp.Weather, out var weatherPrototype))
            _weather.SetWeather(xform.MapID, weatherPrototype, endTime);
    }
}
