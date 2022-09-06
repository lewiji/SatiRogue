using Godot;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Nodes.Actors;

public class Character : GameObject {
   bool _alive = true;
   Particles? _particles;
   public AnimatedSprite3D? AnimatedSprite3D;
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
   public Cell? CurrentCell { get; set; }

   public override void _Ready() {
      Visible = false;
      _particles = GetNode("Particles") as Particles;
      AnimatedSprite3D = GetNode("Visual") as AnimatedSprite3D;
      // TODO try this again
      //WallPeekSprite = GetNode("VisualWallPeek") as AnimatedSprite3D;
      AnimatedSprite3D?.Connect("animation_finished", this, nameof(OnAnimationFinished));
   }

   public override void OnSpawn(EntityBuilder entityBuilder) {
      entityBuilder.Add(this);
   }

   void OnAnimationFinished() {
      if (Enabled) AnimatedSprite3D?.Play("idle");
   }

   public void OnDeathAnimation() {
      if (_particles != null) {
         _particles.Visible = true;
         _particles.Emitting = true;
      }

      if (AnimatedSprite3D == null) return;
      var colorBlank = new Color(1f, 0f, 0f, 0f);
      var colorRed = new Color(1f, 0f, 0f);
      var mat = AnimatedSprite3D.MaterialOverlay;
      mat.Set("shader_param/texture_albedo", AnimatedSprite3D.MaterialOverride.Get("albedo_texture"));
      mat.Set("shader_param/uv1_offset", AnimatedSprite3D.MaterialOverride.Get("uv1_offset"));

      var deathTween = GetTree().CreateTween();
      deathTween.TweenProperty(mat, "shader_param/albedo", colorBlank, 0.25f);
      deathTween.TweenProperty(mat, "shader_param/albedo", colorRed, 0.048f);
      deathTween.TweenProperty(mat, "shader_param/albedo", colorBlank, 0.048f).SetDelay(0.032f);
      deathTween.TweenProperty(mat, "shader_param/albedo", colorRed, 0.048f);
      deathTween.TweenProperty(mat, "shader_param/albedo", colorBlank, 0.048f).SetDelay(0.032f);
      deathTween.TweenProperty(mat, "shader_param/albedo", colorRed, 0.048f);
      deathTween.TweenProperty(mat, "shader_param/albedo", colorBlank, 0.048f).SetDelay(0.032f);
   }

   public void CheckVisibility(bool visible) {
      Visible = visible;
   }

   public override void _ExitTree() {
      AnimatedSprite3D?.MaterialOverlay.Dispose();
   }
}