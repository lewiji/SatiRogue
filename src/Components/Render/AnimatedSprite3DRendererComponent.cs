using Godot;
using GodotOnReady.Attributes;
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
         AnimatedSprite.Play(name);
         await ToSignal(AnimatedSprite, "animation_finished");
         await ToSignal(GetTree().CreateTimer(1f / AnimatedSprite.Frames.GetAnimationSpeed(name)), "timeout");
         if (!IsInstanceValid(AnimatedSprite)) return;
         if (AnimatedSprite.Frames.HasAnimation("idle") && AnimatedSprite.Animation != "die")
            AnimatedSprite.Play("idle");
      }
   }
   
   [OnReady]
   private void ConnectSignals() {
      GridEntity?.Connect(nameof(Entity.Died), this, nameof(OnDead));
   }

   private void OnDead() {
      PlayAnimation("die");
   }
}