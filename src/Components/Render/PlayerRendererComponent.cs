using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Entities;
using SatiRogue.scenes;

namespace SatiRogue.Components.Render; 

public partial class PlayerRendererComponent : AnimatedSprite3DRendererComponent {
   private readonly Spatial _playerScene = GD.Load<PackedScene>("res://src/Player/Player3d.tscn").Instance<Spatial>();
   public PlayerEntity? PlayerEntity => GridEntity as PlayerEntity;
   private Godot.Camera? _camera;
   private readonly Vector3 _cameraOffset = new(0, 9, 7);
   private readonly Vector3 _spriteOffset = new(0, 1f, 0);

   private float SpriteSmoothing { get; set; } = 0.28f;
   private float CameraSmoothing { get; set; } = 0.07f;
   
   protected override void CreateRootNode() {
      RootNode = _playerScene;
   }

   protected override void CreateVisualNodes() {
      if (GridEntity == null) return;
      if (!EntityResourceLocator.SceneNodePaths.TryGetValue(nameof(ThreeDee), out var threeDeePath)) return;
      var threeDeeNode = GetNode<ThreeDee>(threeDeePath);
      threeDeeNode.AddChild(RootNode);
      _playerScene.Owner = threeDeeNode;
      
      CallDeferred(nameof(SetInitialPosition));

      EntityRegistry.Player?.Connect(nameof(PlayerEntity.SignalAnimation), this, nameof(PlayAnimation));
   }
   
   protected override void HandleVisibilityChanged()
   {
   }

   private void SetInitialPosition()
   {
      RootNode.Translation = GridEntity.GridPosition.ToVector3();
      AnimatedSprite = _playerScene.GetNode<AnimatedSprite3D>("Visual");
      _camera = AnimatedSprite.GetNode<Godot.Camera>("Camera");
      _camera.SetAsToplevel(true);
      if (PlayerEntity != null) _camera.Translation = PlayerEntity.GridPosition.ToVector3() + _cameraOffset;
   }
   
   public override void _Process(float delta) {
      if (_camera == null) return;
      if (TargetTranslation != null)
         _camera.Translation = _camera.Translation.LinearInterpolate(TargetTranslation.Value + _cameraOffset, CameraSmoothing);
   }

   public override void _PhysicsProcess(float delta) {
      base._PhysicsProcess(delta);
      if (TargetTranslation == null || RootNode == null || _camera == null) return;
      if (Teleporting) {
         _camera.Translation = TargetTranslation.Value + _cameraOffset;
      }
   }

}