using Godot;
using RelEcs;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public partial class InterpolateWalkAnimationSystem : ISystem {
   
   float _lerpWeight = 14f;
   PhysicsDeltaTime? _delta;

   World? _world;
   public void Run(World world)
   {
      _world ??= world;
      _delta ??= world.GetElement<PhysicsDeltaTime>();

      foreach (var (spatial, gridPos, walkable) in world.Query<Character, GridPositionComponent, Walkable>().Has<Moving>()
      .Build()) {
         if (walkable.Teleporting || !spatial.VisibleOnScreenNotifier3D.IsOnScreen()) {
            TeleportSpatial(spatial, gridPos);
         } else {
            InterpolateSpatial(spatial, gridPos);
         }

         if (walkable.Teleporting)
            walkable.Teleporting = false;
      }
   }

   void InterpolateSpatial(Node3D spatial, GridPositionComponent gridPos) {
      if (spatial.Position.WithinManhattanDistance(gridPos.Position, 0.01f)) {
         spatial.Position = gridPos.Position;
         RemoveMovingComponent(spatial);
         return;
      }

      var translationDelta = spatial.Position.DirectionTo(gridPos.Position) * spatial.Position.DistanceTo(gridPos.Position);
      spatial.Position += (translationDelta * 10f * _delta!.Value);
      //spatial.Position = spatial.Position.Lerp(gridPos.Position, _lerpWeight * _delta!.Value);
      /*spatial.Position.CubicInterpolate(gridPos.Position,
      gridPos.Position - gridPos.LastPosition.DirectionTo(gridPos.Position) * 0.1f,
      gridPos.Position - gridPos.Position.DirectionTo(gridPos.LastPosition) * 0.1f, _lerpWeight * _delta!.Value);*/
   }

   void RemoveMovingComponent(Node3D spatial) {
      if (spatial.GetEntity() is { } entity && _world!.HasComponent<Moving>(entity)) _world!.RemoveComponent<Moving>(entity);
   }

   void TeleportSpatial(Node3D spatial, GridPositionComponent gridPos) {
      spatial.Position = gridPos.Position;
      RemoveMovingComponent(spatial);
      //spatial.ResetPhysicsInterpolation();
   }
}