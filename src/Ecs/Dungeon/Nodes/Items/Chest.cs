using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
namespace SatiRogue.Ecs.Dungeon.Nodes.Items;

public partial class Chest : Item {
   bool _open;
   public AnimatedSprite3D? AnimatedSprite3D;
   public GPUParticles3D? GPUParticles3D;

   public override void _Ready()
   {
	   AnimatedSprite3D = GetNode<AnimatedSprite3D>("Visual");
	   GPUParticles3D = GetNode<GPUParticles3D>("GPUParticles3D");
	   ConnectAnimationFinished();
   }

   public override void OnSpawn(EntityBuilder entityBuilder) {
      entityBuilder.Add(this as Item).Add(new GridPositionComponent()).Add<Closed>();
   }

   void ConnectAnimationFinished() {
      AnimatedSprite3D?.Connect("animation_finished",new Callable(this,nameof(OnAniFinished)));
   }

   void OnAniFinished() {
      if (AnimatedSprite3D?.Animation == "closing")
         AnimatedSprite3D.Animation = "closed";
      else if (AnimatedSprite3D?.Animation == "opening")
         AnimatedSprite3D.Animation = "open";
   }
}