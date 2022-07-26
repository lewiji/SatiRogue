using RelEcs;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Systems;

namespace SatiRogue.Ecs.Play.Nodes.Actors; 

public class Enemy : Character, ISpawnable {
   public void Spawn(EntityBuilder entityBuilder) {
      entityBuilder
         .Add(this)
         .Add(this as Character)
         .Add(new HealthComponent(Health))
         .Add(new GridPositionComponent())
         .Add(new InputDirectionComponent())
         .Add(new InterpolateWalkAnimationSystem())
         .Add<Walkable>();
   }
}