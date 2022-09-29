using Godot;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.Ecs.Play.Nodes.Hud;
using SatiRogue.Ecs.Play.Triggers;
using RelEcs;
using World = RelEcs.World;
using SatiRogue.scenes.Hud;
namespace SatiRogue.Ecs.Play.Systems;

public class HealthSystem : ISystem {
   public World World { get; set; } = null!;

   public void Run() {
      var query = this.QueryBuilder<Entity, Character, HealthComponent, StatBar3D>().Has<Alive>().Build();

      foreach (var (entity, character, health, statBar3D) in query) {
         if (Mathf.IsEqualApprox(statBar3D.Percent, health.Percent)) continue;
         statBar3D.Percent = health.Percent;

         if (character is Player) {
            World.GetElement<HealthUi>().Percent = health.Percent;
         }

         if (health.Value > 0) continue;
         this.Send(new CharacterAnimationTrigger(character, "die"));
         this.Send(new CharacterDiedTrigger(character, entity));
         this.On(entity).Remove<Alive>();
      }
   }
}