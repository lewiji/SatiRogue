using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.MapGenerator.Systems;
using SatiRogue.Ecs.MapGenerator.Triggers;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems;

public class CharacterMovementSystem : GdSystem {
   public override void Run() {
      var mapData = GetElement<MapGenData>();
      var pathfindingHelper = GetElement<PathfindingHelper>();
      var query = QueryBuilder<Character, GridPositionComponent, InputDirectionComponent>().Not<Controllable>().Build();

      foreach (var (character, gridPos, input) in query) {
         if (input.Direction == Vector2.Zero) continue;

         var targetPos = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y);
         var targetCell = mapData.GetCellAt(targetPos);

         if (targetCell.Blocked) continue;

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
      character.CurrentCell = targetCell;
      pathfindingHelper.SetCellWeight(currentCell.Id, currentCell.Occupants.Count);
      pathfindingHelper.SetCellWeight(targetCell.Id, targetCell.Occupants.Count);

      SendWalkAnimation(character);

      if (character.AnimatedSprite3D != null) {
         character.AnimatedSprite3D.FlipH = inputDirectionComponent.Direction.x switch {
            < 0 => true,
            > 0 => false,
            _ => character.AnimatedSprite3D.FlipH
         };
      }
   }

   protected virtual void SendWalkAnimation(Character character) {
      Send(new CharacterAnimationTrigger(character, "walk"));
   }
}