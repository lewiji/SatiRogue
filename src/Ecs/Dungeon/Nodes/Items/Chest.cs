using Godot;
using GodotOnReady.Attributes;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
namespace SatiRogue.Ecs.Dungeon.Nodes.Items;

public partial class Chest : Item {
   bool _open;
   [OnReadyGet("Visual")] public AnimatedSprite3D? AnimatedSprite3D;
   [OnReadyGet("Particles")] public Particles? Particles;
   public bool Locked { get; set; }

   [Export] public bool Open {
      get => _open;
      set {
         if (AnimatedSprite3D != null) {
            // Closing
            if (!value && _open && AnimatedSprite3D.Animation is not "closed" or "closing") {
               AnimatedSprite3D.Animation = "closing";
            } // Opening
            else if (!_open && value && AnimatedSprite3D.Animation is not "open" or "opening") {
               AnimatedSprite3D.Animation = "opening";
               if (Particles != null) Particles.Emitting = true;
            }
         }
         _open = value;
      }
   }

   public override void OnSpawn(EntityBuilder entityBuilder) {
      entityBuilder.Add(this as Item).Add(new GridPositionComponent()).Add<Closed>();
   }

   [OnReady] void ConnectAnimationFinished() {
      AnimatedSprite3D?.Connect("animation_finished", this, nameof(OnAniFinished));
   }

   void OnAniFinished() {
      if (AnimatedSprite3D?.Animation == "closing")
         AnimatedSprite3D.Animation = "closed";
      else if (AnimatedSprite3D?.Animation == "opening")
         AnimatedSprite3D.Animation = "open";
   }
}