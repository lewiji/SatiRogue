using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.MapGenerator.Systems;
using SatiRogue.Ecs.MapGenerator.Triggers;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
namespace SatiRogue.Ecs.Play.Systems;

public class CharacterMovementSystem : GDSystem {
   public override void Run() {
      var mapData = GetElement<MapGenData>();
      var pathfindingHelper = GetElement<PathfindingHelper>();
      var query = QueryBuilder<Character, GridPositionComponent, InputDirectionComponent>().Not<Controllable>().Build();

      foreach (var (character, gridPos, input) in query) {
         if (input.Direction == Vector2.Zero) continue;

         var targetPos = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y);
         var targetCell = mapData.GetCellAt(targetPos);

         if (targetCell.Blocked) continue;

         /*var currentCell = mapData.GetCellAt(gridPos.Position);
         currentCell.Occupants.Remove(enemy.GetInstanceId());
         pathfindingHelper.SetCellWeight(currentCell.Id, currentCell.Occupants.Count);
         gridPos.LastPosition = gridPos.Position;
         gridPos.Position += new Vector3(input.Direction.x, 0, input.Direction.y);
         targetCell.Occupants.Add(enemy.GetInstanceId());
         pathfindingHelper.SetCellWeight(targetCell.Id, targetCell.Occupants.Count);*/

         MoveToCell(mapData, gridPos, character, pathfindingHelper, input, targetCell);
      }
   }

   protected void MoveToCell(MapGenData mapData,
      GridPositionComponent gridPos,
      Character character,
      PathfindingHelper pathfindingHelper,
      InputDirectionComponent inputDirectionComponent,
      Cell targetCell) {
      // Move character to new cell, remove from old cell occupants, recalculate cell weight
      var currentCell = mapData.GetCellAt(gridPos.Position);

      gridPos.LastPosition = gridPos.Position;
      gridPos.Position = targetCell.Position;

      currentCell.Occupants.Remove(character.GetInstanceId());
      targetCell.Occupants.Add(character.GetInstanceId());
      pathfindingHelper.SetCellWeight(currentCell.Id, currentCell.Occupants.Count);
      pathfindingHelper.SetCellWeight(targetCell.Id, targetCell.Occupants.Count);

      Send(new CharacterAnimationTrigger(character, "walk"));

      if (character.AnimatedSprite3D != null) {
         if (inputDirectionComponent.Direction.x < 0) { character.AnimatedSprite3D.FlipH = true; }
         else if (inputDirectionComponent.Direction.x > 0) { character.AnimatedSprite3D.FlipH = false; }
      }
      Logger.Debug($"Moved {character} to: {gridPos.Position}");
   }
}