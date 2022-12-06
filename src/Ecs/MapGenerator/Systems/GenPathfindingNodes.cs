using System;
using System.Collections.Generic;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using RelEcs;
using SatiRogue.Tools;
using World = RelEcs.World;
namespace SatiRogue.Ecs.MapGenerator.Systems;

public partial class PathfindingHelper {
   readonly AStar3D _aStar;
   readonly Dictionary<long, long> _cellIdToAStarId;

   public PathfindingHelper(AStar3D aStar, Dictionary<long, long> cellIdToAStarId) {
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

public partial class GenPathfindingNodes : ISystem {
   
   static readonly Vector3[] Offsets = {
      Vector3.Back, Vector3.Forward, Vector3.Left, Vector3.Right, Vector3.Back + Vector3.Left,
      Vector3.Back + Vector3.Right, Vector3.Forward + Vector3.Left, Vector3.Forward + Vector3.Right
   };

   public void Run(World world) {
      var cellIdToAStarId = new Dictionary<long, long>();
      var mapGenData = world.GetElement<MapGenData>();
      var aStar = new AStar3D();
      aStar.ReserveSpace(mapGenData.IndexedCells.Count);

      foreach (var keyValuePair in mapGenData.IndexedCells) {
         var aStarId = aStar.GetAvailablePointId();
         aStar.AddPoint(aStarId, keyValuePair.Value.Position);
         cellIdToAStarId.Add(keyValuePair.Key, aStarId);
         if (keyValuePair.Value.Type == CellType.Wall) aStar.SetPointDisabled(aStarId);
      }

      var points = aStar.GetPointIds();

      foreach (int point in points) {
         foreach (var offset in Offsets) {
            var neighbourVec = aStar.GetPointPosition(point) + offset;

            if (cellIdToAStarId.TryGetValue(IdCalculator.IdFromVec3(neighbourVec), out var neighbourId))
               aStar.ConnectPoints(point, neighbourId);
         }
      }

      world.AddOrReplaceElement(new PathfindingHelper(aStar, cellIdToAStarId));
   }
}