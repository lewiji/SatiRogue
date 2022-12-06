using Godot;
using SatiRogue.Debug;
namespace SatiRogue.Ecs.Intro.Nodes;

public partial class IntroScene : Control {
   [Signal] public delegate void DebugSkipToNewGameEventHandler();
   AnimationPlayer _animationPlayer = null!;
   bool _skipped;

   public override void _Ready()
   {
	   _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
   }

   public override void _Input(InputEvent @event) {
	  if (!_skipped && @event.IsActionPressed("debug_skip_to_game")) {
		 Logger.Info("Debug: Skipping to new game");
		 _skipped = true;
		 _animationPlayer.Play("RESET");
		 EmitSignal(nameof(DebugSkipToNewGame));
		 return;
	  }

	  if (_skipped || !_animationPlayer.IsPlaying() || !@event.IsActionPressed("ui_cancel"))
		 return;
	  _animationPlayer.Play("RESET");
	  _skipped = true;
   }
}
