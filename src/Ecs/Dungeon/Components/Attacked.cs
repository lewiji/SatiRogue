using RelEcs;
using SatiRogue.Ecs.Dungeon.Components.Actor;

namespace SatiRogue.Ecs.Dungeon.Components;

public partial class Attacked
{
    public readonly Entity AttackingEntity;
    public Attacked(Entity attackingEntity)
    {
        AttackingEntity = attackingEntity;
    }
}