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
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public partial class PlayerMovementSystem : CharacterMovementSystem {
   MessageLog? _messageLog;
   public override void Run(World world) {
      World ??= world;
      InitialiseSystem(world);
      _messageLog ??= world.GetElement<MessageLog>();

      foreach (var (playerEntity, player, gridPos, input) in world.Query<Entity, Player, GridPositionComponent, InputDirectionComponent>().Build()) {
         Logger.Info($"Player Input received: {input.Direction}");
         if (input.Direction == Vector2.Zero)
            continue;

         var targetPos = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y);
         var targetCell = MapData!.GetCellAt(targetPos);

         if (!targetCell.Blocked && !targetCell.Occupied) {
            MoveToCell(player, gridPos, input, targetCell);
            world.Send(new CharacterAudioTrigger(player, "walk"));
         } else {
            HandleOccupants(targetCell, playerEntity, player, input, gridPos);
            world.Send(new CharacterAudioTrigger(player, "sword"));
         }
      }
   }

   public void TeleportToCell(Player player, Vector3 position) {
      InitialiseSystem(World);
      
      if (!player.HasMeta("Entity") || player.GetEntity() is not { } entity) return;
      Logger.Info($"Teleporting entity {entity}");
      var gridPos = World!.GetComponent<GridPositionComponent>(entity);
      World!.GetComponent<Walkable>(entity).Teleporting = true;
      HandleOccupants(MapData!.GetCellAt(position), entity, player, 
         World!.GetComponent<InputDirectionComponent>(entity), gridPos);
      World!.Send(new CharacterAudioTrigger(player, "sword"));
      
      FogSystem.CalculateFov(gridPos, MapData, World!.GetElement<FogMultiMeshes>());
   }

   void HandleOccupants(Cell targetCell,
      Entity playerEntity,
      Player player,
      InputDirectionComponent inputDirectionComponent,
      GridPositionComponent gridPos) {
      var occupantHandled = false;

      foreach (var targetId in targetCell.Occupants) {
         if (GD.InstanceFromId(targetId) is GameObject {Enabled: true} targetNode
             && targetNode.GetEntity() is { } entity) {
            
            switch (targetNode) {
               case Character when World!.IsAlive(entity): {
                  World!.AddComponent(entity, new Attacked(playerEntity));
                  occupantHandled = true;
                  break;
               }
               case Chest when World!.HasComponent<Closed>(entity):
                  World!.AddComponent(entity, new OpeningContainer());
                  occupantHandled = true;
                  break;
               case Health {Taken: false} health:
                  health.Taken = true;
                  var healthAmount = 1;
                  World!.GetComponent<HealthComponent>(((Marshallable<Entity>) player.GetMeta("Entity")).Value).Value += healthAmount;
                  _messageLog?.AddMessage($"Relic healed player {healthAmount} HP.");
                  occupantHandled = true;
                  break;
               case SpatialItem spatialItem when World!.HasComponent<Collectable>(entity):
                  World!.On(entity).Remove<Collectable>().Remove<GridPositionComponent>().Add<InInventory>().Add<JustPickedUp>();
                  spatialItem.BlocksCell = false;
                  spatialItem.Visible = false;
                  _messageLog?.AddMessage($"Picked up {spatialItem.Name} from the ground.");
                  occupantHandled = true;
                  break;
               case Stairs:
                  Logger.Info("Stairs!");
                  MoveToCell(player, gridPos, inputDirectionComponent, targetCell);
                  World!.Send(new CharacterAudioTrigger(player, "walk"));
                  World!.GetElement<StairsConfirmation>().Popup();
                  _messageLog?.AddMessage($"Found the stairs down.");
                  InputSystem.Paused = true;
                  return;
            }
         }
      }

      if (occupantHandled || targetCell.Blocked) return;
      // Default to MoveToCell action if no handled occupants were found and cell isn't blocked
      MoveToCell(player, gridPos, inputDirectionComponent, targetCell);
      World!.Send(new CharacterAudioTrigger(player, "walk"));
   }
}