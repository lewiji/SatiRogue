using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.lib.RelEcsGodot.src;
using World = SatiRogue.lib.RelEcsGodot.src.World;
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

   void OnAnimationFinished(string _) {
      _world?.Despawn(_entity!.Identity);
      QueueFree();
   }

   public override void OnSpawn(EntityBuilder entityBuilder) {
      base.OnSpawn(entityBuilder);
      var eb = entityBuilder.Add(this).Add(new GridPositionComponent());
      _world = eb.World;
      _entity = eb.Id();
   }
}