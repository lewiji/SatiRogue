using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using RelEcs;
using SatiRogue.Ecs.Play.Nodes.Actors;

namespace SatiRogue.Ecs.Play.Systems; 

public class MovementSystem : GDSystem {
   public override void Run() {
      var mapData = GetElement<MapGenData>();
      foreach (var (character, gridPos, input) in Query<Character, GridPositionComponent, InputDirectionComponent>()) {
         if (input.Direction == Vector2.Zero) continue;
         
         var targetPos = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y);
         var targetCell = mapData.GetCellAt(targetPos);
         
         if (!targetCell.Blocked) {
            mapData.GetCellAt(gridPos.Position).Occupants.Remove(character.GetInstanceId());
            gridPos.LastPosition = gridPos.Position;
            gridPos.Position += new Vector3(input.Direction.x, 0, input.Direction.y);
            targetCell.Occupants.Add(character.GetInstanceId());
            input.Direction = Vector2.Zero;
            Logger.Debug($"Moved {character} to: {gridPos.Position}");
         }
         else {
            input.Direction = Vector2.Zero;
         }
         
      }
   }
}