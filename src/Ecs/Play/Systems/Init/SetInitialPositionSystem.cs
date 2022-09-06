using System.Linq;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.MapGenerator.Systems;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Nodes;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.Ecs.Play.Nodes.Items;
using SatiRogue.lib.RelEcsGodot.src;
using SatiRogue.Tools;
namespace SatiRogue.Ecs.Play.Systems.Init;

public class SetInitialPositionSystem : GdSystem {
   public override void Run() {
      var mapData = GetElement<MapGenData>();
      var pathfindingHelper = GetElement<PathfindingHelper>();

      var availableCells = mapData.IndexedCells.Where(c => !c.Value.Blocked).ToArray();

      var stairsQuery = Query<Stairs, GridPositionComponent>();

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
         Logger.Info($"Spawned stairs at {stairs.Translation}");

         // Make hole for stairs to sit in in floor
         var maxWidth = mapData.GeneratorParameters.Width;
         var chunkWidth = mapData.GeneratorParameters.Width.Factors().GetMedian();
         var chunkId = SpatialMapSystem.GetChunkIdForPosition(gridPos.Position, chunkWidth, maxWidth);
         var mapGeometry = GetElement<MapGeometry>();
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
         }
      }

      var query = Query<Character, GridPositionComponent>();

      foreach (var (character, gridPos) in query) {
         if (!availableCells.Any()) break;
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
         chosenCell.Value.Occupants.Add(character.GetInstanceId());
         pathfindingHelper.SetCellWeight(chosenCell.Value.Id, chosenCell.Value.Occupants.Count);
      }

      var itemQuery = Query<Item, GridPositionComponent>();

      foreach (var (item, gridPos) in itemQuery) {
         if (!availableCells.Any()) break;
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
   }
}