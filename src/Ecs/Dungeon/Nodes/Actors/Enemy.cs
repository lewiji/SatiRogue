using Godot;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using RelEcs;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Nodes.Actors;

public class Enemy : Character {
   public SpriteFrames? Frames;
   public Material? Material;
   public Stats Stats = new(1, 10, 1, 1, 0);

   public override void OnSpawn(EntityBuilder entityBuilder) {
      base.OnSpawn(entityBuilder);

      entityBuilder.Add(Stats)
         .Add(new HealthComponent(Stats.Health))
         .Add(new GridPositionComponent())
         .Add(new InputDirectionComponent())
         .Add(new BehaviourTree())
         .Add(new Walkable())
         .Add<Alive>();
   }

   public override void _Ready() {
      base._Ready();

      if (AnimatedSprite3D == null)
         return;
      AnimatedSprite3D.Frames = Frames;
      AnimatedSprite3D.MaterialOverride = Material;
   }
}