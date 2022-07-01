using System.Linq;
using Godot;
using SatiRogue.Components;
using SatiRogue.Components.Behaviours;
using SatiRogue.Components.Render;
using SatiRogue.Entities;
using SatiRogue.Grid.MapGen;
using SatiRogue.MathUtils;

namespace SatiRogue.Commands.MapGen; 

public class MapGenAddStairs : MapGenCommand {
   public MapGenAddStairs(MapGenMapData mapData) : base(mapData) { }
   public override Error Execute() {
      AddDownStairs();

      return Error.Ok;
   }

   private Error AddDownStairs() {
      var blocked = true;
      while (blocked) {
         var startingRoom = MapData.GeneratorSpaces.ElementAt((int) GD.RandRange(0, MapData.GeneratorSpaces.Count));
         var startX = (int) startingRoom.Position.x + (int) GD.RandRange(0, (int) startingRoom.Size.x);
         var startY = (int) startingRoom.Position.y + (int) GD.RandRange(0, (int) startingRoom.Size.y);
         var startVec = new Vector3i(startX, 0, startY);
         if (EntityRegistry.IsPositionBlocked(startVec)) continue;
         blocked = false;
         EntityRegistry.RegisterEntity(
            new StairsEntity(),
            new StairsEntityParameters {
               GridPosition = startVec,
               Direction = Vector2.Down,
               Visible = false,
               Components = new Component[] {new Stairs3DRendererComponent()}
            }
         );
      }

      return Error.Ok;
   }
}