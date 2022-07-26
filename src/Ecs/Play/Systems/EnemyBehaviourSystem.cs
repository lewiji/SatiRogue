using RelEcs;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;

namespace SatiRogue.Ecs.Play.Systems; 

public class EnemyBehaviourSystem : GDSystem {
   public override void Run() {
      foreach (var (enemy, bTree) in Query<Enemy, BehaviourTree>()) {
         if (enemy.Behaving) {
            bTree.TreeInstance?.Step();
         }
      }
   }

}