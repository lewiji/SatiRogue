using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.Dungeon.Triggers;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.MapGenerator.Systems;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public partial class CharacterMovementSystem : RefCounted, ISystem {
   

   protected MapGenData? MapData;
   PathfindingHelper? _pathfindingHelper;
   protected World? World;
   
   public virtual void Run(World world)
   {
      World ??= world;
      InitialiseSystem(world);

      foreach (var (character, gridPos, input) in world.Query<Character, GridPositionComponent, InputDirectionComponent>().Not<Controllable>().Build()) {
         if (input.Direction == Vector2.Zero)
            continue;

         var targetPos = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y);
         var targetCell = MapData!.GetCellAt(targetPos);

         if (targetCell.Blocked)
            continue;

         MoveToCell(character, gridPos, input, targetCell);
      }
   }

   protected void InitialiseSystem(World? world) {
      MapData ??= world?.GetElement<MapGenData>();
      _pathfindingHelper ??= world?.GetElement<PathfindingHelper>();
   }

   public void TeleportToCell(Character character, Vector3 position) {
      InitialiseSystem(World);
      if (!character.HasMeta("Entity") || character.GetEntity() is not { } entity) return;
      Logger.Info($"Teleporting entity {entity.Identity}");
      
      World!.GetComponent<Walkable>(entity).Teleporting = true;
      
      MoveToCell(character, 
         World!.GetComponent<GridPositionComponent>(entity), 
         World!.GetComponent<InputDirectionComponent>(entity), 
         MapData!.GetCellAt(position));
   }

   protected void MoveToCell(Character character, GridPositionComponent gridPos, InputDirectionComponent inputDirectionComponent, Cell targetCell) {
      // Move character to new cell, remove from old cell occupants, recalculate cell weight
      var currentCell = MapData!.GetCellAt(gridPos.Position);

      gridPos.LastPosition = gridPos.Position;
      gridPos.Position = targetCell.Position;

      currentCell.Occupants.Remove(character.GetInstanceId());
      targetCell.Occupants.Add(character.GetInstanceId());
      character.CurrentCell = targetCell;
      _pathfindingHelper?.SetCellWeight(currentCell.Id, currentCell.Occupants.Count);
      _pathfindingHelper?.SetCellWeight(targetCell.Id, targetCell.Occupants.Count);

      if (character.HasMeta("Entity") && character.GetEntity() is { } entity) {
         if (!World!.HasComponent<Moving>(entity)) 
            World!.AddComponent<Moving>(entity);
         
         if (character.Visible) {
            SendWalkAnimation(World!.GetComponent<CharacterAnimationComponent>(entity));
         }
      }

      

      if (character.AnimatedSprite3D != null) {
         character.AnimatedSprite3D.FlipH = inputDirectionComponent.Direction.x switch {
            < 0 => true,
            > 0 => false,
            _ => character.AnimatedSprite3D.FlipH
         };
      }
   }

   protected virtual void SendWalkAnimation(CharacterAnimationComponent animationComponent) {
      animationComponent.Animation = "walk";
   }
}