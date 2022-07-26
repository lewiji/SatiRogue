using Godot;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Systems;
using RelEcs;

namespace SatiRogue.Ecs.Play.Nodes.Actors; 

public class Player : Character, ISpawnable {
   public void Spawn(EntityBuilder entityBuilder) {
      entityBuilder
         .Add(this)
         .Add(this as Character)
         .Add(new HealthComponent(Health))
         .Add(new GridPositionComponent())
         .Add(new InputDirectionComponent())
         .Add<Controllable>()
         .Add<Walkable>();
   }
}