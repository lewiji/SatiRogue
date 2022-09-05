using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems;

public class EnemyBehaviourSystem : GdSystem {
   public override void Run() {
      var playerQuery = Query<Player, GridPositionComponent, HealthComponent, Stats>();
      var enemiesQuery = Query<Enemy, BehaviourTree, InputDirectionComponent, GridPositionComponent, HealthComponent, Stats>();

      foreach (var (_, playerGridPos, playerHealthComponent, playerStats) in playerQuery) {
         foreach (var (enemy, bTree, inputDir, gridPos, healthComponent, enemyStats) in enemiesQuery) {
            if (enemy.Behaving && healthComponent.IsAlive) {
               bTree.Step(World, enemy, inputDir, gridPos, playerHealthComponent, playerGridPos, playerStats, enemyStats);
            }
         }
      }
   }
}