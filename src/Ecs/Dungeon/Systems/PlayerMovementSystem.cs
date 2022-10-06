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
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Systems;

public class PlayerMovementSystem : CharacterMovementSystem {
   MessageLog? _messageLog;
   MapGenData? _mapGenData;
   PathfindingHelper? _pathfindingHelper;
   public override void Run() {
      _mapGenData ??= World.GetElement<MapGenData>();
      _pathfindingHelper ??= World.GetElement<PathfindingHelper>();
      _messageLog ??= World.GetElement<MessageLog>();

      foreach (var (playerEntity, player, gridPos, input) in this.Query<Entity, Player, GridPositionComponent, InputDirectionComponent>()) {
         if (input.Direction == Vector2.Zero)
            continue;

         var targetPos = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y);
         var targetCell = _mapGenData.GetCellAt(targetPos);

         if (!targetCell.Blocked && !targetCell.Occupied) {
            MoveToCell(_mapGenData, gridPos, player, _pathfindingHelper, input, targetCell);
            Logger.Info($"Moved player to: {gridPos.Position}");
            this.Send(new CharacterAudioTrigger(player, "walk"));
         } else {
            HandleOccupants(targetCell, playerEntity, player, input, _mapGenData, _pathfindingHelper, gridPos);
            this.Send(new CharacterAudioTrigger(player, "sword"));
         }
      }
   }

   void HandleOccupants(Cell targetCell,
      Entity playerEntity,
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
               var playerStats = World.GetComponent<Stats>(playerEntity.Identity).Record;
               var enemyStats = World.GetComponent<Stats>(identity.Value).Record;
               var damage = Mathf.Max(0, playerStats.Strength - enemyStats.Defence);
               this.GetComponent<HealthComponent>(entity!.Value).Value -= damage;
               this.Send(new CharacterAnimationTrigger(player, "attack"));
               this.Send(new CharacterAnimationTrigger(character, "hit"));

               if (player.AnimatedSprite3D != null) {
                  player.AnimatedSprite3D.FlipH = inputDirectionComponent.Direction.x switch {
                     < 0 => true,
                     > 0 => false,
                     _ => player.AnimatedSprite3D.FlipH
                  };
               }
               _messageLog?.AddMessage($"Hit {character.Name} for {damage} damage.");
               break;
            }
            case Chest chest when World.HasComponent<Closed>(identity.Value):
               chest.Open = true;
               chest.BlocksCell = false;
               this.On(entity!.Value).Remove<Closed>().Add<Open>();
               chest.Enabled = false;
               var goldAmount = 1;
               World.GetElement<PersistentPlayerData>().Gold += goldAmount;
               _messageLog?.AddMessage($"Retrieved {goldAmount} gold from chest.");
               break;
            case Health health when !health.Taken:
               health.Taken = true;
               var healthAmount = 1;
               this.GetComponent<HealthComponent>(((Marshallable<Entity>) player.GetMeta("Entity")).Value).Value += healthAmount;
               _messageLog?.AddMessage($"Relic healed player {healthAmount} HP.");
               break;
            case SpatialItem spatialItem when World.HasComponent<Collectable>(identity.Value):
               this.On(entity!.Value).Remove<Collectable>().Remove<GridPositionComponent>().Add<InInventory>().Add<JustPickedUp>();
               spatialItem.BlocksCell = false;
               spatialItem.Visible = false;
               _messageLog?.AddMessage($"Picked up {spatialItem.Name} from the ground.");
               break;
            case Stairs stairs:
               Logger.Info("Stairs!");
               MoveToCell(mapData, gridPos, player, pathfindingHelper, inputDirectionComponent, targetCell);
               this.Send(new CharacterAudioTrigger(player, "walk"));
               World.GetElement<StairsConfirmation>().Popup();
               _messageLog?.AddMessage($"Found the stairs down.");
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