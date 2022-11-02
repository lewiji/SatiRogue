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

namespace SatiRogue.Ecs.Dungeon.Systems;

public class PlayerMovementSystem : CharacterMovementSystem {
   MessageLog? _messageLog;

   public override void Run() {
      InitialiseSystem();
      _messageLog ??= World.GetElement<MessageLog>();

      foreach (var (playerEntity, player, gridPos, input) in World.Query<Entity, Player, GridPositionComponent, InputDirectionComponent>().Build()) {
         Logger.Info($"Player Input received: {input.Direction}");
         if (input.Direction == Vector2.Zero)
            continue;

         var targetPos = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y);
         var targetCell = MapData!.GetCellAt(targetPos);

         if (!targetCell.Blocked && !targetCell.Occupied) {
            MoveToCell(player, gridPos, input, targetCell);
            World.GetElement<DebugUi>().SetPlayerPos(targetPos);
            World.Send(new CharacterAudioTrigger(player, "walk"));
         } else {
            HandleOccupants(targetCell, playerEntity, player, input, gridPos);
            World.GetElement<DebugUi>().SetPlayerPos(gridPos.Position);
            World.Send(new CharacterAudioTrigger(player, "sword"));
         }
      }
   }

   public void TeleportToCell(Player player, Vector3 position) {
      InitialiseSystem();
      
      if (!player.HasMeta("Entity") || player.GetMeta("Entity") is not Marshallable<Entity> entity) return;
      Logger.Info($"Teleporting entity {entity.Value}");
      var gridPos = World.GetComponent<GridPositionComponent>(entity.Value);
      World.GetComponent<Walkable>(entity.Value).Teleporting = true;
      HandleOccupants(MapData!.GetCellAt(position), entity.Value, player, 
         World.GetComponent<InputDirectionComponent>(entity.Value), gridPos);
      World.GetElement<DebugUi>().SetPlayerPos(gridPos.Position);
      World.Send(new CharacterAudioTrigger(player, "sword"));
      
      FogSystem.CalculateFov(gridPos, MapData, World.GetElement<FogMultiMeshes>());
   }

   void HandleOccupants(Cell targetCell,
      Entity playerEntity,
      Player player,
      InputDirectionComponent inputDirectionComponent,
      GridPositionComponent gridPos) {
      var occupantHandled = false;

      foreach (var targetId in targetCell.Occupants) {
         if (GD.InstanceFromId(targetId) is GameObject {Enabled: true} targetNode
             && (targetNode.GetMeta("Entity") as Marshallable<Entity>)?.Value is { } entity) {
            switch (targetNode) {
               case Character character when World.IsAlive(entity): {
                  var playerStats = World.GetComponent<Stats>(playerEntity).Record;
                  var enemyStats = World.GetComponent<Stats>(entity).Record;
                  var damage = Mathf.Max(0, playerStats.Strength - enemyStats.Defence);
                  World.GetComponent<HealthComponent>(entity).Value -= damage;
                  World.GetComponent<CharacterAnimationComponent>(playerEntity).Animation = "attack";
                  World.GetComponent<CharacterAnimationComponent>(entity).Animation = "hit";

                  if (player.AnimatedSprite3D != null) {
                     player.AnimatedSprite3D.FlipH = inputDirectionComponent.Direction.x switch {
                        < 0 => true,
                        > 0 => false,
                        _ => player.AnimatedSprite3D.FlipH
                     };
                  }
                  _messageLog?.AddMessage($"Hit {character.CharacterName} for {damage} damage.");
                  occupantHandled = true;
                  break;
               }
               case Chest chest when World.HasComponent<Closed>(entity):
                  chest.Open = true;
                  chest.BlocksCell = false;
                  World.On(entity).Remove<Closed>().Add<Open>();
                  chest.Enabled = false;
                  var goldAmount = 1;
                  var playerStore = World.GetElement<PersistentPlayerData>();
                  playerStore.Gold += goldAmount;
                  World.GetElement<Loot>().NumLoots = playerStore.Gold;
                  _messageLog?.AddMessage($"Retrieved {goldAmount} gold from chest.");
                  occupantHandled = true;
                  break;
               case Health {Taken: false} health:
                  health.Taken = true;
                  var healthAmount = 1;
                  World.GetComponent<HealthComponent>(((Marshallable<Entity>) player.GetMeta("Entity")).Value).Value += healthAmount;
                  _messageLog?.AddMessage($"Relic healed player {healthAmount} HP.");
                  occupantHandled = true;
                  break;
               case SpatialItem spatialItem when World.HasComponent<Collectable>(entity):
                  World.On(entity).Remove<Collectable>().Remove<GridPositionComponent>().Add<InInventory>().Add<JustPickedUp>();
                  spatialItem.BlocksCell = false;
                  spatialItem.Visible = false;
                  _messageLog?.AddMessage($"Picked up {spatialItem.Name} from the ground.");
                  occupantHandled = true;
                  break;
               case Stairs:
                  Logger.Info("Stairs!");
                  MoveToCell(player, gridPos, inputDirectionComponent, targetCell);
                  World.Send(new CharacterAudioTrigger(player, "walk"));
                  World.GetElement<StairsConfirmation>().Popup();
                  _messageLog?.AddMessage($"Found the stairs down.");
                  InputSystem.Paused = true;
                  return;
            }
         }
      }

      if (occupantHandled || targetCell.Blocked) return;
      // Default to MoveToCell action if no handled occupants were found and cell isn't blocked
      MoveToCell(player, gridPos, inputDirectionComponent, targetCell);
      World.Send(new CharacterAudioTrigger(player, "walk"));
   }
}