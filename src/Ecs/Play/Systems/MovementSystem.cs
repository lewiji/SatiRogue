using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using RelEcs;
using SatiRogue.Ecs.MapGenerator.Systems;
using SatiRogue.Ecs.Play.Nodes.Actors;

namespace SatiRogue.Ecs.Play.Systems; 

public class MovementSystem : GDSystem {
   public override void Run() {
      var mapData = GetElement<MapGenData>();
      var pathfindingHelper = GetElement<PathfindingHelper>();
      foreach (var (character, gridPos, input) in Query<Character, GridPositionComponent, InputDirectionComponent>()) {
         if (input.Direction == Vector2.Zero) continue;
         
         var targetPos = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y);
         var targetCell = mapData.GetCellAt(targetPos);
         
         if (!targetCell.Blocked) {
            var currentCell = mapData.GetCellAt(gridPos.Position);
            currentCell.Occupants.Remove(character.GetInstanceId());
            pathfindingHelper.SetCellWeight(currentCell.Id, currentCell.Occupants.Count);
            gridPos.LastPosition = gridPos.Position;
            gridPos.Position += new Vector3(input.Direction.x, 0, input.Direction.y);
            targetCell.Occupants.Add(character.GetInstanceId());
            pathfindingHelper.SetCellWeight(targetCell.Id, targetCell.Occupants.Count);
            input.Direction = Vector2.Zero;
            Logger.Debug($"Moved {character} to: {gridPos.Position}");
         }
         else {
            input.Direction = Vector2.Zero;
         }
         
      }
   }
}