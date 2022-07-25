using Godot;
using RelEcs;
using SatiRogue.RelEcs.Components.MapGen;

namespace SatiRogue.RelEcs.Nodes.MapGen; 

public class Map : Spatial, ISpawnable {
   public override void _EnterTree() {
      Name = "Map";
   }
   public void Spawn(EntityBuilder entityBuilder) {
      entityBuilder
         .Add(this as Spatial);
   }
}