using System.Collections.Generic;
using Godot;
using SatiRogue.Math;

namespace SatiRogue.Grid;

public class MapData {
   private readonly Dictionary<long, Cell> _cells = new();
   public AStar AStar = new();
   public Dictionary<long, int> CellIdToAStarId = new();
   public List<Vector3> CellsVisibilityChanged = new();

   public MapData(int width, int height) {
      AStar.ReserveSpace(width * height);
   }

   public IEnumerable<Cell> Cells => _cells.Values;

   /**
     * Adds new cell if nonexistant at position, and returns the new cell.
     * Returns existing cell if found.
     * Returns null if something weird happened.
     */
   private Cell InitialiseOrGetCell(Vector3i position) {
      return InitialiseOrGetCell(IdCalculator.IdFromVec3(position), position);
   }

   private Cell InitialiseOrGetCell(long id, Vector3i position) {
      // Try to add id to collection, if already exists, return matching cell struct
      if (_cells.ContainsKey(id))
         return _cells[id];

      // create and add new cell otherwise
      var cell = new Cell(id);
      _cells[id] = cell;
      var aStarId = AStar.GetAvailablePointId();
      AStar.AddPoint(aStarId, position.ToVector3());
      CellIdToAStarId.Add(id, aStarId);
      return cell;
   }

   public void ConnectPathfindingPoints() {
      var offsets = new[] {
         Vector3i.Back, Vector3i.Forward, Vector3i.Left, Vector3i.Right, Vector3i.Back + Vector3i.Left,
         Vector3i.Back + Vector3i.Right, Vector3i.Forward + Vector3i.Left, Vector3i.Forward + Vector3i.Right
      };

      var points = AStar.GetPoints();
      foreach (int point in points)
      foreach (var offset in offsets) {
         var neighbourVec = new Vector3i(AStar.GetPointPosition(point)) + offset;
         if (CellIdToAStarId.TryGetValue(IdCalculator.IdFromVec3(neighbourVec), out var neighbourId)) AStar.ConnectPoints(point, neighbourId);
      }
   }

   public void BlockCell(long id) {
      if (CellIdToAStarId.TryGetValue(id, out var toBlockId)) AStar.SetPointDisabled(toBlockId);
   }

   public Vector3[] FindPath(Vector3i from, Vector3i to) {
      var idFrom = CellIdToAStarId[IdCalculator.IdFromVec3(from)];
      var idTo = CellIdToAStarId[IdCalculator.IdFromVec3(to)];
      return AStar.GetPointPath(idFrom, idTo);
   }

   /**
     * Get cell by Vector3 position
     */
   public Cell GetCellAt(Vector3i position) {
      return InitialiseOrGetCell(position);
   }

   /**
     * Get cell by exact id
     */
   /*public Cell GetCellById(long id) {
       return InitialiseOrGetCell(id);
   }*/
   public Cell SetCellType(Vector3i position, CellType? type) {
      return InitialiseOrGetCell(position).SetCellType(type);
   }

   public Cell SetCellVisibility(Vector3i position, CellVisibility? visibility) {
      return InitialiseOrGetCell(position).SetCellVisibility(visibility);
   }

   /**
     * Add occupant to cell at specified position, and return that cell
     */
   public Cell AddOccupant(Vector3i position, ulong uid) {
      return InitialiseOrGetCell(position).AddOccupant(uid);
   }

   /**
     * Remove occupant from cell at specified position, and return that cell
     */
   public Cell RemoveOccupant(Vector3i position, ulong uid) {
      return InitialiseOrGetCell(position).RemoveOccupant(uid);
   }

   /**
     * Add condition to cell at specified position, and return that cell
     */
   public Cell AddCondition(Vector3i position, CellCondition condition) {
      return InitialiseOrGetCell(position).AddCondition(condition);
   }

   /**
     * Remove condition from cell at specified position, and return that cell
     */
   public Cell RemoveCondition(Vector3i position, CellCondition condition) {
      return InitialiseOrGetCell(position).RemoveCondition(condition);
   }

   /**
     * Clear all cells and empty grid data.
     */
   public void ClearCells() {
      _cells.Clear();
      AStar.Clear();
   }

   public void SetLight(Vector3i gridVec, float f) {
      var cell = GetCellAt(gridVec);
      if (cell.Luminosity == null) CellsVisibilityChanged.Add(cell.Position.ToVector3());
      cell.Luminosity = f;
   }

   public bool IsWall(Vector3i gridVec) {
      return GetCellAt(gridVec).Blocked;
   }
}