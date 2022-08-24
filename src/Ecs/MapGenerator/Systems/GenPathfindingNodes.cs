using System;
using System.Collections.Generic;
using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using CellType = SatiRogue.Ecs.MapGenerator.Components.CellType;
namespace SatiRogue.Ecs.MapGenerator.Systems;

public class PathfindingHelper {
   private readonly AStar _aStar;
   private readonly Dictionary<long, int> _cellIdToAStarId;

   public PathfindingHelper(AStar aStar, Dictionary<long, int> cellIdToAStarId) {
      _aStar = aStar;
      _cellIdToAStarId = cellIdToAStarId;
   }

   public Vector3[] FindPath(Vector3 from, Vector3 to) {
      if (_cellIdToAStarId.TryGetValue(IdCalculator.IdFromVec3(from), out var idFrom)
          && _cellIdToAStarId.TryGetValue(IdCalculator.IdFromVec3(to), out var idTo))
         return _aStar.GetPointPath(idFrom, idTo);

      Logger.Warn($"FindPath tried to access invalid coords, from: {from}, to: {to}");

      return Array.Empty<Vector3>();
   }

   public void SetCellWeight(Vector3 pos, float weight = 0f) {
      SetCellWeight(IdCalculator.IdFromVec3(pos), weight);
   }

   public void SetCellWeight(long cellId, float weight = 0f) {
      if (_cellIdToAStarId.TryGetValue(cellId, out var point))
         _aStar.SetPointWeightScale(point, weight);
      else
         Logger.Warn($"SetCellWeight tried to access non-existent cell, id: {cellId}");
   }

   public void SetCellDisabled(Vector3 pos, bool disabled = true) {
      SetCellDisabled(IdCalculator.IdFromVec3(pos), disabled);
   }

   public void SetCellDisabled(long cellId, bool disabled = true) {
      if (_cellIdToAStarId.TryGetValue(cellId, out var point))
         _aStar.SetPointDisabled(point, disabled);
      else
         Logger.Warn($"SetCellDisabled tried to access non-existent cell, id: {cellId}");
   }
}

public class GenPathfindingNodes : GdSystem {
   private static readonly Vector3[] Offsets = {
      Vector3.Back, Vector3.Forward, Vector3.Left, Vector3.Right, Vector3.Back + Vector3.Left,
      Vector3.Back + Vector3.Right, Vector3.Forward + Vector3.Left, Vector3.Forward + Vector3.Right
   };

   public override void Run() {
      var cellIdToAStarId = new Dictionary<long, int>();
      var mapGenData = GetElement<MapGenData>();
      var aStar = new AStar();
      aStar.ReserveSpace(mapGenData.IndexedCells.Count);

      foreach (var keyValuePair in mapGenData.IndexedCells) {
         var aStarId = aStar.GetAvailablePointId();
         aStar.AddPoint(aStarId, keyValuePair.Value.Position);
         cellIdToAStarId.Add(keyValuePair.Key, aStarId);
         if (keyValuePair.Value.Type == CellType.Wall) aStar.SetPointDisabled(aStarId);
      }

      var points = aStar.GetPoints();

      foreach (int point in points) {
         foreach (var offset in Offsets) {
            var neighbourVec = new Vector3(aStar.GetPointPosition(point)) + offset;

            if (cellIdToAStarId.TryGetValue(IdCalculator.IdFromVec3(neighbourVec), out var neighbourId))
               aStar.ConnectPoints(point, neighbourId);
         }
      }

      AddElement(new PathfindingHelper(aStar, cellIdToAStarId));
   }
}