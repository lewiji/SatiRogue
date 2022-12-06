using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public partial class AttackSystem : ISystem
{
    MessageLog? _messageLog;
    public void Run(World world)
    {
        _messageLog ??= world.GetElement<MessageLog>();
        
        foreach (var (targetEntity, targetCharacter, targetHealth, attackedComponent) in world.Query<Entity, Character, HealthComponent, Attacked>().Build())
        {
            var attackingEntity = attackedComponent.AttackingEntity;
            var attackerStats = world.GetComponent<Stats>(attackingEntity).Record;
            var targetStats = world.GetComponent<Stats>(targetEntity).Record;
            
            var damage = Mathf.Max(0, attackerStats.Strength - targetStats.Defence);
            targetHealth.Value -= damage;
            world.RemoveComponent<Attacked>(targetEntity);

            if (world.TryGetComponent<CharacterAnimationComponent>(targetEntity, out var targetAnimation))
                targetAnimation!.Animation = "hit";

            if (!world.TryGetComponent<Character>(attackingEntity, out var sourceCharacter) ||
                !world.TryGetComponent<InputDirectionComponent>(attackingEntity, out var inputDirectionComponent))
                continue;
            
            if (sourceCharacter?.AnimatedSprite3D != null)
                switch (inputDirectionComponent?.Direction.x)
                {
                    case < 0:
                        sourceCharacter.AnimatedSprite3D.FlipH = true;
                        break;
                    case > 0:
                        sourceCharacter.AnimatedSprite3D.FlipH = false;
                        break;
                    default:
                        sourceCharacter.AnimatedSprite3D.FlipH = sourceCharacter.AnimatedSprite3D.FlipH;
                        break;
                }

            if (world.TryGetComponent<CharacterAnimationComponent>(attackingEntity, out var attackerAnimation))
                attackerAnimation!.Animation = "attack";

            _messageLog?.AddMessage($"{sourceCharacter?.CharacterName} hit {targetCharacter.CharacterName} for {damage} damage.");

        }
    }
}