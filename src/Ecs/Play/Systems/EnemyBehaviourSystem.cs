using RelEcs;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
namespace SatiRogue.Ecs.Play.Systems;

public class EnemyBehaviourSystem : GdSystem {
   public override void Run() {
      foreach (var (player, playerGridPos, playerHealthComponent) in Query<Nodes.Actors.Player, GridPositionComponent, HealthComponent>()) {
         foreach (var (enemy, bTree, inputDir, gridPos, healthComponent) in
                  Query<Enemy, BehaviourTree, InputDirectionComponent, GridPositionComponent, HealthComponent>()) {
            if (enemy.Behaving && healthComponent.IsAlive) {
               bTree.Step(World, enemy, inputDir, gridPos, playerHealthComponent, playerGridPos);
            }
         }
      }
   }
}