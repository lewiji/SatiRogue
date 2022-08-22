using Godot;
using GodotOnReady.Attributes;
using RelEcs;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
namespace SatiRogue.Ecs.Play.Nodes.Actors;

public partial class Player : Character {
   [OnReadyGet("AnimationPlayer")] public AnimationPlayer AnimationPlayer = null!;
   [OnReadyGet("DiagonalLockIndicator")] public Spatial DiagonalLockIndicator = null!;
   [OnReadyGet("DirectionIndicator")] public DirectionIndicator DirectionIndicator = null!;

   [OnReady] private void SetStats() {
      Health = 10000;
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