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

public class CharacterMovementSystem : Reference, ISystem {
   public World World { get; set; } = null!;

   protected MapGenData? MapData;
   PathfindingHelper? _pathfindingHelper;

   public virtual void Run() {
      InitialiseSystem();

      foreach (var (character, gridPos, input) in World.Query<Character, GridPositionComponent, InputDirectionComponent>().Not<Controllable>().Build()) {
         if (input.Direction == Vector2.Zero)
            continue;

         var targetPos = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y);
         var targetCell = MapData!.GetCellAt(targetPos);

         if (targetCell.Blocked)
            continue;

         MoveToCell(character, gridPos, input, targetCell);
      }
   }

   protected void InitialiseSystem() {
      MapData ??= World.GetElement<MapGenData>();
      _pathfindingHelper ??= World.GetElement<PathfindingHelper>();
   }

   public void TeleportToCell(Character character, Vector3 position) {
      InitialiseSystem();
      if (!character.HasMeta("Entity") || character.GetMeta("Entity") is not Marshallable<Entity> entity) return;
      Logger.Info($"Teleporting entity {entity.Value.Identity}");
      
      World.GetComponent<Walkable>(entity.Value).Teleporting = true;
      
      MoveToCell(character, 
         World.GetComponent<GridPositionComponent>(entity.Value), 
         World.GetComponent<InputDirectionComponent>(entity.Value), 
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

      if (character.HasMeta("Entity") && character.GetMeta("Entity") is Marshallable<Entity> entity) {
         if (!World.HasComponent<Moving>(entity.Value)) 
            World.AddComponent<Moving>(entity.Value);
         
         if (character.Visible) {
            SendWalkAnimation(World.GetComponent<CharacterAnimationComponent>(entity.Value));
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