using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Nodes.Actors;

public partial class Player : Character {
   [OnReadyGet("AnimationPlayer")] public AnimationPlayer AnimationPlayer = null!;
   [OnReadyGet("DiagonalLockIndicator")] public Spatial DiagonalLockIndicator = null!;
   [OnReadyGet("DirectionIndicator")] public DirectionIndicator DirectionIndicator = null!;

   public override void Spawn(EntityBuilder entityBuilder) {
      var stats = new Stats(10, 10, 1, 1, 0);

      entityBuilder.Add(this)
         .Add(this as Character)
         .Add(stats)
         .Add(new HealthComponent(stats.Health))
         .Add(new GridPositionComponent())
         .Add(new InputDirectionComponent())
         .Add<Controllable>()
         .Add<Walkable>()
         .Add<Alive>();
   }
}