using Godot;
using Godot.Collections;
using GodotOnReady.Attributes;

namespace SatiRogue.Entities;

public partial class Enemy3D : Spatial {
   private readonly EnemyEntity? _enemyEntity;
   public Enemy3D() { }

   public Enemy3D(EnemyEntity enemyEntity) {
      _enemyEntity = enemyEntity;

      var sprite = new AnimatedSprite3D {
         Frames = EntityResourceLocator.GetResource<SpriteFrames>(_enemyEntity.EntityType),
         MaterialOverride = EntityResourceLocator.GetResource<Material>(_enemyEntity.EntityType),
         Playing = true,
         Animation = "idle",
         Centered = true,
         PixelSize = 0.05f,
         RotationDegrees = new Vector3(-33f, 0, 0)
      };
      if (sprite.Frames != null) {
         var firstFrameTexture = (Texture) ((Array) ((Dictionary) sprite.Frames.Animations[0])["frames"])[0];
         sprite.Translation = new Vector3(0, firstFrameTexture.GetHeight() / 2f, 0) * sprite.PixelSize;
      }

      AddChild(sprite);
   }

   [OnReady]
   private void ConnectPositionChanged() {
      _enemyEntity?.Connect(nameof(EnemyEntity.EnemyPositionChanged), this, nameof(HandlePositionChanged));
   }

   private void HandlePositionChanged() {
      if (_enemyEntity == null) return;
      Translation = _enemyEntity.GridPosition.ToVector3();
      Visible = _enemyEntity.Visible;
   }
}