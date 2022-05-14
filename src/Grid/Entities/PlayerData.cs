using Godot;
using Godot.Collections;
using SatiRogue.Debug;
using SatiRogue.Math;

namespace SatiRogue.Grid.Entities;

public class PlayerData : EntityData {
    [Signal] public delegate void PlayerPositionChanged();

    public PlayerData() : base() { }

    public PlayerData(Vector3i? gridPosition = null) : base(gridPosition, true) { }

    public override void _Ready() {
        Connect(nameof(PositionChanged), this, nameof(OnPositionChanged));
        GetNode<GridGenerator>(GridGenerator.Path).Connect(nameof(GridGenerator.MapChanged), this, nameof(OnMapDataChanged));
    }

    private void OnPositionChanged() {
        CalculateVisibility();
        EmitSignal(nameof(PlayerPositionChanged));
    }

    private void OnMapDataChanged() {
        CallDeferred(nameof(CalculateVisibility));
    }

    private void CalculateVisibility() {
        ShadowCast.ComputeVisibility(GridGenerator._mapData, GridPosition, 11.0f);
    }

}