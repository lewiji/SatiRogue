using Godot;
using SatiRogue.Grid;
using SatiRogue.Grid.MapGen;
using SatiRogue.MathUtils;

namespace SatiRogue.Commands.MapGen;

public class MapGenCreateRooms : MapGenCommand {
   public MapGenCreateRooms(MapGenMapData mapData) : base(mapData) { }

   public override Error Execute() {
      var mapParams = MapData.MapParams;
      for (var roomIndex = 0; roomIndex < mapParams.NumRooms; roomIndex++) {
         var roomParams = new MapGenRoomParams(mapParams);
         for (var x = Mathf.RoundToInt(roomParams.FloorSpace.Position.x); x < Mathf.RoundToInt(roomParams.FloorSpace.End.x); x++)
         for (var z = Mathf.RoundToInt(roomParams.FloorSpace.Position.y); z < Mathf.RoundToInt(roomParams.FloorSpace.End.y); z++) {
            var position = new Vector3i(x, 0, z);
            MapData.SetCellType(position, CellType.Void);
         }

         MapData.GeneratorSpaces.Add(roomParams.FloorSpace);
      }

      return Error.Ok;
   }
}