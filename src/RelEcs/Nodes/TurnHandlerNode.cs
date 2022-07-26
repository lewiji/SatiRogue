using Godot;
using RelEcs;

namespace SatiRogue.RelEcs.Nodes; 

public class TurnHandlerNode : Node {
   public void Spawn(EntityBuilder entityBuilder) {
      entityBuilder
         .Add(this)
         .Add(new Turn());
   }
}