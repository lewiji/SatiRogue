using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Dungeon.Systems;

public partial class EnemyBehaviourSystem : ISystem {
   

   public void Run(World world) {
      var playerQuery = world.Query<Player, GridPositionComponent, HealthComponent, Stats, CharacterAnimationComponent>().Build();
      var enemiesQuery = world.Query<Enemy, BehaviourTree, InputDirectionComponent, GridPositionComponent, CharacterAnimationComponent, HealthComponent, Stats>().Build();
      foreach (var (_, playerGridPos, playerHealthComponent, playerStats, playerAni) in playerQuery) {
         foreach (var (enemy, bTree, inputDir, gridPos, enemyAni, healthComponent, enemyStats) in enemiesQuery) {
            if (enemy.Behaving && healthComponent.IsAlive) {
               bTree.Step(world, enemy, inputDir, gridPos, playerHealthComponent, playerGridPos, enemyAni, playerStats, enemyStats, 
               playerAni);
            }
         }
      }
   }
}