using Godot;

namespace SatiRogue.Grid.Entities;

public class PlayerData : EntityData {
    [Signal]
    public delegate void PlayerPositionChanged();

    public override void _Ready() {
        BlocksCell = true;
        Connect(nameof(PositionChanged), this, nameof(OnPositionChanged));
    }

    private void OnPositionChanged() {
        EmitSignal(nameof(PlayerPositionChanged));
    }
}