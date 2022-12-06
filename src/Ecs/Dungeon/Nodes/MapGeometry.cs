using Godot;
namespace SatiRogue.Ecs.Dungeon.Nodes;

public partial class MapGeometry : Node3D {
   public override void _EnterTree() {
      Name = "MapGeometry";
   }
}