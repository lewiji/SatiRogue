using Godot;
using RelEcs;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class InterpolateWalkAnimationSystem : ISystem {
   public World World { get; set; } = null!;
   float _lerpWeight = 14f;
   PhysicsDeltaTime? _delta;

   public void Run() {
      _delta ??= World.GetElement<PhysicsDeltaTime>();

      foreach (var (spatial, gridPos, walkable) in this.Query<Character, GridPositionComponent, Walkable>()) {
         if (walkable.Teleporting) {
            TeleportSpatial(spatial, gridPos);
         } else {
            InterpolateSpatial(spatial, gridPos);
         }

         if (walkable.Teleporting)
            walkable.Teleporting = false;
      }
   }

   void InterpolateSpatial(Spatial spatial, GridPositionComponent gridPos) {
      if (spatial.Translation.DistanceSquaredTo(gridPos.Position) < 0.003f) {
         spatial.Translation = gridPos.Position;
         return;
      }

      spatial.Translation = spatial.Translation.CubicInterpolate(gridPos.Position,
         gridPos.LastPosition + gridPos.LastPosition.DirectionTo(gridPos.Position) * 0.1f,
         gridPos.Position - gridPos.Position.DirectionTo(gridPos.LastPosition) * 0.1f, _lerpWeight * _delta!.Value);
   }

   static void TeleportSpatial(Spatial spatial, GridPositionComponent gridPos) {
      spatial.Translation = gridPos.Position;
      spatial.ResetPhysicsInterpolation();
   }
}