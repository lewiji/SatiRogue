using Godot;
using RelEcs;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using SatiRogue.Ecs.Dungeon.Triggers;
using SatiRogue.scenes.Hud;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public partial class HealthSystem : ISystem {
   
   PersistentPlayerData? _playerData = null;

   public void Run(World world) {
      _playerData ??= world.GetElement<PersistentPlayerData>();
      var query = world.Query<Entity, Character, HealthComponent, CharacterAnimationComponent, StatBar3D>().Has<Alive>().Build();

      foreach (var (entity, character, health, animationComponent, statBar3D) in query) {
         if (health.Invincible && health.Value < health.Max) health.Value = health.Max;
         
         if (Mathf.IsEqualApprox(statBar3D.Percent, health.Percent))
            continue;
         
         statBar3D.Percent = health.Percent;

         if (character is Player) {
            _playerData.Health = health.Value;
            world.GetElement<HealthUi>().Percent = health.Percent;
         }

         if (health.Value > 0)
            continue;
         animationComponent.Animation = "die";
         //world.Send(new CharacterDiedTrigger(character, entity));
         world.On(entity).Remove<Alive>();
         world.On(entity).Add<Dead>();
      }
   }
}