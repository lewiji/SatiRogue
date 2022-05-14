using System;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Grid;
using SatiRogue.Math;

namespace SatiRogue.Entities;

public class EntityData : Node {
    [Signal]
    public delegate void PositionChanged();

    private Vector3i _gridPosition;
    public Vector3i GridPosition {
        get => _gridPosition;
        set {
            LastPosition = _gridPosition;
            _gridPosition = value;
            EmitSignal(nameof(PositionChanged));
        }
    }
    public Vector3i InputDirection { get; protected set; }
    public Vector3i LastPosition { get; protected set; }
    public bool BlocksCell { get; set; } = false;
    public Guid Uuid { get; protected set; } = Guid.NewGuid();

    public EntityData(Vector3i? gridPosition = null, bool? blocksCell = null) {
        GridPosition = gridPosition.GetValueOrDefault();
        BlocksCell = blocksCell.GetValueOrDefault();
        Name = "Entity";
    }

    public bool Move(MovementDirection dir) {
        InputDirection = EntityUtils.MovementDirectionToVector(dir);
        var targetPosition = GridPosition + InputDirection;
        var currentCell = MapGenerator._mapData.GetCellAt(GridPosition);
        var targetCell = MapGenerator._mapData.GetCellAt(targetPosition);
        if (!targetCell.Blocked) {
            currentCell.Occupants.Remove(GetInstanceId());
            targetCell.Occupants.Add(GetInstanceId());
            GridPosition = targetPosition;
            return true;
        }

        return false;
    }
}