using Godot;
using SatiRogue.Ecs.MapGenerator.Triggers;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.Ecs.Play.Nodes.Hud;
using SatiRogue.lib.RelEcsGodot.src;
using SatiRogue.scenes.Hud;
namespace SatiRogue.Ecs.Play.Systems;

public class HealthSystem : GdSystem {
   public override void Run() {
      var query = QueryBuilder<Entity, Character, HealthComponent, StatBar3D>().Has<Alive>().Build();

      foreach (var (entity, character, health, statBar3D) in query) {
         if (Mathf.IsEqualApprox(statBar3D.Percent, health.Percent)) continue;
         statBar3D.Percent = health.Percent;

         if (character is Player) {
            GetElement<HealthUi>().Percent = health.Percent;
         }

         if (health.Value > 0) continue;
         Send(new CharacterAnimationTrigger(character, "die"));
         Send(new CharacterDiedTrigger(character, entity));
         On(entity).Remove<Alive>();
      }
   }
}