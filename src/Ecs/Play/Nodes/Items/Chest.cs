using Godot;
using GodotOnReady.Attributes;
namespace SatiRogue.Ecs.Play.Nodes.Items;

[Tool]
public partial class Chest : Spatial {
   private bool _open;
   [OnReadyGet("Visual")] public AnimatedSprite3D? AnimatedSprite3D;
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
            }
         }
         _open = value;
      }
   }

   [OnReady] private void ConnectAnimationFinished() {
      AnimatedSprite3D?.Connect("animation_finished", this, nameof(OnAniFinished));
   }

   private void OnAniFinished() {
      if (AnimatedSprite3D?.Animation == "closing")
         AnimatedSprite3D.Animation = "closed";
      else if (AnimatedSprite3D?.Animation == "opening")
         AnimatedSprite3D.Animation = "open";
   }
}