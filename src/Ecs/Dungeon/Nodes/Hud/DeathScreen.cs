using Godot;
namespace SatiRogue.Ecs.Dungeon.Nodes.Hud;

public partial class DeathScreen : Control {
   [Signal] public delegate void ContinueEventHandler();
   [Signal] public delegate void ExitEventHandler();

   AnimationPlayer _animationPlayer = null!;
   Button _continueButton = null!;
   Button _exitButton = null!;

   public override void _Ready()
   {
	   _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	   _continueButton = GetNode<Button>("%Continue");
	   _exitButton = GetNode<Button>("%Exit");
	   ConnectButtons();
   }

   void ConnectButtons() {
      _continueButton.Connect("pressed",new Callable(this,nameof(OnContinuePressed)));
      _exitButton.Connect("pressed",new Callable(this,nameof(OnExitPressed)));
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