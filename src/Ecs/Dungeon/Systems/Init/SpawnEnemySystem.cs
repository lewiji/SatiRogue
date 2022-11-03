using System.Collections.Generic;
using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.MapGenerator.Components;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems.Init;

public class SpawnEnemySystem : ISystem {
   public World World { get; set; } = null!;
   static readonly PackedScene EnemyScene = GD.Load<PackedScene>("res://src/Character/Enemy.tscn");

   public enum EnemyType {
      Harpy,
      Maw,
      Rat,
      FireKasina
   }

   static readonly Godot.Collections.Dictionary<EnemyType, string> HumanReadableEnemyNames = new() {
      {EnemyType.Harpy, "Harpy"},
      {EnemyType.Maw, "Maw"},
      {EnemyType.Rat, "Rat"},
      {EnemyType.FireKasina, "FireKasina"}
   };

   static readonly SpriteFrames HarpyFrames = GD.Load<SpriteFrames>("res://resources/enemies/harpy/harpy_blue_spriteframes.tres");
   static readonly SpriteFrames MawFrames = GD.Load<SpriteFrames>("res://resources/enemies/maw/maw_purple_sprite_Frames.tres");
   static readonly SpriteFrames RatFrames = GD.Load<SpriteFrames>("res://resources/enemies/ratfolk/ratfolk_axe_spriteframes.tres");
   static readonly SpriteFrames FireElementalFrames = GD.Load<SpriteFrames>(
      "res://resources/enemies/fire_elemental/FireElementalSpriteFrames.tres");

   static readonly Dictionary<EnemyType, SpriteFrames> EnemyGraphics = new() {
      {EnemyType.Harpy, HarpyFrames},
      {EnemyType.Maw, MawFrames},
      {EnemyType.Rat, RatFrames},
      {EnemyType.FireKasina, FireElementalFrames}
   };

   public record EnemyRecord {
      public EnemyType EnemyType { get; set; }
      public string Name { get; set; } = "";
      public SpriteFrames SpriteFrames { get; set; }
      public EnemyClass Class { get; set; }
   }

   static readonly EnemyRecord[] EnemyRecords = {
      new() {
         EnemyType = EnemyType.Harpy,
         Name = HumanReadableEnemyNames[EnemyType.Harpy],
         SpriteFrames = EnemyGraphics[EnemyType.Harpy],
         Class = EnemyClass.LowlyEnemy
      },
      new() {
         EnemyType = EnemyType.Maw,
         Name = HumanReadableEnemyNames[EnemyType.Maw],
         SpriteFrames = EnemyGraphics[EnemyType.Maw],
         Class = EnemyClass.LowlyEnemy
      },
      new() {
         EnemyType = EnemyType.Rat,
         Name = HumanReadableEnemyNames[EnemyType.Rat],
         SpriteFrames = EnemyGraphics[EnemyType.Rat],
         Class = EnemyClass.LowlyEnemy
      },
      new() {
         EnemyType = EnemyType.FireKasina,
         Name = HumanReadableEnemyNames[EnemyType.FireKasina],
         SpriteFrames = EnemyGraphics[EnemyType.FireKasina],
         Class = EnemyClass.LowlyEnemy
      }
   };

   public void Run() {
      var numEnemies = World.GetElement<MapGenData>().GeneratorParameters.NumEnemies;
      Logger.Info($"Spawning {numEnemies} enemies");
      var entitiesNode = World.GetElement<Entities>();

      for (var enemy = 0; enemy < numEnemies; enemy++) {
         var enemyNode = EnemyScene.Instance<Enemy>();
         var monsterId = Mathf.FloorToInt((float) GD.RandRange(0, EnemyRecords.Length));
         var health = Mathf.RoundToInt((float) GD.RandRange(1, 3));
         var enemyRecord = EnemyRecords[monsterId];
         enemyNode.EnemyRecord = enemyRecord;
         entitiesNode.AddChild(enemyNode);
         enemyNode.Stats.Record.Health = health;
         World.Spawn(enemyNode);
      }
   }
}

public readonly record struct EnemyGraphics(SpriteFrames Frames, Material Material) {
   public SpriteFrames Frames { get; } = Frames;
   public Material Material { get; } = Material;
}