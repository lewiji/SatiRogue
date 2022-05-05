using Godot;

namespace RoguelikeMono.Ecs; 

public abstract class Component : Node {
   public Entity? Entity;
   public bool Ticking = true;

   public virtual void Tick(float delta) {
      
   }
}