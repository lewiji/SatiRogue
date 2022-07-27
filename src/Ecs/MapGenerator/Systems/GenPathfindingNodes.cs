using System.Collections.Generic;
using Godot;
using RelEcs;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Grid;
using CellType = SatiRogue.Ecs.MapGenerator.Components.CellType;

namespace SatiRogue.Ecs.MapGenerator.Systems;

public class PathfindingHelper {
   private AStar _aStar;
   private readonly Dictionary<long, int> _cellIdToAStarId;
   public PathfindingHelper(AStar aStar, Dictionary<long, int> cellIdToAStarId) {
      _aStar = aStar;
      _cellIdToAStarId = cellIdToAStarId;
   }
   
   public Vector3[] FindPath(Vector3 from, Vector3 to) {
      var idFrom = _cellIdToAStarId[IdCalculator.IdFromVec3(from)];
      var idTo = _cellIdToAStarId[IdCalculator.IdFromVec3(to)];
      return _aStar.GetPointPath(idFrom, idTo);
   }

   public void SetCellWeight(Vector3 pos, float weight = 0f) 
      => SetCellWeight(IdCalculator.IdFromVec3(pos), weight);
   
   public void SetCellWeight(long cellId, float weight = 0f) {
      _aStar.SetPointWeightScale(_cellIdToAStarId[cellId], weight);
   }

   public void SetCellDisabled(Vector3 pos, bool disabled = true) 
      => SetCellDisabled(IdCalculator.IdFromVec3(pos), disabled);
   
   public void SetCellDisabled(long cellId, bool disabled = true) {
      _aStar.SetPointDisabled(_cellIdToAStarId[cellId], disabled);
   }
}

public class GenPathfindingNodes : GDSystem {
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