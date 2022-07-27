using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using RelEcs;
using SatiRogue.Ecs.MapGenerator.Systems;
using SatiRogue.Ecs.Play.Nodes.Actors;

namespace SatiRogue.Ecs.Play.Systems; 

public class EnemyMovementSystem : GDSystem {
   public override void Run() {
      var mapData = GetElement<MapGenData>();
      var pathfindingHelper = GetElement<PathfindingHelper>();
      foreach (var (enemy, gridPos, input) in Query<Enemy, GridPositionComponent, InputDirectionComponent>()) {
         if (input.Direction == Vector2.Zero) continue;
         
         var targetPos = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y);
         var targetCell = mapData.GetCellAt(targetPos);
         
         if (!targetCell.Blocked) {
            var currentCell = mapData.GetCellAt(gridPos.Position);
            currentCell.Occupants.Remove(enemy.GetInstanceId());
            pathfindingHelper.SetCellWeight(currentCell.Id, currentCell.Occupants.Count);
            gridPos.LastPosition = gridPos.Position;
            gridPos.Position += new Vector3(input.Direction.x, 0, input.Direction.y);
            targetCell.Occupants.Add(enemy.GetInstanceId());
            pathfindingHelper.SetCellWeight(targetCell.Id, targetCell.Occupants.Count);
            input.Direction = Vector2.Zero;
            Logger.Debug($"Moved {enemy} to: {gridPos.Position}");
         }
         else {
            input.Direction = Vector2.Zero;
         }
      }
   }
}