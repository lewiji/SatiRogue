using Godot;
namespace SatiRogue.Ecs.Core;

public partial class Entities : Node3D {
   public override void _EnterTree() {
      Name = "Entities";
   }
}