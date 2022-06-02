using Godot;

namespace SatiRogue.Components.Render; 

public class AnimatedSprite3DRendererComponent : SpatialRendererComponent {
   protected AnimatedSprite3D? AnimatedSprite;

   public async void PlayAnimation(string name) {
      if (AnimatedSprite != null && AnimatedSprite.Frames.HasAnimation(name)) {
         AnimatedSprite.Play(name);
         await ToSignal(AnimatedSprite, "animation_finished");
         await ToSignal(GetTree().CreateTimer(1f / AnimatedSprite.Frames.GetAnimationSpeed(name)), "timeout");
         if (AnimatedSprite.Frames.HasAnimation("idle") && AnimatedSprite.Animation != "die")
            AnimatedSprite.Play("idle");
      }
   }
}