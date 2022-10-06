using Godot;
namespace SatiRogue.Ecs.Dungeon.Nodes;

public class MapGeometry : Spatial {
   public override void _EnterTree() {
      Name = "MapGeometry";
   }
}