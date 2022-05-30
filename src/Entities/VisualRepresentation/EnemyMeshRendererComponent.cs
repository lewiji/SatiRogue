using System;
using Godot;
using Godot.Collections;
using GodotOnReady.Attributes;
using SatiRogue.Components;
using SatiRogue.scenes;
using SatiRogue.scenes.Hud;
using Array = Godot.Collections.Array;

namespace SatiRogue.Entities;

public partial class EnemyMeshRendererComponent : Component {
   private Spatial? _rootSpatialNode;
   private AnimatedSprite3D? _animatedSprite3D;
   private StatBar3D? _statBar3D;
   public EnemyEntity? Entity;
   public override GameObject? EcOwner {
      get => Entity;
      set => Entity = value as EnemyEntity;
   }

   [OnReady]
   private void CreateSpatialNodes() {
      if (Entity == null) throw new Exception("No parent entity found for EnemyMeshRenderer");
      _rootSpatialNode = new MeshRendererEntity() {Translation = Entity.GridPosition.ToVector3(), EcOwner = this.EcOwner};
      
      _animatedSprite3D = new AnimatedSprite3D {
         Frames = EntityResourceLocator.GetResource<SpriteFrames>(Entity.EntityType),
         MaterialOverride = EntityResourceLocator.GetResource<Material>(Entity.EntityType),
         Playing = true,
         Animation = "idle",
         Centered = true,
         PixelSize = 0.05f,
         RotationDegrees = new Vector3(-33f, 0, 0)
      };
      if (_animatedSprite3D.Frames != null) {
         var firstFrameTexture = (Texture) ((Array) ((Dictionary) _animatedSprite3D.Frames.Animations[0])["frames"])[0];
         _animatedSprite3D.Translation = new Vector3(0, firstFrameTexture.GetHeight() / 2f, 0) * _animatedSprite3D.PixelSize;
      }
      _rootSpatialNode.AddChild(_animatedSprite3D);

      _statBar3D = (StatBar3D) EntityResourceLocator.StatBar3DScene.Instance();
      _statBar3D.Scale = new Vector3(0.05f,0.05f,0.05f);
      _rootSpatialNode.AddChild(_statBar3D);

      if (EntityResourceLocator.SceneNodePaths.TryGetValue(nameof(ThreeDee), out var threeDeePath)) {
         var threeDeeNode = GetNode<ThreeDee>(threeDeePath);
         threeDeeNode.EnemiesSpatial?.AddChild(_rootSpatialNode);
      }
   }

   [OnReady]
   private void ConnectPositionChanged() {
      Entity?.Connect(nameof(EnemyEntity.EnemyPositionChanged), this, nameof(HandlePositionChanged));
   }

   private void HandlePositionChanged() {
      if (Entity == null || _rootSpatialNode == null) return;
      _rootSpatialNode.Translation = Entity.GridPosition.ToVector3();
      _rootSpatialNode.Visible = Entity.Visible;
   }
}