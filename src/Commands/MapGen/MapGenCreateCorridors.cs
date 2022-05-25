using System.Linq;
using Godot;
using SatiRogue.Grid;
using SatiRogue.MathUtils;

namespace SatiRogue.Commands.MapGen;

public class MapGenCreateCorridors : MapGenCommand {
   public MapGenCreateCorridors(MapData mapData) : base(mapData) { }

   public override Error Execute() {
      var arr = MapData.GeneratorSpaces.ToArray();
      for (var i = 1; i < arr.Length; i++) {
         var lastSpace = arr[i - 1];
         var currSpace = arr[i];
         var lastCentre = lastSpace.Position + lastSpace.Size / 2f;
         var currCentre = currSpace.Position + currSpace.Size / 2f;

         if (lastCentre.x < currCentre.x)
            for (var x = (int) lastCentre.x; x < (int) currCentre.x; x++)
               MapData.SetCellType(new Vector3i(x, 0, (int) lastCentre.y), CellType.Void);
         else
            for (var x = (int) lastCentre.x; x > (int) currCentre.x; x--)
               MapData.SetCellType(new Vector3i(x, 0, (int) lastCentre.y), CellType.Void);

         if (lastCentre.y < currCentre.y)
            for (var y = (int) lastCentre.y; y < (int) currCentre.y; y++)
               MapData.SetCellType(new Vector3i((int) currCentre.x, 0, y), CellType.Void);
         else
            for (var y = (int) lastCentre.y; y > (int) currCentre.y; y--)
               MapData.SetCellType(new Vector3i((int) currCentre.x, 0, y), CellType.Void);
      }

      return Error.Ok;
   }
}