using System.Collections.Generic;
using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Resources;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems.Init;

public class SpawnEnemySystem : ISystem {
   public World World { get; set; } = null!;
   static readonly PackedScene EnemyScene = GD.Load<PackedScene>("res://src/Character/Enemy.tscn");
   static readonly List<EnemyResource> EnemyResources = new ();

   public void Run()
   {
      LoadEnemyResources();

      var numEnemies = World.GetElement<MapGenData>().GeneratorParameters.NumEnemies;
      Logger.Info($"Spawning {numEnemies} enemies");
      var entitiesNode = World.GetElement<Entities>();

      for (var enemy = 0; enemy < numEnemies; enemy++) {
         var enemyNode = EnemyScene.Instance<Enemy>();
         var monsterId = Mathf.FloorToInt((float) GD.RandRange(0, EnemyResources.Count));
         var health = Mathf.RoundToInt((float) GD.RandRange(1, 3));
         var enemyResource = EnemyResources[monsterId];
         enemyNode.EnemyResource = enemyResource;
         entitiesNode.AddChild(enemyNode);
         enemyNode.Stats.Record.Health = health;
         World.Spawn(enemyNode);
      }
   }

   void LoadEnemyResources()
   {
      if (EnemyResources.Count > 0) return;
      
      var dir = new Directory();
      dir.Open("res://resources/enemies");
      dir.ListDirBegin(true);
      var path = dir.GetNext();
      while (path != "")
      {
         if (!dir.CurrentIsDir())
         {
            var res = GD.Load($"{dir.GetCurrentDir()}/{path}");
            if (res is EnemyResource enemyResource)
            {
               EnemyResources.Add(enemyResource);
            }
         }
         path = dir.GetNext();
      }
   }
}

public readonly record struct EnemyGraphics(SpriteFrames Frames, Material Material) {
   public SpriteFrames Frames { get; } = Frames;
   public Material Material { get; } = Material;
}