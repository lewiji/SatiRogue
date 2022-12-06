using Godot;
using SatiRogue.Ecs.MapGenerator.Components;
using RelEcs;
using World = RelEcs.World;
namespace SatiRogue.Ecs.MapGenerator.Systems.MapGenStrategies;

public partial class CreateRooms : ISystem {
   

   public void Run(World world) {
      var mapGenData = world.GetElement<MapGenData>();
      var mapParams = mapGenData.GeneratorParameters;

      for (var roomIndex = 0; roomIndex < mapParams.NumRooms; roomIndex++) {
         var roomParams = new MapGenRoomParams(mapParams);

         for (var x = Mathf.RoundToInt(roomParams.FloorSpace.Position.x); x < Mathf.RoundToInt(roomParams.FloorSpace.End.x); x++) {
            for (var z = Mathf.RoundToInt(roomParams.FloorSpace.Position.y); z < Mathf.RoundToInt(roomParams.FloorSpace.End.y); z++) {
               var position = new Vector3(x, 0, z);
               mapGenData.SetCellType(position, CellType.Void);
            }
         }

         mapGenData.GeneratorSpaces.Add(roomParams.FloorSpace);
      }
   }
}