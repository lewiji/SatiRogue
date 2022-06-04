using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using SatiRogue.Grid.MapGen;
using SatiRogue.MathUtils;

namespace SatiRogue.Grid;


public class MapData : AbstractMapData {
   
   public readonly Stack<Vector3> CellsVisibilityChanged = new();
   protected readonly Dictionary<long, int> CellIdToAStarId = new();
   public AStar AStar { get; protected set; } = new();
   
   private static readonly Vector3i[] Offsets = {
      Vector3i.Back, Vector3i.Forward, Vector3i.Left, Vector3i.Right, Vector3i.Back + Vector3i.Left,
      Vector3i.Back + Vector3i.Right, Vector3i.Forward + Vector3i.Left, Vector3i.Forward + Vector3i.Right
   };
   
   public MapData(MapGenMapData generatedMapData) {
      IndexedCells = generatedMapData.IndexedCells;
      InitialiseAStar();
   }
   public Cell[] Cells => IndexedCells.Values.ToArray();

   public Cell GetCellById(long cellId)
   {
      return IndexedCells[cellId];
   }

   private void InitialiseAStar() {
      AStar.ReserveSpace(IndexedCells.Count);
      foreach (var keyValuePair in IndexedCells) {
         var aStarId = AStar.GetAvailablePointId();
         AStar.AddPoint(aStarId, keyValuePair.Value.Position.ToVector3());
         CellIdToAStarId.Add(keyValuePair.Key, aStarId);
         keyValuePair.Value.Connect(nameof(Cell.CellTypeChanged), this, nameof(OnCellTypeChanged));
      }
      
      var points = AStar.GetPoints();
      foreach (int point in points)
      foreach (var offset in Offsets) {
         var neighbourVec = new Vector3i(AStar.GetPointPosition(point)) + offset;
         if (CellIdToAStarId.TryGetValue(IdCalculator.IdFromVec3(neighbourVec), out var neighbourId))
            AStar.ConnectPoints(point, neighbourId);
      }
   }
   
   private void OnCellTypeChanged(int typeId, long cellId) {
      var typeEnum = (CellType)typeId;
      if (typeEnum == CellType.Wall) {
         BlockAStarCell(cellId);
      }
   }

   private void BlockAStarCell(long id) {
      if (CellIdToAStarId.TryGetValue(id, out var toBlockId)) AStar.SetPointDisabled(toBlockId);
   }
   
   public Vector3[] FindPath(Vector3i from, Vector3i? to) {
      if (!to.HasValue) return new Vector3[] { };
      var idFrom = CellIdToAStarId[IdCalculator.IdFromVec3(from)];
      var idTo = CellIdToAStarId[IdCalculator.IdFromVec3(to.Value)];
      return AStar.GetPointPath(idFrom, idTo);
   }
  
   public void SetLight(Vector3i gridVec, float f) {
      var cell = GetCellAt(gridVec);
      if (cell.Luminosity == null)
      {
         CellsVisibilityChanged.Push(cell.Position.ToVector3());
         cell.Luminosity = f;
      }
   }

   public override void ClearCells() {
      base.ClearCells();
      AStar.Clear();
   }

}