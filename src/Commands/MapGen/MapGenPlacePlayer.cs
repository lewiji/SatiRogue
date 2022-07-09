using System.Linq;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Entities;
using SatiRogue.Grid;
using SatiRogue.Grid.MapGen;
using SatiRogue.MathUtils;
using SatiRogue.Tools;

namespace SatiRogue.Commands.MapGen;

public class MapGenPlacePlayer : MapGenCommand {
   public MapGenPlacePlayer(MapGenMapData mapData) : base(mapData) { }

   public override Error Execute() {
      var startingRoom = MapData.GeneratorSpaces.ElementAt((int) GD.RandRange(0, MapData.GeneratorSpaces.Count));
      var startX = (int) startingRoom.Position.x + (int) GD.RandRange(0, (int) startingRoom.Size.x);
      var startY = (int) startingRoom.Position.y + (int) GD.RandRange(0, (int) startingRoom.Size.y);
      EntityRegistry.RegisterEntity(
         new PlayerEntity(),
         new GridEntityParameters {
            GridPosition = new Vector3i(startX, 0, startY)
         });

      Logger.Info($"Created player at {new Vector3(startX, 0, startY)}");
      return Error.Ok;
   }
}