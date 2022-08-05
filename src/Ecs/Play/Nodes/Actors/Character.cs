using Godot;
using RelEcs;
using SatiRogue.scenes.Hud;

namespace SatiRogue.Ecs.Play.Nodes.Actors; 

public class Character : Spatial {
   [Export] public int Health = 10;
   [Export] public int Strength = 1;
   [Export] public float Speed = 1;
   [Export] public bool BlocksCell = true;
   [Export] public bool Enabled = true;
   [Export] public int SightRange = 10;
   public bool Behaving => Health > 0 && Enabled;
   public StatBar3D? StatBar3D;
   public AnimatedSprite3D? AnimatedSprite3D;

   public override void _Ready() {
      AnimatedSprite3D = GetNode("Visual") as AnimatedSprite3D;
      if (AnimatedSprite3D != null) {
         AnimatedSprite3D.Connect("animation_finished", this, nameof(OnAnimationFinished));
      }
   }

   private void OnAnimationFinished() {
      AnimatedSprite3D?.Play("idle");
   }
}