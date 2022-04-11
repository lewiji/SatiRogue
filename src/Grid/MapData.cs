using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Ninject.Infrastructure.Language;
using RoguelikeMono.Math;

namespace RoguelikeMono.Grid;

public class MapData
{
    public IEnumerable<Cell> Cells { get => _cells.Values; }
    private Dictionary<long, Cell> _cells = new ();

    /**
     * Adds new cell if nonexistant at position, and returns the new cell.
     * Returns existing cell if found.
     * Returns null if something weird happened.
     */
    private Cell InitialiseOrGetCell(Vector3i position) => InitialiseOrGetCell(IdCalculator.IdFromVec3(position));
    private Cell InitialiseOrGetCell(long id)
    {
        // Try to add id to collection, if already exists, return matching cell struct
        if (_cells.ContainsKey(id)) 
            return _cells[id];
        
        // create and add new cell otherwise
        var cell = new Cell(id);
        _cells[id] = cell;
        return cell;
    }

    
    /** Get cell by Vector3 position */
    public Cell GetCellAt(Vector3i position) => InitialiseOrGetCell(position);
    /** Get cell by exact id */
    public Cell GetCellById(long id) => InitialiseOrGetCell(id);

    public Cell SetCellType(Vector3i position, CellType? type) => InitialiseOrGetCell(position).SetCellType(type);

    /** Add occupant to cell at specified position, and return that cell */
    public Cell AddOccupant(Vector3i position, ulong uid) => InitialiseOrGetCell(position).AddOccupant(uid);
    /** Remove occupant from cell at specified position, and return that cell */
    public Cell RemoveOccupant(Vector3i position, ulong uid) => InitialiseOrGetCell(position).RemoveOccupant(uid);
    
    /** Add condition to cell at specified position, and return that cell */
    public Cell AddCondition(Vector3i position, CellCondition condition) => InitialiseOrGetCell(position).AddCondition(condition);
    /** Remove condition from cell at specified position, and return that cell */
    public Cell RemoveCondition(Vector3i position, CellCondition condition) => InitialiseOrGetCell(position).RemoveCondition(condition);

    /**
     * Clear all cells and empty grid data.
     */
    public void ClearCells()
    {
        _cells.Clear();
    }
}