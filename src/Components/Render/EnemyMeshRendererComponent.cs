using System;
using Godot;
using Godot.Collections;
using GodotOnReady.Attributes;
using SatiRogue.Entities;
using SatiRogue.scenes;
using SatiRogue.scenes.Hud;
using Array = Godot.Collections.Array;

namespace SatiRogue.Components.Render;

public partial class EnemyMeshRendererComponent : SpatialRendererComponent {
   private AnimatedSprite3D? _animatedSprite3D;
   public EnemyEntity? EnemyEntity => GridEntity as EnemyEntity;

   protected override void CreateVisualNodes() {
      if (EnemyEntity == null) throw new Exception("No parent entity found for EnemyMeshRenderer");
      
      _animatedSprite3D = new AnimatedSprite3D {
         Frames = EntityResourceLocator.GetResource<SpriteFrames>(EnemyEntity.EntityType),
         MaterialOverride = EntityResourceLocator.GetResource<Material>(EnemyEntity.EntityType),
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
      RootNode?.AddChild(_animatedSprite3D);
   }


}