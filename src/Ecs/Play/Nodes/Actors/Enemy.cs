using Godot;
using RelEcs;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
namespace SatiRogue.Ecs.Play.Nodes.Actors;

public class Enemy : Character, ISpawnable {
   public SpriteFrames? Frames;
   public SpatialMaterial? Material;

   public void Spawn(EntityBuilder entityBuilder) {
      Health = 1;

      entityBuilder.Add(this)
         .Add(this as Character)
         .Add(new HealthComponent(Health))
         .Add(new GridPositionComponent())
         .Add(new InputDirectionComponent())
         .Add(new BehaviourTree())
         .Add<Walkable>()
         .Add<Alive>();
   }

   public override void _Ready() {
      base._Ready();

      if (AnimatedSprite3D == null) return;
      AnimatedSprite3D.Frames = Frames;
      AnimatedSprite3D.MaterialOverride = Material;
   }
}