using Godot;
using GodotOnReady.Attributes;
using RelEcs;
using SatiRogue.Ecs.Play.Components;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Play.Nodes.Items;

public partial class Health : Item {
   Entity? _entity;
   bool _taken;
   World? _world;
   [OnReadyGet("Visual")] public AnimatedSprite3D? AnimatedSprite3D;
   [OnReadyGet("AnimationPlayer")] public AnimationPlayer? AnimationPlayer;
   [OnReadyGet("Particles")] public Particles? Particles;

   public bool Taken {
      get => _taken;
      set {
         if (_taken) return;

         _taken = value;

         if (!_taken) return;
         AnimationPlayer?.Connect("animation_finished", this, nameof(OnAnimationFinished));
         AnimationPlayer?.Play("taken");
         BlocksCell = false;
      }
   }

   void OnAnimationFinished(string name) {
      _world?.Despawn(_entity!.Identity);
      QueueFree();
   }

   public override void Spawn(EntityBuilder entityBuilder) {
      var eb = entityBuilder.Add(this).Add(this as Item).Add(new GridPositionComponent());
      _world = eb.World;
      _entity = eb.Id();
   }
}