using System;
using Godot;
using RoguelikeMono.Grid;
using RoguelikeMono.Grid.Entities;

namespace RoguelikeMono.Player;

public class PlayerNode2D : Node2D {
    private Vector2? _targetPosition;

    private bool _teleporting;
    public bool Teleporting {
        get => _teleporting;
        set {
            _teleporting = value;
            if (value) {
                if (_camera != null) {
                    GD.Print("Disable cam smoothing");
                    _camera.SmoothingEnabled = false;
                }
            }
            else {
                if (_camera != null) {
                    CallDeferred(nameof(EnableCameraSmoothingAfterTeleport));
                }
            }
        }
    }

    [Export] private NodePath? _cameraPath { get; set; }
    private Camera2D? _camera;

    public override void _Ready()
    {
        if (EntityRegistry.Player == null)
            throw new Exception("Trying to connect to PlayerPositionChanged signal, but Player is null in EntityRegistry");
        EntityRegistry.Player.Connect(nameof(PlayerData.PlayerPositionChanged), this, nameof(OnGridPositionChanged));
        
        _camera = GetNode<Camera2D>(_cameraPath);
        _camera.SmoothingEnabled = false;
        Teleporting = true;
    }

    private void OnGridPositionChanged()
    {
        var tileWorldPosition = (EntityRegistry.Player!.GridPosition * TileMapGridRepresentation.TileSize);
        if (tileWorldPosition != null) {
            _targetPosition = new Vector2(tileWorldPosition.Value.x, tileWorldPosition.Value.z);
        }
    }

    private async void EnableCameraSmoothingAfterTeleport() {
        GD.Print("Enable cam smoothing");
        await ToSignal(GetTree().CreateTimer(0.08f), "timeout");
        if (_camera != null) _camera.SmoothingEnabled = true;
    }

    public override void _Process(float delta) {
        if (_targetPosition != null && !Position.IsEqualApprox(_targetPosition.Value)) {
            if (Teleporting) {
                Position = _targetPosition.Value;
                Teleporting = false;
            }
            else {
                Position = Position.LinearInterpolate(_targetPosition.Value, delta / 0.0625f);
            }
        }
    }
}