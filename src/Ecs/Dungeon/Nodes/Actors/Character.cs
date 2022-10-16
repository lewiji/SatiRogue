using Godot;
using GodotOnReady.Attributes;
using RelEcs;
using SatiRogue.Ecs.MapGenerator.Components;

namespace SatiRogue.Ecs.Dungeon.Nodes.Actors;

public partial class Character : GameObject {
   bool _alive = true;
   public Particles? Particles;
   public AnimatedSprite3D? AnimatedSprite3D;
   public string CharacterName = "";
   public bool Behaving {
      get => Alive && Enabled;
   }
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

   [OnReady]
   public virtual void OnReady() {
      Visible = false;
      Particles = GetNode("Particles") as Particles;
      AnimatedSprite3D = GetNode("Visual") as AnimatedSprite3D;
      // TODO try this again
      //WallPeekSprite = GetNode("VisualWallPeek") as AnimatedSprite3D;
      AnimatedSprite3D?.Connect("animation_finished", this, nameof(OnAnimationFinished));
   }

   public override void OnSpawn(EntityBuilder entityBuilder) {
      entityBuilder.Add(this as Character);
   }

   void OnAnimationFinished() {
      if (Enabled)
         AnimatedSprite3D?.Play("idle");
   }

   public void OnDeathAnimation() {
      if (Particles != null) {
         Particles.Visible = true;
         Particles.Emitting = true;
      }
   }

   public void CheckVisibility(bool visible) {
      Visible = visible;
   }
}