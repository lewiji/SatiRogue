using Godot;
using GodotOnReady.Attributes;
using RelEcs;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
namespace SatiRogue.Ecs.Play.Nodes.Actors;

public partial class Player : Character, ISpawnable {
   [OnReadyGet("AnimationPlayer")] public AnimationPlayer AnimationPlayer = null!;

   public void Spawn(EntityBuilder entityBuilder) {
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