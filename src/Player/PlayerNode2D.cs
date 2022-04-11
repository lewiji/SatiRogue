using System;
using Godot;
using RoguelikeMono.Grid;
using RoguelikeMono.Grid.Entities;

namespace RoguelikeMono.Player;

public class PlayerNode2D : Node2D {
    private Vector2? _targetPosition;
    private Node2D? _visualRepresentation;

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
                    EnableCameraSmoothingAfterTeleport();
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
        _visualRepresentation = GetNode<Node2D>("Visual");
        _visualRepresentation.SetAsToplevel(true);
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
        await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
        if (_camera != null) _camera.SmoothingEnabled = true;
    }

    public override void _PhysicsProcess(float delta) {
        if (_targetPosition != null && _visualRepresentation != null) {
            if (Teleporting) {
                Position = _targetPosition.Value;
                _visualRepresentation.Position = Position;
                Teleporting = false;
            } else {
                if (!Position.IsEqualApprox(_targetPosition.Value)) {
                    Position = _targetPosition.Value;
                }
                if (!_visualRepresentation.Position.IsEqualApprox(_targetPosition.Value)) {
                    _visualRepresentation.Position = _visualRepresentation.Position.LinearInterpolate(_targetPosition.Value, delta / 0.0625f);
                }
            }
        }
    }
}