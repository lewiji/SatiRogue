using Godot;
using RelEcs;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.resources;

namespace SatiRogue.Ecs.Dungeon.Nodes.Actors;

public partial class Player : Character {
   public AnimationPlayer AnimationPlayer = null!;
   public Node3D DiagonalLockIndicator = null!;
   public DirectionIndicator DirectionIndicator = null!;

   public override void OnSpawn(EntityBuilder entityBuilder) {
      base.OnSpawn(entityBuilder);

      var playerStore = entityBuilder.World.GetElement<PersistentPlayerData>();
      CharacterName = playerStore.PlayerName;

      entityBuilder
         //.Add(this as Player)
         .Add(playerStore.Stats)
         .Add(new HealthComponent(playerStore.Stats.Record.Health, playerStore.Health))
         .Add(new GridPositionComponent())
         .Add(new InputDirectionComponent())
         .Add(new CharacterAnimationComponent())
         .Add(new Walkable())
         .Add<Controllable>()
         .Add<Alive>();
   }

   public override void _Ready()
   {
	   AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	   DiagonalLockIndicator = GetNode<Node3D>("DiagonalLockIndicator");
	   DirectionIndicator = GetNode<DirectionIndicator>("DirectionIndicator");
	   SetupReflectionProbe();
	   SetChildVisibility();
   }

   async void SetupReflectionProbe()
   {
	   var reflProbe = GetNode<ReflectionProbe>("ReflectionProbe");
	   
      if (SatiConfig.IsMobile) {
         reflProbe.QueueFree();
      } else {
	      await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
	      reflProbe.Translate(new Vector3(0, 0.0001f, 0));
      }
   }

   void SetChildVisibility()
   {
	   DirectionIndicator.Visible = false;
	   DiagonalLockIndicator.Visible = false;
   }
}