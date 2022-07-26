using System.Collections.Generic;
using System.Linq;
using Godot;
using SatiRogue.Ecs.MapGenerator.Components;
using RelEcs;

namespace SatiRogue.Ecs.MapGenerator.Systems.MapGenStrategies; 

public class FloodFill : GDSystem {
   public override void Run() {
      var mapGenData = GetElement<MapGenData>();
      var startPoint = mapGenData.GeneratorSpaces.First();
      var centre = startPoint.Position + startPoint.Size / 2f;
      FloodFillWalls(mapGenData, (int) centre.x, (int) centre.y);
   }
   
   /** Flood fill from centre of voids, convert voids to floor, and set edges (nulls) as walls **/
   private void FloodFillWalls(MapGenData mapGenData, int posX, int posY) {
      var tiles = new Stack<Vector3>();
      tiles.Push(new Vector3(posX, 0, posY));

      while (tiles.Count > 0) {
         var position = tiles.Pop();
         var cell = mapGenData.GetCellAt(position);
         switch (cell.Type) {
            // null is an uncarved space, make it a wall
            case null:
               mapGenData.SetCellType(cell.Id, CellType.Wall);
               break;
            case CellType.Void:
               // set Void, non-null spaces we've traversed to Floors
               cell.Type = CellType.Floor;

               // Flood fill recursively
               tiles.Push(new Vector3(position.x - 1, 0, position.z));
               tiles.Push(new Vector3(position.x + 1, 0, position.z));
               tiles.Push(new Vector3(position.x, 0, position.z - 1));
               tiles.Push(new Vector3(position.x, 0, position.z + 1));
               tiles.Push(new Vector3(position.x + 1, 0, position.z + 1));
               tiles.Push(new Vector3(position.x - 1, 0, position.z + 1));
               tiles.Push(new Vector3(position.x + 1, 0, position.z - 1));
               tiles.Push(new Vector3(position.x - 1, 0, position.z - 1));
               break;
         }
      }
   }
}