using System.Collections.Generic;
using System.Linq;
using Godot;
using SatiRogue.Grid;
using SatiRogue.MathUtils;

namespace SatiRogue.Commands.MapGen;

public class MapGenFloodFill : MapGenCommand {
   public MapGenFloodFill(MapData mapData) : base(mapData) { }

   public override Error Execute() {
      var startPoint = MapData.GeneratorSpaces.First();
      var centre = startPoint.Position + startPoint.Size / 2f;
      FloodFillWalls((int) centre.x, (int) centre.y);
      return Error.Ok;
   }

   /** Flood fill from centre of voids, convert voids to floor, and set edges (nulls) as walls **/
   private void FloodFillWalls(int posX, int posY) {
      var tiles = new Stack<Vector3i>();
      tiles.Push(new Vector3i(posX, 0, posY));

      while (tiles.Count > 0) {
         var position = tiles.Pop();
         var cell = MapData.GetCellAt(position);
         switch (cell.Type) {
            // null is an uncarved space, make it a wall
            case null:
               cell.SetCellType(CellType.Wall);
               break;
            case CellType.Void:
               // set Void, non-null spaces we've traversed to Floors
               cell.Type = CellType.Floor;

               // Flood fill recursively
               tiles.Push(new Vector3i(position.x - 1, 0, position.z));
               tiles.Push(new Vector3i(position.x + 1, 0, position.z));
               tiles.Push(new Vector3i(position.x, 0, position.z - 1));
               tiles.Push(new Vector3i(position.x, 0, position.z + 1));
               tiles.Push(new Vector3i(position.x + 1, 0, position.z + 1));
               tiles.Push(new Vector3i(position.x - 1, 0, position.z + 1));
               tiles.Push(new Vector3i(position.x + 1, 0, position.z - 1));
               tiles.Push(new Vector3i(position.x - 1, 0, position.z - 1));
               break;
         }
      }
   }
}