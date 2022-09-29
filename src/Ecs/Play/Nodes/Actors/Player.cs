using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using RelEcs;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Nodes.Actors;

public partial class Player : Character {
   [OnReadyGet("AnimationPlayer")]
   public AnimationPlayer AnimationPlayer = null!;

   [OnReadyGet("DiagonalLockIndicator")]
   public Spatial DiagonalLockIndicator = null!;

   [OnReadyGet("DirectionIndicator")]
   public DirectionIndicator DirectionIndicator = null!;
   Stats _stats = new(10, 10, 1, 1, 0);

   public override void OnSpawn(EntityBuilder entityBuilder) {
      base.OnSpawn(entityBuilder);

      entityBuilder
         //.Add(this as Player)
         .Add(_stats)
         .Add(new HealthComponent(_stats.Health))
         .Add(new GridPositionComponent())
         .Add(new InputDirectionComponent())
         .Add(new Walkable())
         .Add<Controllable>()
         .Add<Alive>();
   }
}