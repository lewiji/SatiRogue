using RelEcs;
using SatiRogue.Components.Behaviours;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Systems;

namespace SatiRogue.Ecs.Play.Nodes.Actors; 

public class Enemy : Character, ISpawnable {
   public void Spawn(EntityBuilder entityBuilder) {
      Health = 1;
      entityBuilder
         .Add(this)
         .Add(this as Character)
         .Add(new HealthComponent(Health))
         .Add(new GridPositionComponent())
         .Add(new InputDirectionComponent())
         .Add(new BehaviourTree())
         .Add<Walkable>();
   }
}