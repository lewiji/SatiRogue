using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Dungeon.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Items;
using SatiRogue.Ecs.MapGenerator.Components;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems.Init;

public class SpawnItemsSystem : ISystem {
   public World World { get; set; } = null!;
   static readonly PackedScene ChestScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Items/Chest.tscn");
   static readonly PackedScene HealthScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Items/Health.tscn");
   static readonly PackedScene SpatialItemScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Items/SpatialItem.tscn");
   static readonly PackedScene LampScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Lamp.tscn");

   public void Run() {
      var numChests = Mathf.CeilToInt(this.GetElement<MapGenData>().GeneratorParameters.NumRooms / (float) GD.RandRange(3f, 5f));
      Logger.Info($"Spawning {numChests} chests");
      var entitiesNode = World.GetElement<Entities>();

      for (var chestIndex = 0; chestIndex < numChests; chestIndex++) {
         var chestNode = ChestScene.Instance<Chest>();
         entitiesNode.AddChild(chestNode);
         this.Spawn(chestNode);
      }

      var numHealth = Mathf.CeilToInt(this.GetElement<MapGenData>().GeneratorParameters.NumRooms / (float) GD.RandRange(3f, 5f));
      Logger.Info($"Spawning {numHealth} health crystals.");

      for (var healthIndex = 0; healthIndex < numHealth; healthIndex++) {
         var healthNode = HealthScene.Instance<Health>();
         entitiesNode.AddChild(healthNode);
         this.Spawn(healthNode);
      }

      var numAnkhs = Mathf.CeilToInt(this.GetElement<MapGenData>().GeneratorParameters.NumRooms / (float) GD.RandRange(4f, 6f));
      Logger.Info($"Spawning {numAnkhs} Ankhs");

      for (var ankhIndex = 0; ankhIndex < numHealth; ankhIndex++) {
         var spatialItem = SpatialItemScene.Instance<SpatialItem>();
         entitiesNode.AddChild(spatialItem);
         this.Spawn(spatialItem);
      }

      var numLamps = Mathf.RoundToInt(this.GetElement<MapGenData>().GeneratorParameters.NumRooms / (float) GD.RandRange(4f, 8f));
      Logger.Info($"Spawning {numLamps} lamps");

      for (var lampIndex = 0; lampIndex < numHealth; lampIndex++) {
         var lamp = LampScene.Instance<Lamp>();
         entitiesNode.AddChild(lamp);
         this.Spawn(lamp);
      }
   }
}