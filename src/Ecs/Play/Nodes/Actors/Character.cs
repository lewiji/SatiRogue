using Godot;
namespace SatiRogue.Ecs.Play.Nodes.Actors;

public class Character : Spatial {
   public bool Alive = true;
   public AnimatedSprite3D? AnimatedSprite3D;
   [Export] public bool BlocksCell = true;
   [Export] public bool Enabled = true;
   [Export] public int Health = 10;
   public Particles? Particles;
   [Export] public int SightRange = 10;
   [Export] public float Speed = 1;
   [Export] public int Strength = 1;
   public bool Behaving { get => Alive && Enabled; }

   public override void _Ready() {
      AnimatedSprite3D = GetNode("Visual") as AnimatedSprite3D;
      Particles = GetNode("Particles") as Particles;

      AnimatedSprite3D?.Connect("animation_finished", this, nameof(OnAnimationFinished));
   }

   private void OnAnimationFinished() {
      if (Alive) AnimatedSprite3D?.Play("idle");
   }

   public async void OnDeathAnimation() {
      if (Particles != null) {
         Particles.Visible = true;
         Particles.Emitting = true;
      }
      var tween = GetTree().CreateTween();

      if (AnimatedSprite3D != null) {
         var mat = (SpatialMaterial) AnimatedSprite3D.MaterialOverride;
         tween.TweenProperty(mat, "emission", Colors.Black, 0.25f);
         tween.TweenProperty(mat, "emission", Colors.Red, 0.048f);
         tween.TweenProperty(mat, "emission", Colors.Black, 0.048f).SetDelay(0.032f);
         tween.TweenProperty(mat, "emission", Colors.Red, 0.048f);
         tween.TweenProperty(mat, "emission", Colors.Black, 0.048f).SetDelay(0.032f);
         tween.TweenProperty(mat, "emission", Colors.Red, 0.048f);
         tween.TweenProperty(mat, "emission", Colors.Black, 0.048f).SetDelay(0.032f);
      }
   }
}