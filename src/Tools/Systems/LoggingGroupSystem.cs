using System;
using System.Reactive.Linq;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Plugins.ReactiveSystems.Systems;
using Godot;
using RoguelikeMono.Tools.Components;

namespace RoguelikeMono.Tools.Systems;

public class LoggingGroupSystem : IReactToEntitySystem
{
    public IGroup Group => new Group(typeof(CanLogComponent));

    public IObservable<IEntity>? ReactToEntity(IEntity entity)
    {
        var logComponent = entity.GetComponent<CanLogComponent>();
        return logComponent.Message?.Where(x => !x.Equals("")).Select(x => entity);
    }

    public void Process(IEntity entity)
    {
        var canTalkComponent = entity.GetComponent<CanLogComponent>();
        GD.Print($"Entity says '{canTalkComponent.Message}' @ {DateTime.Now}");
    }
}