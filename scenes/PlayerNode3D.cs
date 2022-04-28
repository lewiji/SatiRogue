using System;
using Godot;
using RoguelikeMono.Grid;
using RoguelikeMono.Grid.Entities;

namespace RoguelikeMono.scenes;

public class PlayerNode3D : Spatial
{
    private Vector3? _targetPosition;
    private Spatial? _visualRepresentation;
    private Vector3 _spriteOffset = new Vector3(0, 1f, 0);
    private Vector3 _cameraOffset = new Vector3(0, 9, 7);

    [Export] private float SpriteSmoothing { get; set; }
    [Export] private float CameraSmoothing { get; set; }
    [Export] private NodePath? CameraPath { get; set; }
    private Camera? _camera;

    public bool Teleporting { get; set; }

    public override void _Ready()
    {
        if (EntityRegistry.Player == null)
            throw new Exception("Trying to connect to PlayerPositionChanged signal, but Player is null in EntityRegistry");
        EntityRegistry.Player.Connect(nameof(PlayerData.PlayerPositionChanged), this, nameof(OnGridPositionChanged));

        _camera = GetNode<Camera>(CameraPath);
        _camera.SetAsToplevel(true);
        _visualRepresentation = GetNode<Spatial>("Visual");
        _visualRepresentation.SetAsToplevel(true);
        Teleporting = true;
    }

    private void OnGridPositionChanged()
    {
        var tileWorldPosition = EntityRegistry.Player!.GridPosition;
        _targetPosition = new Vector3(tileWorldPosition.x, 0f, tileWorldPosition.z);
    }

    public override void _Process(float delta)
    {
        _camera.Translation = _camera.Translation.LinearInterpolate(_targetPosition.Value + _cameraOffset, CameraSmoothing);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (EntityRegistry.Player == null) return;
        if (_targetPosition == null || _visualRepresentation == null || _camera == null) return;
        
        if (Teleporting) {
            Translation = _targetPosition.Value;
            _camera.Translation = Translation + _cameraOffset;
            _visualRepresentation.Translation = Translation + _spriteOffset;
            Teleporting = false;
        } else {
            if (!Translation.IsEqualApprox(_targetPosition.Value)) {
                Translation = _targetPosition.Value;
            }
            if (!_visualRepresentation.Translation.IsEqualApprox(_targetPosition.Value  + _spriteOffset))
            {
                _visualRepresentation.Translation = _visualRepresentation.Translation.LinearInterpolate(
                    _targetPosition.Value + _spriteOffset, SpriteSmoothing);
            }
        }
    }
}