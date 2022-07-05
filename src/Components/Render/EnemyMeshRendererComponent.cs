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
   public static PackedScene EnemyParticlesScene = GD.Load<PackedScene>("res://scenes/ThreeDee/Enemy/EnemyParticles.tscn");
   private Particles? _particles;
   
   public float YOffset;
   
   protected override async void OnDead() {
      base.OnDead();
      if (_particles != null) _particles.Emitting = true;
   }

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

      var particles = EnemyParticlesScene.Instance<Particles>();
      AnimatedSprite.AddChild(particles);
      _particles = particles;

      var label = new Label3D();
      label.Text = EnemyEntity.Name;
      label.Translation = Vector3.Up * 3f;
      label.PixelSize = 0.025f;
      label.Billboard = SpatialMaterial.BillboardMode.Enabled;
      label.CastShadow = GeometryInstance.ShadowCastingSetting.Off;
      label.Visible = false;
      RootNode?.AddChild(label);
      
      base.CreateVisualNodes();
   }
}