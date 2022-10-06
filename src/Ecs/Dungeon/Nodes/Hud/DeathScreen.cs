using Godot;
using GodotOnReady.Attributes;
namespace SatiRogue.Ecs.Dungeon.Nodes.Hud;

public partial class DeathScreen : Control {
   [Signal] public delegate void Continue();
   [Signal] public delegate void Exit();

   [OnReadyGet("AnimationPlayer")] AnimationPlayer _animationPlayer = null!;
   [OnReadyGet("%Continue")] Button _continueButton = null!;
   [OnReadyGet("%Exit")] Button _exitButton = null!;

   [OnReady] void ConnectButtons() {
      _continueButton.Connect("pressed", this, nameof(OnContinuePressed));
      _exitButton.Connect("pressed", this, nameof(OnExitPressed));
   }

   void OnContinuePressed() {
      EmitSignal(nameof(Continue));
      FadeFromDeath();
   }

   void OnExitPressed() {
      EmitSignal(nameof(Exit));
      FadeFromDeath();
   }

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