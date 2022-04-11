using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using RoguelikeMono.Math;

namespace RoguelikeMono.Grid;

public enum CellCondition { Frozen, Aflame, Wet, Destroyed }
public enum CellType { Floor, Wall, DoorClosed, DoorOpen, Stairs, Void }

public class Cell
{
    public long Id;
    public Vector3i Position { get => IdCalculator.Vec3FromId(Id); }
    public CellType? CellType;
    public HashSet<ulong> Occupants = new ();
    public HashSet<CellCondition> Conditions = new HashSet<CellCondition>();
    public bool Blocked => 
        Conditions.Contains(CellCondition.Destroyed) || 
        CellType is Grid.CellType.Wall or Grid.CellType.DoorClosed || 
        Occupants.Count(x => GD.InstanceFromId(x).Get("BlocksCell").Equals(true)) > 0;
    
    public Cell(long id, CellType? cellType = null, IEnumerable<ulong>? occupants = null, IEnumerable<CellCondition>? conditions = null)
    {
        Id = id;
        if (occupants != null) Occupants = occupants.ToHashSet();
        if (conditions != null) Conditions = conditions.ToHashSet();
        CellType = cellType;
    }

    public static Cell FromPosition(Vector3i position)
    {
        return new Cell(IdCalculator.IdFromVec3(position));
    }

    public Cell SetCellType(CellType? type)
    {
        CellType = type;
        return this;
    }
    
    public Cell AddOccupant(ulong uid)
    {
        Occupants.Add(uid);
        return this;
    }

    public Cell RemoveOccupant(ulong uid)
    {
        Occupants.Remove(uid);
        return this;
    }

    public Cell RemoveOccupants()
    {
        Occupants.Clear();
        return this;
    }

    public Cell AddCondition(CellCondition condition)
    {
        Conditions.Add(condition);
        return this;
    }

    public Cell RemoveCondition(CellCondition condition)
    {
        Conditions.Remove(condition);
        return this;
    }

    public Cell RemoveConditions()
    {
        Conditions.Clear();
        return this;
    }
}