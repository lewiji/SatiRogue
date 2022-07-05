using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Debug;
using SatiRogue.Entities;
using SatiRogue.MathUtils;

namespace SatiRogue.Components.Render; 

public partial class AnimatedSprite3DRendererComponent : SpatialRendererComponent {
   protected AnimatedSprite3D? AnimatedSprite;
   
   protected override void HandlePositionChanged() {
      base.HandlePositionChanged();
      if (GridEntity == null || RootNode == null || AnimatedSprite ==  null) return;
      if (GridEntity.MovementComponent == null) return;
      
      if (GridEntity.MovementComponent.InputDirection == Vector3i.Left) {
         AnimatedSprite.FlipH = true;
      } else if (GridEntity.MovementComponent.InputDirection == Vector3i.Right) {
         AnimatedSprite.FlipH = false;  
      }
   }

   public async void PlayAnimation(string name) {
      if (AnimatedSprite != null && AnimatedSprite.Frames.HasAnimation(name)) {
         if (AnimatedSprite.Animation == "die") return;
         AnimatedSprite.Play(name);
         await ToSignal(AnimatedSprite, "animation_finished");
         if (!IsInstanceValid(AnimatedSprite)) return;
         if (AnimatedSprite.Animation == "die") return;
         if (AnimatedSprite.Frames.HasAnimation("idle"))
            AnimatedSprite.Play("idle");
      }
   }
   
   [OnReady]
   private void ConnectSignals() {
      GridEntity?.Connect(nameof(Entity.Died), this, nameof(OnDead));
   }

   protected virtual void OnDead() {
      PlayAnimation("die");
   }
}