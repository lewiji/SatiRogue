using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Dungeon.Systems;

public class EnemyBehaviourSystem : ISystem {
   public World World { get; set; } = null!;

   public void Run() {
      var playerQuery = World.Query<Player, GridPositionComponent, HealthComponent, Stats, CharacterAnimationComponent>().Build();
      var enemiesQuery = World.Query<Enemy, BehaviourTree, InputDirectionComponent, GridPositionComponent, CharacterAnimationComponent, HealthComponent, Stats>().Build();
      foreach (var (_, playerGridPos, playerHealthComponent, playerStats, playerAni) in playerQuery) {
         foreach (var (enemy, bTree, inputDir, gridPos, enemyAni, healthComponent, enemyStats) in enemiesQuery) {
            if (enemy.Behaving && healthComponent.IsAlive) {
               bTree.Step(World, enemy, inputDir, gridPos, playerHealthComponent, playerGridPos, enemyAni, playerStats, enemyStats, 
               playerAni);
            }
         }
      }
   }
}