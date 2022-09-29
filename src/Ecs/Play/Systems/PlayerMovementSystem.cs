using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.MapGenerator.Systems;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.Ecs.Play.Nodes.Hud;
using SatiRogue.Ecs.Play.Nodes.Items;
using SatiRogue.Ecs.Play.Triggers;
using RelEcs;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Systems;

public class PlayerMovementSystem : CharacterMovementSystem {
   public override void Run() {
      var mapData = World.GetElement<MapGenData>();
      var pathfindingHelper = World.GetElement<PathfindingHelper>();

      foreach (var (player, gridPos, input) in this.Query<Player, GridPositionComponent, InputDirectionComponent>()) {
         if (input.Direction == Vector2.Zero)
            continue;

         var targetPos = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y);
         var targetCell = mapData.GetCellAt(targetPos);

         if (!targetCell.Blocked && !targetCell.Occupied) {
            MoveToCell(mapData, gridPos, player, pathfindingHelper, input, targetCell);
            Logger.Info($"Moved player to: {gridPos.Position}");
            this.Send(new CharacterAudioTrigger(player, "walk"));
         } else {
            HandleOccupants(targetCell, player, input, mapData, pathfindingHelper, gridPos);
            this.Send(new CharacterAudioTrigger(player, "sword"));
         }
      }
   }

   void HandleOccupants(Cell targetCell,
      Player player,
      InputDirectionComponent inputDirectionComponent,
      MapGenData mapData,
      PathfindingHelper pathfindingHelper,
      GridPositionComponent gridPos) {
      foreach (var targetId in targetCell.Occupants) {
         var target = GD.InstanceFromId(targetId);

         if (target is GameObject {Enabled: false}) {
            continue;
         }
         var entity = target?.GetMeta("Entity") as Marshallable<Entity>;
         var identity = entity?.Value.Identity;

         if (identity == null)
            continue;

         switch (target) {
            case Character character when World.IsAlive(identity.Value): {
               this.GetComponent<HealthComponent>(entity!.Value).Value -= 1;
               this.Send(new CharacterAnimationTrigger(player, "attack"));
               this.Send(new CharacterAnimationTrigger(character, "hit"));

               if (player.AnimatedSprite3D != null) {
                  player.AnimatedSprite3D.FlipH = inputDirectionComponent.Direction.x switch {
                     < 0 => true,
                     > 0 => false,
                     _ => player.AnimatedSprite3D.FlipH
                  };
               }
               break;
            }
            case Chest chest when World.HasComponent<Closed>(identity.Value):
               chest.Open = true;
               chest.BlocksCell = false;
               this.On(entity!.Value).Remove<Closed>().Add<Open>();
               chest.Enabled = false;
               World.GetElement<Loot>().NumLoots += 1;
               break;
            case Health health when !health.Taken:
               health.Taken = true;
               this.GetComponent<HealthComponent>(((Marshallable<Entity>) player.GetMeta("Entity")).Value).Value += 1;
               break;
            case SpatialItem spatialItem when World.HasComponent<Collectable>(identity.Value):
               this.On(entity!.Value).Remove<Collectable>().Remove<GridPositionComponent>().Add<InInventory>();
               spatialItem.BlocksCell = false;
               spatialItem.Visible = false;
               break;
            case Stairs stairs:
               Logger.Info("Stairs!");
               MoveToCell(mapData, gridPos, player, pathfindingHelper, inputDirectionComponent, targetCell);
               this.Send(new CharacterAudioTrigger(player, "walk"));
               World.GetElement<StairsConfirmation>().Popup();
               InputSystem.Paused = true;
               return;
            default:
               MoveToCell(mapData, gridPos, player, pathfindingHelper, inputDirectionComponent, targetCell);
               this.Send(new CharacterAudioTrigger(player, "walk"));
               return;
         }
      }
   }
}