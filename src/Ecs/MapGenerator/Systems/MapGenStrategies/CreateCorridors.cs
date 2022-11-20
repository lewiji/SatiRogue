using System.Linq;
using Godot;
using SatiRogue.Ecs.MapGenerator.Components;
using RelEcs;
using World = RelEcs.World;
namespace SatiRogue.Ecs.MapGenerator.Systems.MapGenStrategies;

public class CreateCorridors : ISystem {
   

   public void Run(World world) {
      var mapGenData = world.GetElement<MapGenData>();
      var arr = mapGenData.GeneratorSpaces.ToArray();

      for (var i = 1; i < arr.Length; i++) {
         var lastSpace = arr[i - 1];
         var currSpace = arr[i];
         var lastCentre = lastSpace.Position + lastSpace.Size / 2f;
         var currCentre = currSpace.Position + currSpace.Size / 2f;

         if (lastCentre.x < currCentre.x)
            for (var x = (int) lastCentre.x; x < (int) currCentre.x; x++)
               mapGenData.SetCellType(new Vector3(x, 0, (int) lastCentre.y), CellType.Void);
         else
            for (var x = (int) lastCentre.x; x > (int) currCentre.x; x--)
               mapGenData.SetCellType(new Vector3(x, 0, (int) lastCentre.y), CellType.Void);

         if (lastCentre.y < currCentre.y)
            for (var y = (int) lastCentre.y; y < (int) currCentre.y; y++)
               mapGenData.SetCellType(new Vector3((int) currCentre.x, 0, y), CellType.Void);
         else
            for (var y = (int) lastCentre.y; y > (int) currCentre.y; y--)
               mapGenData.SetCellType(new Vector3((int) currCentre.x, 0, y), CellType.Void);
      }
   }
}