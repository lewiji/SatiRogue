using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Nodes.Items;
namespace SatiRogue.Ecs.Play.Systems.Init;

public class SpawnItemsSystem : GDSystem {
   private static readonly PackedScene ChestScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Items/Chest.tscn");
   private static readonly PackedScene HealthScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Items/Health.tscn");

   public override void Run() {
      var numChests = Mathf.CeilToInt(GetElement<MapGenData>().GeneratorParameters.NumRooms / (float) GD.RandRange(3f, 5f));
      Logger.Info($"Spawning {numChests} chests");
      var entitiesNode = World.GetElement<Core.Entities>();

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
   }
}