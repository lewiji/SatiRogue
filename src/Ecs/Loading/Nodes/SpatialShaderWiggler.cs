using Godot;
using GodotOnReady.Attributes;

namespace SatiRogue.Ecs.Loading.Nodes;

public partial class SpatialShaderWiggler : Spatial {
   [OnReadyGet("AnimationPlayer")] AnimationPlayer _animationPlayer = null!;

   [OnReady] void StartAnimationAtRandomTime() {
      var speed = (float) GD.RandRange(0.618f, 1.618f);
      if (GD.Randi() % 2 == 0) speed *= -1;
      _animationPlayer.Play("wiggle", -1f, speed);
   }
}