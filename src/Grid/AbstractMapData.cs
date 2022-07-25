using System.Collections.Generic;
using Godot;
using SatiRogue.MathUtils;

namespace SatiRogue.Grid; 

public abstract class AbstractMapData : Resource {
   public Dictionary<long, Cell> IndexedCells { get; protected set; } = new();
   /**
     * Adds new cell if nonexistant at position, and returns the new cell.
     * Returns existing cell if found.
     * Returns null if something weird happened.
     */
   private Cell InitialiseOrGetCell(Vector3i position) {
      return InitialiseOrGetCell(IdCalculator.IdFromVec3i(position), position);
   }

   private Cell InitialiseOrGetCell(long id, Vector3i position) {
      // Try to add id to collection, if already exists, return matching cell struct
      if (IndexedCells.ContainsKey(id))
         return IndexedCells[id];
      // create and add new cell otherwise
      var cell = new Cell(id);
      IndexedCells[id] = cell;
      return cell;
   }
   
   /**
     * Get cell by Vector3 position
     */
   public Cell GetCellAt(Vector3i position) {
      return InitialiseOrGetCell(position);
   }
   
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
   public virtual void ClearCells() {
      IndexedCells.Clear();
   }
   
   public bool IsWall(Vector3i gridVec) {
      return GetCellAt(gridVec).Type == CellType.Wall;
   }
}