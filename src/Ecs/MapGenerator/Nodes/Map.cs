using Godot;
using RelEcs;

namespace SatiRogue.Ecs.MapGenerator.Nodes; 

public class Map : Spatial, ISpawnable {
   public override void _EnterTree() {
      Name = "Map";
   }
   public void Spawn(EntityBuilder entityBuilder) {
      entityBuilder
         .Add(this as Spatial);
   }
}