using Godot;
using RelEcs;
using SatiRogue.Debug;

namespace SatiRogue.Ecs.Play.Systems; 

public class SpawnEnemySystem : GDSystem {
   private static PackedScene _enemyScene = GD.Load<PackedScene>("res://src/Character/Enemy.tscn");

   public override void Run() {
      Logger.Info("Spawning enemy");
      var enemyNode = _enemyScene.Instance<Nodes.Actors.Enemy>();
      World.GetElement<Core.Entities>().AddChild(enemyNode);
      var entity = Spawn(enemyNode).Id();
      World.AddElement(enemyNode);
   }
}