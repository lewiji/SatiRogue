using Godot;
using Godot.Collections;
using SatiRogue.Debug;
using SatiRogue.Math;

namespace SatiRogue.Grid.Entities;

public class PlayerData : EntityData {
    [Signal] public delegate void PlayerPositionChanged();

    public override void _Ready() {
        BlocksCell = true;
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
        /*Vector3i[] positions = {
            GridPosition,
            GridPosition + Vector3i.Forward,
            GridPosition + Vector3i.Back,
            GridPosition + Vector3i.Left,
            GridPosition + Vector3i.Right,
            GridPosition + Vector3i.Forward + Vector3i.Left,
            GridPosition + Vector3i.Forward + Vector3i.Right,
            GridPosition + Vector3i.Back + Vector3i.Left,
            GridPosition + Vector3i.Back + Vector3i.Right,
        };
        var changedPositions = new Array<Vector3>();
        foreach (var position in positions) {
            GridGenerator._mapData.SetCellVisibility(position, CellVisibility.CurrentlyVisible);
            changedPositions.Add(position.ToVector3());
        }
        
        GetNode<GridGenerator>(GridGenerator.Path).EmitSignal(nameof(GridGenerator.VisibilityChanged), changedPositions);*/
        Logger.Info("Computing Visibility");
        ShadowCast.ComputeVisibility(GridGenerator._mapData, GridPosition, 11.0f);
    }
}