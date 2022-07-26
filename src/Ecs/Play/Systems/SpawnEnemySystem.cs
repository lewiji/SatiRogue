using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;

namespace SatiRogue.Ecs.Play.Systems; 

public class SpawnEnemySystem : GDSystem {
   private static PackedScene _enemyScene = GD.Load<PackedScene>("res://src/Character/Enemy.tscn");

   public override void Run() {
      var numEnemies = GetElement<MapGenData>().GeneratorParameters.NumEnemies;
      Logger.Info($"Spawning {numEnemies} enemies");
      var entitiesNode = World.GetElement<Core.Entities>();
      for (var enemy = 0; enemy < numEnemies; enemy++) {
         var enemyNode = _enemyScene.Instance<Nodes.Actors.Enemy>();
         entitiesNode.AddChild(enemyNode);
         Spawn(enemyNode);
      }
   }
}