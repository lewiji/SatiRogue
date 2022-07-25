using Godot;
using RelEcs;
using SatiRogue.RelEcs.Components;

namespace SatiRogue.RelEcs.Systems; 

public class InterpolateWalkAnimationSystem : GDSystem {
   private float _lerpWeight => 14f;
   
   public override void Run() {
      foreach (var (spatial, gridPos, walkable) in Query<Spatial, GridPositionComponent, Walkable>()) {
         if (!gridPos.Position.IsEqualApprox(spatial.Translation)) {
            if (walkable.Teleporting) {
               TeleportSpatial(spatial, gridPos);
            } else {
               InterpolateSpatial(spatial, gridPos);
            }
         }
         
         if (walkable.Teleporting) walkable.Teleporting = false;
      }
      
   }

   private void InterpolateSpatial(Spatial spatial, GridPositionComponent gridPos) {
      var distanceSq = spatial.Translation.DistanceTo(gridPos.Position);
      if (distanceSq < 0.01f) {
         spatial.Translation = gridPos.Position;
      }
      else {
         TryGetElement<DeltaTime>(out var delta);
         spatial.Translation = spatial.Translation.LinearInterpolate(gridPos.Position, _lerpWeight * delta.Value);
      }
   }

   private static void TeleportSpatial(Spatial spatial, GridPositionComponent gridPos) {
      spatial.Translation = gridPos.Position;
      spatial.ResetPhysicsInterpolation();
   }
}