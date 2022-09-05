using Godot;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems;

public class InterpolateWalkAnimationSystem : GdSystem {
   float _lerpWeight = 14f;
   PhysicsDeltaTime? _delta;

   public override void Run() {
      _delta ??= GetElement<PhysicsDeltaTime>();

      foreach (var (spatial, gridPos, walkable) in Query<Character, GridPositionComponent, Walkable>()) {
         if (walkable.Teleporting) {
            TeleportSpatial(spatial, gridPos);
         } else {
            InterpolateSpatial(spatial, gridPos);
         }

         if (walkable.Teleporting) walkable.Teleporting = false;
      }
   }

   void InterpolateSpatial(Spatial spatial, GridPositionComponent gridPos) {
      var currentTranslation = spatial.Translation;
      if (currentTranslation.DistanceSquaredTo(gridPos.Position) < 0.005f) return;

      spatial.Translation = currentTranslation.LinearInterpolate(gridPos.Position, _lerpWeight * _delta!.Value);
   }

   static void TeleportSpatial(Spatial spatial, GridPositionComponent gridPos) {
      spatial.Translation = gridPos.Position;
      spatial.ResetPhysicsInterpolation();
   }
}