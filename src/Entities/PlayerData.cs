using System;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Grid;
using SatiRogue.Math;

namespace SatiRogue.Entities;

public class PlayerData : EntityData {
    [Signal] public delegate void PlayerPositionChanged();
    
    public PlayerData () { }

    public PlayerData(Vector3i? gridPosition = null) : base(gridPosition, true) {
        Uuid = Guid.Empty;
        Name = "Player";
    }

    public override void _Ready() {
        Logger.Info("Player ready");
        Connect(nameof(PositionChanged), this, nameof(OnPositionChanged));
        GetNode<MapGenerator>(MapGenerator.Path).Connect(nameof(MapGenerator.MapChanged), this, nameof(OnMapDataChanged));
        CallDeferred(nameof(OnPositionChanged));
    }

    private void OnPositionChanged() {
        Logger.Info("Player position changed");
        CalculateVisibility();
        EmitSignal(nameof(PlayerPositionChanged));
    }

    private void OnMapDataChanged() {
        Logger.Info("Player map data changed");
        CallDeferred(nameof(CalculateVisibility));
    }

    private void CalculateVisibility() {
        ShadowCast.ComputeVisibility(MapGenerator._mapData, GridPosition, 11.0f);
    }

}