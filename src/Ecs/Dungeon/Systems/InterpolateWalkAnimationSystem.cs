using Godot;
using RelEcs;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class InterpolateWalkAnimationSystem : ISystem {
   public World World { get; set; } = null!;
   float _lerpWeight = 14f;
   PhysicsDeltaTime? _delta;

   public void Run() {
      _delta ??= World.GetElement<PhysicsDeltaTime>();

      foreach (var (spatial, gridPos, walkable) in this.QueryBuilder<Character, GridPositionComponent, Walkable>().Has<Moving>()
      .Build()) {
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
      if (spatial.Translation.DistanceSquaredTo(gridPos.Position) < 0.0005f) {
         spatial.Translation = gridPos.Position;
         RemoveMovingComponent(spatial);
         return;
      }

      spatial.Translation = spatial.Translation.LinearInterpolate(gridPos.Position, _lerpWeight * _delta!.Value);
      /*spatial.Translation.CubicInterpolate(gridPos.Position,
      gridPos.Position - gridPos.LastPosition.DirectionTo(gridPos.Position) * 0.1f,
      gridPos.Position - gridPos.Position.DirectionTo(gridPos.LastPosition) * 0.1f, _lerpWeight * _delta!.Value);*/
   }

   void RemoveMovingComponent(Spatial spatial) {
      if (spatial.GetEntity() is { } entity && World.HasComponent<Moving>(entity.Identity)) World.RemoveComponent<Moving>(entity.Identity);
   }

   void TeleportSpatial(Spatial spatial, GridPositionComponent gridPos) {
      spatial.Translation = gridPos.Position;
      RemoveMovingComponent(spatial);
      spatial.ResetPhysicsInterpolation();
   }
}