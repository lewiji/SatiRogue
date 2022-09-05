using Godot;
using GodotOnReady.Attributes;
namespace SatiRogue.Ecs.Play.Nodes.Hud;

public partial class DeathScreen : Control {
   [OnReadyGet("AnimationPlayer")] AnimationPlayer _animationPlayer = null!;

   public void FadeToDeath() {
      _animationPlayer.Play("fade_to_death");
   }

   public void FadeFromDeath() {
      _animationPlayer.Play("fade_from_death");
   }

   public void FadeToBlack() {
      _animationPlayer.Play("fade_to_black");
   }

   public void FadeFromBlack() {
      _animationPlayer.Play("fade_from_black");
   }
}