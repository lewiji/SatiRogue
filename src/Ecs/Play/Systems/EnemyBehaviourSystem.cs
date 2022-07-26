using RelEcs;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;

namespace SatiRogue.Ecs.Play.Systems; 

public class EnemyBehaviourSystem : GDSystem {
   public override void Run() {
      var player = Query<Nodes.Actors.Player, GridPositionComponent>().GetEnumerator().Current;
      foreach (var (enemy, bTree, inputDir, gridPos) in Query<Enemy, BehaviourTree, InputDirectionComponent, GridPositionComponent>()) {
         if (enemy.Behaving) {
            bTree.Step(World, inputDir, gridPos, player.Item2);
         }
      }
   }

}