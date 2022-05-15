using System;
using Godot;
using SatiRogue.Entities;
using SatiRogue.Grid;

namespace SatiRogue.Player;

public class PlayerNode2D : Node2D {
   private Camera2D? _camera;
   private Vector2? _targetPosition;

   private bool _teleporting;
   private Node2D? _visualRepresentation;

   public bool Teleporting {
      get => _teleporting;
      set {
         _teleporting = value;
         if (_camera == null) return;
         SetCameraSmoothing(_teleporting);
      }
   }

   [Export] private NodePath? _cameraPath { get; set; }

   public override void _Ready() {
      CallDeferred(nameof(HandleInitialPlayerTurn));
   }

   private void HandleInitialPlayerTurn() {
      if (EntityRegistry.Player == null)
         throw new Exception("Trying to connect to PlayerPositionChanged signal, but Player is null in EntityRegistry");
      EntityRegistry.Player.Connect(nameof(PlayerData.PlayerPositionChanged), this, nameof(OnGridPositionChanged));

      _camera = GetNode<Camera2D>(_cameraPath);
      _camera.SmoothingEnabled = false;
      _visualRepresentation = GetNode<Node2D>("Visual");
      _visualRepresentation.SetAsToplevel(true);
      Teleporting = true;

      CallDeferred(nameof(OnGridPositionChanged));
   }

   private void OnGridPositionChanged() {
      var tileWorldPosition = EntityRegistry.Player!.GridPosition * TileMapGridRepresentation.TileSize;
      if (tileWorldPosition != null) _targetPosition = new Vector2(tileWorldPosition.Value.x, tileWorldPosition.Value.z);
   }

   private async void SetCameraSmoothing(bool enabled) {
      if (!enabled) await ToSignal(GetTree().CreateTimer(0.01f), "timeout");
      if (_camera != null) _camera.SmoothingEnabled = enabled;
   }

   public override void _PhysicsProcess(float delta) {
      if (_targetPosition == null || _visualRepresentation == null) return;
      if (Teleporting) {
         Position = _targetPosition.Value;
         _visualRepresentation.Position = Position;
         Teleporting = false;
      }
      else {
         if (!Position.IsEqualApprox(_targetPosition.Value)) Position = _targetPosition.Value;
         if (!_visualRepresentation.Position.IsEqualApprox(_targetPosition.Value))
            _visualRepresentation.Position = _visualRepresentation.Position.LinearInterpolate(_targetPosition.Value, delta / 0.0625f);
      }
   }
}