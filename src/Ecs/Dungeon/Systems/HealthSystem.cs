using Godot;
using RelEcs;
using SatiRogue.Ecs.Core.Nodes;
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
      var query = this.QueryBuilder<Entity, Character, HealthComponent, StatBar3D>().Has<Alive>().Build();

      foreach (var (entity, character, health, statBar3D) in query) {
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
         this.Send(new CharacterAnimationTrigger(character, "die"));
         this.Send(new CharacterDiedTrigger(character, entity));
         this.On(entity).Remove<Alive>();
      }
   }
}