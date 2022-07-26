using System.Linq;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Components;
using RelEcs;

namespace SatiRogue.Ecs.Play.Systems; 

public class SpawnPlayerSystem : GDSystem {
   private PackedScene _playerScene = GD.Load<PackedScene>("res://src/Player/Player3d.tscn");

   public override void Run() {
      var mapData = GetElement<MapGenData>();
      Logger.Info("Spawning Player entity");
      var playerNode = _playerScene.Instance<Nodes.Actors.Player>();
      World.GetElement<Core.Entities>().AddChild(playerNode);
      var playerEntity = Spawn(playerNode).Id();
      World.AddElement(playerNode);
      
      var genSpaces = mapData.GeneratorSpaces;
      var room = genSpaces.ElementAt((int)GD.RandRange(0, genSpaces.Count));
      var x = (int) room.Position.x + (int) GD.RandRange(0, (int) room.Size.x);
      var y = (int) room.Position.y + (int) GD.RandRange(0, (int) room.Size.y);
      
      var query = Query<Nodes.Actors.Player, GridPositionComponent>();

      foreach (var (player, gridPos) in query) {
         gridPos.Position = new Vector3(x, 0, y);
         Logger.Info($"Set player pos to: {gridPos.Position}");
      }
   }
}