using Godot;
using RelEcs;
using SatiRogue.MathUtils;
using SatiRogue.RelEcs.Components;

namespace SatiRogue.RelEcs; 

public class MovementSystem : GDSystem {
   public override void Run() {
      foreach (var (gridPos, input) in Query<GridPositionComponent, InputDirectionComponent>()) {
         
         if (input.Direction == Vector2.Zero) continue;
         
         gridPos.LastPosition = gridPos.Position;
         gridPos.Position += new Vector3(input.Direction.x, 0, input.Direction.y);
         input.Direction = Vector2.Zero;
      }
   }
}