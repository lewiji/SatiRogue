using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Dungeon.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Items;
using SatiRogue.Ecs.MapGenerator.Components;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems.Init;

public partial class SpawnItemsSystem : ISystem {
   
   static readonly PackedScene ChestScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Items/Chest.tscn");
   static readonly PackedScene HealthScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Items/Health.tscn");
   static readonly PackedScene SpatialItemScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Items/SpatialItem.tscn");
   static readonly PackedScene LampScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Lamp.tscn");

   public void Run(World world) {
      var numChests = Mathf.CeilToInt(world.GetElement<MapGenData>().GeneratorParameters.NumRooms / (float) GD.RandRange(3f, 5f));
      Logger.Info($"Spawning {numChests} chests");
      var entitiesNode = world.GetElement<Entities>();

      for (var chestIndex = 0; chestIndex < numChests; chestIndex++) {
         var chestNode = ChestScene.Instantiate<Chest>();
         entitiesNode.AddChild(chestNode);
         world.Spawn(chestNode);
      }

      var numHealth = Mathf.CeilToInt(world.GetElement<MapGenData>().GeneratorParameters.NumRooms / (float) GD.RandRange(3f, 5f));
      Logger.Info($"Spawning {numHealth} health crystals.");

      for (var healthIndex = 0; healthIndex < numHealth; healthIndex++) {
         var healthNode = HealthScene.Instantiate<Health>();
         entitiesNode.AddChild(healthNode);
         world.Spawn(healthNode);
      }

      var numAnkhs = Mathf.CeilToInt(world.GetElement<MapGenData>().GeneratorParameters.NumRooms / (float) GD.RandRange(4f, 6f));
      Logger.Info($"Spawning {numAnkhs} Ankhs");

      for (var ankhIndex = 0; ankhIndex < numHealth; ankhIndex++) {
         var spatialItem = SpatialItemScene.Instantiate<SpatialItem>();
         entitiesNode.AddChild(spatialItem);
         world.Spawn(spatialItem);
      }

      var numLamps = Mathf.RoundToInt(world.GetElement<MapGenData>().GeneratorParameters.NumRooms / (float) GD.RandRange(4f, 8f));
      Logger.Info($"Spawning {numLamps} lamps");

      for (var lampIndex = 0; lampIndex < numHealth; lampIndex++) {
         var lamp = LampScene.Instantiate<Lamp>();
         entitiesNode.AddChild(lamp);
         world.Spawn(lamp);
      }
   }
}