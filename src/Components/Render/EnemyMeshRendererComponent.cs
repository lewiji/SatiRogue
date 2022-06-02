using System;
using Godot;
using Godot.Collections;
using GodotOnReady.Attributes;
using SatiRogue.Entities;
using SatiRogue.scenes;
using SatiRogue.scenes.Hud;
using Array = Godot.Collections.Array;

namespace SatiRogue.Components.Render;
public partial class EnemyMeshRendererComponent : AnimatedSprite3DRendererComponent {
   public EnemyEntity? EnemyEntity => GridEntity as EnemyEntity;
   
   public float YOffset;

   protected override void CreateVisualNodes() {
      if (EnemyEntity == null) throw new Exception("No parent entity found for EnemyMeshRenderer");
      
      AnimatedSprite = new AnimatedSprite3D {
         Frames = EntityResourceLocator.GetResource<SpriteFrames>(EnemyEntity.EntityType),
         MaterialOverride = EntityResourceLocator.GetResource<Material>(EnemyEntity.EntityType),
         Playing = true,
         Animation = "idle",
         Centered = true,
         PixelSize = 0.05f,
         RotationDegrees = new Vector3(-20f, 0, 0)
      };
      if (AnimatedSprite.Frames != null) {
         var firstFrameTexture = (Texture) ((Array) ((Dictionary) AnimatedSprite.Frames.Animations[0])["frames"])[0];
         AnimatedSprite.Translation = new Vector3(0, firstFrameTexture.GetHeight() / 2f, 0) * AnimatedSprite.PixelSize;
         YOffset = AnimatedSprite.Translation.y;
      }
      RootNode?.AddChild(AnimatedSprite);
   }
   
   [OnReady]
   private void ConnectSignals() {
      GridEntity?.Connect(nameof(Entity.Died), this, nameof(OnDead));
   }

   private void OnDead() {
      PlayAnimation("die");
   }
}