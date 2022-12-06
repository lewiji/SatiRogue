using Godot;
namespace SatiRogue.Ecs.Loading.Nodes;

public partial class SpatialShaderWiggler : Node3D {
   AnimationPlayer _animationPlayer = null!;

   public override void _Ready()
   {
	   _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	   StartAnimationAtRandomTime();
   }

   void StartAnimationAtRandomTime() {
      var speed = (float) GD.RandRange(0.618f, 1.618f);
      if (GD.Randi() % 2 == 0) speed *= -1;
      _animationPlayer.Play("wiggle", -1f, speed);
   }
}