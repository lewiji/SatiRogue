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

public class HealthSystem : ISystem {
   public World World { get; set; } = null!;
   PersistentPlayerData? _playerData = null;

   public void Run() {
      _playerData ??= World.GetElement<PersistentPlayerData>();
      var query = World.Query<Entity, Character, HealthComponent, CharacterAnimationComponent, StatBar3D>().Has<Alive>().Build();

      foreach (var (entity, character, health, animationComponent, statBar3D) in query) {
         if (health.Invincible && health.Value < health.Max) health.Value = health.Max;
         
         if (Mathf.IsEqualApprox(statBar3D.Percent, health.Percent))
            continue;
         
         statBar3D.Percent = health.Percent;

         if (character is Player) {
            _playerData.Health = health.Value;
            World.GetElement<HealthUi>().Percent = health.Percent;
         }

         if (health.Value > 0)
            continue;
         animationComponent.Animation = "die";
         //World.Send(new CharacterDiedTrigger(character, entity));
         World.On(entity).Remove<Alive>();
         World.On(entity).Add<Dead>();
      }
   }
}