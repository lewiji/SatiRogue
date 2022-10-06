using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.Dungeon.Triggers;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.MapGenerator.Systems;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class CharacterMovementSystem : Reference, ISystem {
   public World World { get; set; } = null!;

   public virtual void Run() {
      var mapData = World.GetElement<MapGenData>();
      var pathfindingHelper = World.GetElement<PathfindingHelper>();
      var query = this.QueryBuilder<Character, GridPositionComponent, InputDirectionComponent>().Not<Controllable>().Build();

      foreach (var (character, gridPos, input) in query) {
         if (input.Direction == Vector2.Zero)
            continue;

         var targetPos = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y);
         var targetCell = mapData.GetCellAt(targetPos);

         if (targetCell.Blocked)
            continue;

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

      if (character.Visible) {
         SendWalkAnimation(character);
      }

      if (character.AnimatedSprite3D != null) {
         character.AnimatedSprite3D.FlipH = inputDirectionComponent.Direction.x switch {
            < 0 => true,
            > 0 => false,
            _ => character.AnimatedSprite3D.FlipH
         };
      }
   }

   protected virtual void SendWalkAnimation(Character character) {
      this.Send(new CharacterAnimationTrigger(character, "walk"));
   }
}