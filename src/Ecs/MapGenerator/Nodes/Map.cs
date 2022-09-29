using Godot;
using RelEcs;
using World = RelEcs.World;
namespace SatiRogue.Ecs.MapGenerator.Nodes;

public class Map : Spatial, ISpawnable {
   public override void _EnterTree() {
      Name = "Map";
   }

   public void Spawn(EntityBuilder entityBuilder) {
      entityBuilder.Add(this as Spatial);
   }
}