using Godot;
using RelEcs;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.MapGenerator.Systems;
using SatiRogue.Ecs.MapGenerator.Triggers;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
namespace SatiRogue.Ecs.Play.Systems;

public class PlayerMovementSystem : CharacterMovementSystem {
   public override void Run() {
      var mapData = GetElement<MapGenData>();
      var pathfindingHelper = GetElement<PathfindingHelper>();

      foreach (var (player, gridPos, input) in Query<Nodes.Actors.Player, GridPositionComponent, InputDirectionComponent>()) {
         if (input.Direction == Vector2.Zero) continue;

         var targetPos = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y);
         var targetCell = mapData.GetCellAt(targetPos);

         if (!targetCell.Blocked) {
            MoveToCell(mapData, gridPos, player, pathfindingHelper, input, targetCell);
            Send(new CharacterAudioTrigger(player, "walk"));
         } else {
            HandleOccupants(targetCell, player, input);
            Send(new CharacterAudioTrigger(player, "sword"));
         }
      }
   }

   private void HandleOccupants(Cell targetCell, Nodes.Actors.Player player, InputDirectionComponent inputDirectionComponent) {
      foreach (var targetId in targetCell.Occupants) {
         var target = GD.InstanceFromId(targetId) as Character;
         var entity = target?.GetMeta("Entity") as Entity;

         if (IsAlive(entity!)) {
            GetComponent<HealthComponent>(entity!).Value -= 1;
            Send(new CharacterAnimationTrigger(player, "attack"));
            Send(new CharacterAnimationTrigger(target!, "hit"));

            if (player.AnimatedSprite3D != null) {
               player.AnimatedSprite3D.FlipH = inputDirectionComponent.Direction.x switch {
                  < 0 => true,
                  > 0 => false,
                  _ => player.AnimatedSprite3D.FlipH
               };
            }
         }
      }
   }
}