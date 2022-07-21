using Godot;
using RelEcs;
using SatiRogue.MathUtils;

namespace SatiRogue.RelEcs; 

public class MovementSystem : ASystem {
   public override void Run() {
      foreach (var (node, gridPos, input) in Query<Spatial, GridPositionComponent, InputDirectionComponent>()) {
         gridPos.LastPosition = gridPos.Position;
         gridPos.Position += new Vector3i((int) input.Direction.x, 0, (int) input.Direction.y);
         input.Direction = Vector2.Zero;
         node.Translation = gridPos.Position.ToVector3();
      }
   }
}