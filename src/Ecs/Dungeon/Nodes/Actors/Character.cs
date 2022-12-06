using Godot;
using RelEcs;
using SatiRogue.Ecs.MapGenerator.Components;

namespace SatiRogue.Ecs.Dungeon.Nodes.Actors;

public partial class Character : GameObject {
   bool _alive = true;
   public GPUParticles3D? GPUParticles3D;
   public AnimatedSprite3D? AnimatedSprite3D;
   public string CharacterName = "";
   public VisibleOnScreenNotifier3D VisibleOnScreenNotifier3D = default!;
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

   public override void _Ready()
   {
	   VisibleOnScreenNotifier3D = GetNode<VisibleOnScreenNotifier3D>("VisibleOnScreenNotifier3D");
			Visible = false;
      GPUParticles3D = GetNode("Particles") as GPUParticles3D;
      AnimatedSprite3D = GetNode("Visual") as AnimatedSprite3D;
      // TODO try this again
      //WallPeekSprite = GetNode("VisualWallPeek") as AnimatedSprite3D;
      AnimatedSprite3D?.Connect("animation_finished",new Callable(this,nameof(OnAnimationFinished)));
   }

   public override void OnSpawn(EntityBuilder entityBuilder) {
      entityBuilder.Add(this as Character);
   }

   void OnAnimationFinished() {
      if (Enabled)
         AnimatedSprite3D?.Play("idle");
   }

   public void OnDeathAnimation() {
      if (GPUParticles3D != null) {
         GPUParticles3D.Visible = true;
         GPUParticles3D.Emitting = true;
      }
   }

   public void CheckVisibility(bool visible) {
      Visible = visible;
   }
}