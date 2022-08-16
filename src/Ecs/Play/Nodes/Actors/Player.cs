using Godot;
using GodotOnReady.Attributes;
using RelEcs;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
namespace SatiRogue.Ecs.Play.Nodes.Actors;

public partial class Player : Character {
   [OnReadyGet("AnimationPlayer")] public AnimationPlayer AnimationPlayer = null!;

   [OnReady] private void SetStats() {
      Health = 100;
   }

   public override void Spawn(EntityBuilder entityBuilder) {
      entityBuilder.Add(this)
         .Add(this as Character)
         .Add(new HealthComponent(Health))
         .Add(new GridPositionComponent())
         .Add(new InputDirectionComponent())
         .Add<Controllable>()
         .Add<Walkable>()
         .Add<Alive>();
   }
}