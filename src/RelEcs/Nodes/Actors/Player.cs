using Godot;
using RelEcs;
using SatiRogue.RelEcs.Components;

namespace SatiRogue.RelEcs.Nodes.Actors; 

public class Player : Character, ISpawnable {
   public void Spawn(EntityBuilder entityBuilder) {
      entityBuilder
         .Add(this as Spatial)
         .Add(new HealthComponent(Health))
         .Add(new GridPositionComponent())
         .Add(new InputDirectionComponent())
         .Add<Controllable>();
   }
}