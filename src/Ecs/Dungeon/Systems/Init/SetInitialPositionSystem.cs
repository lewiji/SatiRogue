using System.Linq;
using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.Dungeon.Nodes.Items;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.MapGenerator.Systems;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems.Init;

public class SetInitialPositionSystem : ISystem {
   

   public void Run(World world) {
      var mapData = world.GetElement<MapGenData>();
      var pathfindingHelper = world.GetElement<PathfindingHelper>();

      var availableCells = mapData.IndexedCells.Where(c => !c.Value.Blocked).ToArray();

      var stairsQuery = world.Query<Stairs, GridPositionComponent>().Build();
      foreach (var (stairs, gridPos) in stairsQuery) {
         var chosenCell = availableCells[(int) (GD.Randi() % availableCells.Length)];

         var limit = 32;

         while (chosenCell.Value.Occupied && limit > 0) {
            chosenCell = availableCells[(int) (GD.Randi() % availableCells.Length)];
            limit -= 1;
         }

         if (limit == 0) {
            break;
         }
         gridPos.Position = chosenCell.Value.Position;
         chosenCell.Value.Occupants.Add(stairs.GetInstanceId());
         pathfindingHelper.SetCellWeight(chosenCell.Value.Id, chosenCell.Value.Occupants.Count);
         stairs.Translation = gridPos.Position;
         
         world.TryGetElement<DebugUi>()?.SetStairsPos(gridPos.Position);
         Logger.Info($"Spawned stairs at {stairs.Translation}");

         // Make hole for stairs to sit in in floor
         /*var maxWidth = mapData.GeneratorParameters.Width;
         var chunkWidth = mapData.GeneratorParameters.Width.Factors().GetMedian();
         var chunkId = SpatialMapSystem.GetChunkIdForPosition(gridPos.Position, chunkWidth, maxWidth);
         var mapGeometry = world.GetElement<MapGeometry>();
         var chunkSpatial = mapGeometry.GetNode<Spatial>($"Chunk{chunkId}");
         var floorMultiMesh = chunkSpatial.GetChild<MultiMeshInstance>(0);
         var instanceId = -1;
         var localGridPos = chunkSpatial.ToLocal(gridPos.Position);
         var instanceCount = floorMultiMesh.Multimesh.InstanceCount;

         for (var i = 0; i < instanceCount; i++) {
            var tileTransform = floorMultiMesh.Multimesh.GetInstanceTransform(i);

            if (tileTransform.origin.IsEqualApprox(localGridPos)) {
               instanceId = i;
               break;
            }
         }

         if (instanceId > -1) {
            Logger.Info(
               $"Found floor tile {instanceId} at {chunkSpatial.ToGlobal(floorMultiMesh.Multimesh.GetInstanceTransform(instanceId).origin)}");
            floorMultiMesh.Multimesh.SetInstanceTransform(instanceId, new Transform(Basis.Identity, new Vector3(-1000f, -1000f, -1000f)));
            Logger.Info($"Removed floor tile {instanceId} from chunk {chunkId}.");
         } else {
            Logger.Warn($"Failed to find floor tile at {localGridPos} ({gridPos.Position})");
         }*/
      }

      var query = world.Query<Character, GridPositionComponent>().Build();
      foreach (var (character, gridPos) in query) {
         if (!availableCells.Any())
            break;
         var chosenCell = availableCells[(int) (GD.Randi() % availableCells.Length)];
         var limit = 32;

         while (chosenCell.Value.Occupied && limit > 0) {
            chosenCell = availableCells[(int) (GD.Randi() % availableCells.Length)];
            limit -= 1;
         }

         if (limit == 0) {
            break;
         }

         gridPos.Position = chosenCell.Value.Position;
         if (character.GetEntity() is {} entity) world.AddComponent<Moving>(entity);

         chosenCell.Value.Occupants.Add(character.GetInstanceId());
         pathfindingHelper.SetCellWeight(chosenCell.Value.Id, chosenCell.Value.Occupants.Count);
      }

      var itemQuery = world.Query<Item, GridPositionComponent>().Build();
      foreach (var (item, gridPos) in itemQuery) {
         if (!availableCells.Any())
            break;
         var chosenCell = availableCells[(int) (GD.Randi() % availableCells.Length)];
         var limit = 32;

         while (chosenCell.Value.Occupied && limit > 0) {
            chosenCell = availableCells[(int) (GD.Randi() % availableCells.Length)];
            limit -= 1;
         }

         if (limit == 0) {
            break;
         }
         gridPos.Position = chosenCell.Value.Position;
         chosenCell.Value.Occupants.Add(item.GetInstanceId());
         pathfindingHelper.SetCellWeight(chosenCell.Value.Id, chosenCell.Value.Occupants.Count);
         item.Translation = gridPos.Position;
      }

      var propQuery = world.Query<Prop, GridPositionComponent>().Build();
      foreach (var (prop, gridPos) in propQuery) {
         if (!availableCells.Any())
            break;
         var chosenCell = availableCells[(int) (GD.Randi() % availableCells.Length)];
         var limit = 32;

         while (chosenCell.Value.Occupied && limit > 0) {
            chosenCell = availableCells[(int) (GD.Randi() % availableCells.Length)];
            limit -= 1;
         }

         if (limit == 0) {
            break;
         }
         gridPos.Position = chosenCell.Value.Position;
         chosenCell.Value.Occupants.Add(prop.GetInstanceId());
         pathfindingHelper.SetCellWeight(chosenCell.Value.Id, chosenCell.Value.Occupants.Count);
         prop.Translation = gridPos.Position;
      }
   }
}