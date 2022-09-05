using Godot;
using GodotOnReady.Attributes;
namespace SatiRogue.Ecs.Intro.Nodes;

public partial class IntroScene : Control {
   [OnReadyGet("AnimationPlayer")] AnimationPlayer _animationPlayer = null!;
   bool _skipped;

   public override void _Input(InputEvent @event) {
      if (_skipped || !_animationPlayer.IsPlaying() || !@event.IsActionPressed("ui_cancel"))
         return;
      _animationPlayer.Play("RESET");
      _skipped = true;
   }
}