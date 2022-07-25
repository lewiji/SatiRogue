using Godot;

namespace SatiRogue.RelEcs.Nodes; 

public class Entities : Spatial {
   public override void _EnterTree() {
      Name = "Entities";
   }
}