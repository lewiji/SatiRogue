using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems.Init;

public class SpawnEnemySystem : GdSystem {
   static readonly PackedScene EnemyScene = GD.Load<PackedScene>("res://src/Character/Enemy.tscn");

   static readonly SpatialMaterial HarpyMat = GD.Load<SpatialMaterial>("res://resources/enemies/harpy/harpy_blue_spatial_mat.tres");
   static readonly SpriteFrames HarpyFrames = GD.Load<SpriteFrames>("res://resources/enemies/harpy/harpy_blue_spriteframes.tres");
   static readonly SpatialMaterial MawMat = GD.Load<SpatialMaterial>("res://resources/enemies/maw/maw_purple_spatial_mat.tres");
   static readonly SpriteFrames MawFrames = GD.Load<SpriteFrames>("res://resources/enemies/maw/maw_purple_sprite_Frames.tres");
   static readonly SpatialMaterial RatMat = GD.Load<SpatialMaterial>("res://resources/enemies/ratfolk/ratfolk_axe_spatial_mat.tres");
   static readonly SpriteFrames RatFrames = GD.Load<SpriteFrames>("res://resources/enemies/ratfolk/ratfolk_axe_spriteframes.tres");
   static readonly SpatialMaterial FireElementalMat
      = GD.Load<SpatialMaterial>("res://resources/enemies/fire_elemental/FireElementalSpatialMaterial.tres");
   static readonly SpriteFrames FireElementalFrames
      = GD.Load<SpriteFrames>("res://resources/enemies/fire_elemental/FireElementalSpriteFrames.tres");

   static readonly EnemyGraphics[] EnemyGraphicsArray = {
      new(HarpyFrames, HarpyMat),
      new(MawFrames, MawMat),
      new(RatFrames, RatMat),
      new(FireElementalFrames, FireElementalMat)
   };

   public override void Run() {
      var numEnemies = GetElement<MapGenData>().GeneratorParameters.NumEnemies;
      Logger.Info($"Spawning {numEnemies} enemies");
      var entitiesNode = World.GetElement<Entities>();

      for (var enemy = 0; enemy < numEnemies; enemy++) {
         var enemyNode = EnemyScene.Instance<Enemy>();
         var monsterId = Mathf.FloorToInt((float) GD.RandRange(0, EnemyGraphicsArray.Length));
         var health = Mathf.RoundToInt((float) GD.RandRange(1, 3));
         enemyNode.Material = EnemyGraphicsArray[monsterId].Material;
         enemyNode.Frames = EnemyGraphicsArray[monsterId].Frames;
         entitiesNode.AddChild(enemyNode);
         enemyNode.Stats.Health = health;
         Spawn(enemyNode);
      }
   }
}

public readonly record struct EnemyGraphics(SpriteFrames Frames, Material Material) {
   public SpriteFrames Frames { get; } = Frames;
   public Material Material { get; } = Material;
}