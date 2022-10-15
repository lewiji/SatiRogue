using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using SatiRogue.Ecs.Dungeon.Nodes.Items;
using SatiRogue.Ecs.Dungeon.Systems.Init;
using SatiRogue.Ecs.Dungeon.Triggers;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.MapGenerator.Systems;
using SatiRogue.Tools;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class PlayerMovementSystem : CharacterMovementSystem {
   MessageLog? _messageLog;

   public override void Run() {
      InitialiseSystem();
      _messageLog ??= World.GetElement<MessageLog>();

      foreach (var (playerEntity, player, gridPos, input) in this.Query<Entity, Player, GridPositionComponent, InputDirectionComponent>()) {
         
         Logger.Info($"Player Input received: {input.Direction}");
         if (input.Direction == Vector2.Zero)
            continue;

         var targetPos = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y);
         var targetCell = MapData!.GetCellAt(targetPos);

         if (!targetCell.Blocked && !targetCell.Occupied) {
            MoveToCell(player, gridPos, input, targetCell);
            World.GetElement<DebugUi>().SetPlayerPos(targetPos);
            this.Send(new CharacterAudioTrigger(player, "walk"));
         } else {
            HandleOccupants(targetCell, playerEntity, player, input, gridPos);
            World.GetElement<DebugUi>().SetPlayerPos(gridPos.Position);
            this.Send(new CharacterAudioTrigger(player, "sword"));
         }
      }
   }

   public void TeleportToCell(Player player, Vector3 position) {
      InitialiseSystem();
      
      if (!player.HasMeta("Entity") || player.GetMeta("Entity") is not Marshallable<Entity> entity) return;
      Logger.Info($"Teleporting entity {entity.Value.Identity}");
      var gridPos = World.GetComponent<GridPositionComponent>(entity.Value.Identity);
      World.GetComponent<Walkable>(entity.Value.Identity).Teleporting = true;
      HandleOccupants(MapData!.GetCellAt(position), entity.Value, player, 
         World.GetComponent<InputDirectionComponent>(entity.Value.Identity), gridPos);
      World.GetElement<DebugUi>().SetPlayerPos(gridPos.Position);
      this.Send(new CharacterAudioTrigger(player, "sword"));
      
      FogSystem.CalculateFov(gridPos, MapData, World.GetElement<FogMultiMeshes>());
   }

   void HandleOccupants(Cell targetCell,
      Entity playerEntity,
      Player player,
      InputDirectionComponent inputDirectionComponent,
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
               World.GetComponent<CharacterAnimationComponent>(playerEntity.Identity).Animation = "attack";
               World.GetComponent<CharacterAnimationComponent>(identity.Value).Animation = "hit";

               if (player.AnimatedSprite3D != null) {
                  player.AnimatedSprite3D.FlipH = inputDirectionComponent.Direction.x switch {
                     < 0 => true,
                     > 0 => false,
                     _ => player.AnimatedSprite3D.FlipH
                  };
               }
               _messageLog?.AddMessage($"Hit {character.CharacterName} for {damage} damage.");
               break;
            }
            case Chest chest when World.HasComponent<Closed>(identity.Value):
               chest.Open = true;
               chest.BlocksCell = false;
               this.On(entity!.Value).Remove<Closed>().Add<Open>();
               chest.Enabled = false;
               var goldAmount = 1;
               var playerStore = World.GetElement<PersistentPlayerData>();
               playerStore.Gold += goldAmount;
               World.GetElement<Loot>().NumLoots = playerStore.Gold;
               _messageLog?.AddMessage($"Retrieved {goldAmount} gold from chest.");
               break;
            case Health {Taken: false} health:
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
               MoveToCell(player, gridPos, inputDirectionComponent, targetCell);
               this.Send(new CharacterAudioTrigger(player, "walk"));
               World.GetElement<StairsConfirmation>().Popup();
               _messageLog?.AddMessage($"Found the stairs down.");
               InputSystem.Paused = true;
               return;
            default:
               MoveToCell(player, gridPos, inputDirectionComponent, targetCell);
               this.Send(new CharacterAudioTrigger(player, "walk"));
               return;
         }
      }
   }
}