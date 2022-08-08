using Godot;
namespace SatiRogue.Ecs.Play.Nodes.Actors;

public class Character : Spatial {
   private bool _alive = true;
   private Particles? _particles;
   public AnimatedSprite3D? AnimatedSprite3D;
   [Export] public bool BlocksCell = true;
   [Export] public bool Enabled = true;
   [Export] public int Health = 10;
   [Export] public int SightRange = 10;
   [Export] public float Speed = 1;
   [Export] public int Strength = 1;
   public AnimatedSprite3D? WallPeekSprite;
   public bool Behaving { get => Alive && Enabled; }
   public bool Alive {
      get => _alive;
      set {
         if (!value) {
            Enabled = false;
            BlocksCell = false;
         } else if (_alive != value) {
            // alive has gone from false to true, rise from your grave
            Enabled = true;
            BlocksCell = true;
         }

         _alive = value;
      }
   }

   public override void _Ready() {
      _particles = GetNode("Particles") as Particles;
      AnimatedSprite3D = GetNode("Visual") as AnimatedSprite3D;
      //WallPeekSprite = GetNode("VisualWallPeek") as AnimatedSprite3D;
      AnimatedSprite3D?.Connect("animation_finished", this, nameof(OnAnimationFinished));
   }

   private void OnAnimationFinished() {
      if (Enabled) AnimatedSprite3D?.Play("idle");
   }

   public void OnDeathAnimation() {
      if (_particles != null) {
         _particles.Visible = true;
         _particles.Emitting = true;
      }
      var tween = GetTree().CreateTween();

      if (AnimatedSprite3D == null) return;
      var colorBlank = new Color(1f, 0f, 0f, 0f);
      var colorRed = new Color(1f, 0f, 0f);
      var mat = AnimatedSprite3D.MaterialOverlay;
      mat.Set("shader_param/texture_albedo", AnimatedSprite3D.MaterialOverride.Get("albedo_texture"));
      mat.Set("shader_param/uv1_offset", AnimatedSprite3D.MaterialOverride.Get("uv1_offset"));
      tween.TweenProperty(mat, "shader_param/albedo", colorBlank, 0.25f);
      tween.TweenProperty(mat, "shader_param/albedo", colorRed, 0.048f);
      tween.TweenProperty(mat, "shader_param/albedo", colorBlank, 0.048f).SetDelay(0.032f);
      tween.TweenProperty(mat, "shader_param/albedo", colorRed, 0.048f);
      tween.TweenProperty(mat, "shader_param/albedo", colorBlank, 0.048f).SetDelay(0.032f);
      tween.TweenProperty(mat, "shader_param/albedo", colorRed, 0.048f);
      tween.TweenProperty(mat, "shader_param/albedo", colorBlank, 0.048f).SetDelay(0.032f);
   }
}