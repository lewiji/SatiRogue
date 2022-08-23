using System.Threading.Tasks;
using Godot;
using GodotOnReady.Attributes;

namespace SatiRogue.Ecs.Core.Nodes;

public partial class Fade : CanvasLayer {
   [OnReadyGet("AnimationPlayer")] private AnimationPlayer _animationPlayer = null!;

   public async Task FadeToBlack() {
      _animationPlayer.Play("fade_to_black");
      await ToSignal(_animationPlayer, "animation_finished");
   }

   public async Task FadeFromBlack() {
      _animationPlayer.Play("fade_from_black");
      await ToSignal(_animationPlayer, "animation_finished");
   }
}