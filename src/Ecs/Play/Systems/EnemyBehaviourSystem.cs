using RelEcs;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;

namespace SatiRogue.Ecs.Play.Systems; 

public class EnemyBehaviourSystem : GDSystem {
   public override void Run() {
      foreach (var (player, playerGridPos) in Query<Nodes.Actors.Player, GridPositionComponent>()) {
         foreach (var (enemy, bTree, inputDir, gridPos) in Query<Enemy, BehaviourTree, InputDirectionComponent, GridPositionComponent>()) {
            if (enemy.Behaving) {
               bTree.Step(World, enemy, inputDir, gridPos, player, playerGridPos);
            }
         }
      }
   }

}