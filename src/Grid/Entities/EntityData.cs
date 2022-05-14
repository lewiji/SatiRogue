using System;
using Godot;
using SatiRogue.Math;

namespace SatiRogue.Grid.Entities;

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
    public string Uuid { get; } = Guid.NewGuid().ToString();

    public EntityData(Vector3i? gridPosition = null, bool? blocksCell = null) {
        GridPosition = gridPosition.GetValueOrDefault();
        BlocksCell = blocksCell.GetValueOrDefault();
    }

    public bool Move(MovementDirection dir) {
        InputDirection = EntityUtils.MovementDirectionToVector(dir);
        var targetPosition = GridPosition + InputDirection;
        var currentCell = GridGenerator._mapData.GetCellAt(GridPosition);
        var targetCell = GridGenerator._mapData.GetCellAt(targetPosition);
        if (!targetCell.Blocked) {
            currentCell.Occupants.Remove(GetInstanceId());
            targetCell.Occupants.Add(GetInstanceId());
            GridPosition = targetPosition;
            return true;
        }

        return false;
    }
}