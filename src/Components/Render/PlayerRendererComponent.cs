using System;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Entities;
using SatiRogue.MathUtils;
using SatiRogue.scenes;

namespace SatiRogue.Components.Render; 

public partial class PlayerRendererComponent : AnimatedSprite3DRendererComponent {
   private readonly Spatial _playerScene = GD.Load<PackedScene>("res://src/Player/Player3d.tscn").Instance<Spatial>();
   public PlayerEntity? PlayerEntity => GridEntity as PlayerEntity;
   private MovementComponent? _playerMovementComponent;
   private Godot.Camera? _camera;
   private readonly Vector3 _cameraOffset = new(0, 12f, 12f);
   private readonly Vector3 _spriteOffset = new(0, 1f, 0);
   private Vector3 _lastInputDir;
   private Vector3i _lastInputDirVector3I;
   private bool _dirty = false;
   private Vector3? _targetTranslation;

   private float SpriteSmoothing { get; set; } = 0.28f;
   private float CameraSmoothing { get; set; } = 13f;
   
   protected override void CreateRootNode() {
      RootNode = _playerScene;
   }

   protected override void CreateVisualNodes() {
      if (GridEntity == null) return;
      if (!EntityResourceLocator.SceneNodePaths.TryGetValue(nameof(ThreeDee), out var threeDeePath)) return;
      var threeDeeNode = GetNode<ThreeDee>(threeDeePath);
      threeDeeNode.AddChild(RootNode);
      _playerScene.Owner = threeDeeNode;
      _dirty = true;
      
      CallDeferred(nameof(SetInitialPosition));

      EntityRegistry.Player?.Connect(nameof(PlayerEntity.SignalAnimation), this, nameof(PlayAnimation));
   }
   
   protected override void HandleVisibilityChanged()
   {
   }

   private void SetInitialPosition()
   {
      AnimatedSprite = _playerScene.GetNode<AnimatedSprite3D>("Visual");
      _camera = AnimatedSprite.GetNode<Godot.Camera>("Camera");
      _camera.SetAsToplevel(true);
      if (PlayerEntity != null) {
         var playerpos = PlayerEntity.GridPosition.ToVector3();
         _camera.Translation = new Vector3(playerpos.x + _cameraOffset.x, _cameraOffset.y, playerpos.z + _cameraOffset.z);
         _playerMovementComponent = PlayerEntity.GetComponent<PlayerMovementComponent>();
      }
   }
   
   protected override void HandlePositionChanged() {
      base.HandlePositionChanged();
      _targetTranslation = TargetTranslation;
   }

   public override void _Process(float delta) {
      if (_targetTranslation == null || RootNode == null || _camera == null) return;
      if (Teleporting) {
         _camera.Translation = _targetTranslation.Value + _cameraOffset;
         return;
      }
      
      if (_playerMovementComponent == null) return;
      
      if (_playerMovementComponent.InputDirection != Vector3i.Zero && _lastInputDirVector3I != _playerMovementComponent.InputDirection) {
         _lastInputDir = _playerMovementComponent.InputDirection.ToVector3();
         _lastInputDirVector3I = _playerMovementComponent.InputDirection;
      }

      var offsetTarget = new Vector3(_targetTranslation.Value.x + _cameraOffset.x, _cameraOffset.y, _targetTranslation.Value.z + _cameraOffset.z);
      var distance = Mathf.Abs(_camera.Translation.DistanceTo(offsetTarget));
      if (distance <= 0.03f) {
         _camera.Translation = offsetTarget;
         _targetTranslation = null;
      } else {
         _camera.Translation = _camera.Translation.LinearInterpolate(offsetTarget, CameraSmoothing * delta);
      }
   }

   public override void _PhysicsProcess(float delta) {
      if (_dirty) {
         _dirty = false;
         if (GridEntity != null) RootNode.Translation = GridEntity.GridPosition.ToVector3();
         ResetPhysicsInterpolation();
      }
      base._PhysicsProcess(delta);
   }

}