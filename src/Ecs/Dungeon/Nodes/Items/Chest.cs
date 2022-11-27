using Godot;
using GodotOnReady.Attributes;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
namespace SatiRogue.Ecs.Dungeon.Nodes.Items;

public partial class Chest : Item {
   bool _open;
   [OnReadyGet("Visual")] public AnimatedSprite3D? AnimatedSprite3D;
   [OnReadyGet("Particles")] public Particles? Particles;

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