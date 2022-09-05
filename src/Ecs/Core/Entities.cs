using Godot;
namespace SatiRogue.Ecs.Core;

public class Entities : Spatial {
   public override void _EnterTree() {
      Name = "Entities";
   }
}