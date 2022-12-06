using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Nodes.Items;

public partial class Health : Item {
   Entity? _entity;
   bool _taken;
   World? _world;
   
   public AnimatedSprite3D? AnimatedSprite3D;
   public AnimationPlayer? AnimationPlayer;
   public GPUParticles3D? GPUParticles3D;

   public bool Taken {
      get => _taken;
      set {
         if (_taken)
            return;

         _taken = value;

         if (!_taken)
            return;
         AnimationPlayer?.Connect("animation_finished",new Callable(this,nameof(OnAnimationFinished)));
         AnimationPlayer?.Play("taken");
         BlocksCell = false;
      }
   }

   public override void _Ready()
   {
	   AnimatedSprite3D = GetNode<AnimatedSprite3D>("Visual");
	   AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	   GPUParticles3D = GetNode<GPUParticles3D>("GPUParticles3D");
   }

   void OnAnimationFinished(string _) {
      _world?.Despawn(_entity!);
      QueueFree();
   }

   public override void OnSpawn(EntityBuilder entityBuilder) {
      base.OnSpawn(entityBuilder);
      var eb = entityBuilder.Add(new GridPositionComponent());
      _world = eb.World;
      _entity = eb.Id();
   }
}