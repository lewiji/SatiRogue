using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Nodes.Items;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems.Init;

public class SpawnItemsSystem : GdSystem {
   static readonly PackedScene ChestScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Items/Chest.tscn");
   static readonly PackedScene HealthScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Items/Health.tscn");
   static readonly PackedScene SpatialItemScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Items/SpatialItem.tscn");

   public override void Run() {
      var numChests = Mathf.CeilToInt(GetElement<MapGenData>().GeneratorParameters.NumRooms / (float) GD.RandRange(3f, 5f));
      Logger.Info($"Spawning {numChests} chests");
      var entitiesNode = World.GetElement<Entities>();

      for (var chestIndex = 0; chestIndex < numChests; chestIndex++) {
         var chestNode = ChestScene.Instance<Chest>();
         entitiesNode.AddChild(chestNode);
         Spawn(chestNode);
      }

      var numHealth = Mathf.CeilToInt(GetElement<MapGenData>().GeneratorParameters.NumRooms / (float) GD.RandRange(3f, 5f));
      Logger.Info($"Spawning {numHealth} health crystals.");

      for (var healthIndex = 0; healthIndex < numHealth; healthIndex++) {
         var healthNode = HealthScene.Instance<Health>();
         entitiesNode.AddChild(healthNode);
         Spawn(healthNode);
      }

      var numAnkhs = Mathf.CeilToInt(GetElement<MapGenData>().GeneratorParameters.NumRooms / (float) GD.RandRange(4f, 6f));
      Logger.Info($"Spawning {numAnkhs} Ankhs");

      for (var ankhIndex = 0; ankhIndex < numHealth; ankhIndex++) {
         var spatialItem = SpatialItemScene.Instance<SpatialItem>();
         entitiesNode.AddChild(spatialItem);
         Spawn(spatialItem);
      }
   }
}