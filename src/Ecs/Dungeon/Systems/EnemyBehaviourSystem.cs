using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Dungeon.Systems;

public class EnemyBehaviourSystem : ISystem {
   public World World { get; set; } = null!;

   public void Run() {
      var playerQuery = this.Query<Player, GridPositionComponent, HealthComponent, Stats>();
      var enemiesQuery = this.Query<Enemy, BehaviourTree, InputDirectionComponent, GridPositionComponent, HealthComponent, Stats>();

      foreach (var (_, playerGridPos, playerHealthComponent, playerStats) in playerQuery) {
         foreach (var (enemy, bTree, inputDir, gridPos, healthComponent, enemyStats) in enemiesQuery) {
            if (enemy.Behaving && healthComponent.IsAlive) {
               bTree.Step(World, enemy, inputDir, gridPos, playerHealthComponent, playerGridPos, playerStats, enemyStats);
            }
         }
      }
   }
}