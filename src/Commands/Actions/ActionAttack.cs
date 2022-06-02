using System;
using Godot;
using SatiRogue.Components;
using SatiRogue.Components.Render;
using SatiRogue.Debug;
using SatiRogue.Entities;

namespace SatiRogue.Commands.Actions;

public class ActionAttack : Action
{
    private StatsComponent? _ownerStats;
    private StatsComponent? _targetStats;
    public ActionAttack(Entity owner, Entity target) : base(owner, target)
    {
        _ownerStats = owner?.GetComponent<StatsComponent>();
        _targetStats = target?.GetComponent<StatsComponent>();
        if (_ownerStats == null) throw new Exception($"ActionAttack owner {Owner?.Name} had no StatsComponent");
        if (_targetStats == null) throw new Exception($"ActionAttack target {Target?.Name} had no StatsComponent");
    }

    public override Error Execute()
    {
        Logger.Info($"{Owner!.Name} attacking {Target!.Name} for ");
        _targetStats?.Subtract(1);
        if (Owner is EnemyEntity enemyEntity)
            enemyEntity.GetComponent<EnemyMeshRendererComponent>()?.PlayAnimation("attack");
        else if (Owner is PlayerEntity playerEntity)
            playerEntity.PlayAnimation("attack");
        return Error.Ok;
    }
}