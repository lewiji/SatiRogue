using Godot;
using RoguelikeMono.Math;

namespace RoguelikeMono.Grid.Entities;

public class PlayerData : Node
{
    [Signal] public delegate void PlayerPositionChanged();
    
    private Vector3i _gridPosition;
    public Vector3i GridPosition
    {
        get => _gridPosition;
        set
        {
            _gridPosition = value;
            EmitSignal(nameof(PlayerPositionChanged));
        }
    }

    public bool Move(MovementDirection dir) {
        var targetPosition = GridPosition + EntityUtils.MovementDirectionToVector(dir);
        var targetCell = GridGenerator._mapData.GetCellAt(targetPosition);
        if (!targetCell.Blocked) {
            GridPosition = targetPosition;
        }
        return true;
    }
    
    public override void _Ready()
    {
        
    }
}