using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using RelEcs;

namespace SatiRogue.Ecs.Play.Systems; 

public class MovementSystem : GDSystem {
   public override void Run() {
      foreach (var (gridPos, input) in Query<GridPositionComponent, PlayerInputDirectionComponent>()) {
         
         if (input.Direction == Vector2.Zero) continue;
         
         Logger.Info("Move?");

         gridPos.LastPosition = gridPos.Position;
         var targetPos = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y);
         var targetCell = GetElement<MapGenData>().GetCellAt(targetPos);
         if (!targetCell.Blocked) {
            gridPos.Position += new Vector3(input.Direction.x, 0, input.Direction.y);
            input.Direction = Vector2.Zero;
            Logger.Info($"Moved to: {gridPos.Position}");
         }
         
      }
   }
}