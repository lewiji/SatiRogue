using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using RelEcs;
using SatiRogue.Ecs.Core.Nodes;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Nodes.Actors;

public partial class Player : Character {
   [OnReadyGet("AnimationPlayer")]
   public AnimationPlayer AnimationPlayer = null!;

   [OnReadyGet("DiagonalLockIndicator")]
   public Spatial DiagonalLockIndicator = null!;

   [OnReadyGet("DirectionIndicator")]
   public DirectionIndicator DirectionIndicator = null!;

   public override void OnSpawn(EntityBuilder entityBuilder) {
      base.OnSpawn(entityBuilder);
      var playerStore = entityBuilder.World.GetElement<PersistentPlayerData>();

      entityBuilder
         //.Add(this as Player)
         .Add(playerStore.Stats)
         .Add(new HealthComponent(playerStore.Stats.Health, playerStore.Health))
         .Add(new GridPositionComponent())
         .Add(new InputDirectionComponent())
         .Add(new Walkable())
         .Add<Controllable>()
         .Add<Alive>();
   }
}