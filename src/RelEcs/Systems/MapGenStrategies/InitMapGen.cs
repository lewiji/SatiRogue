using RelEcs;
using SatiRogue.RelEcs.Components.MapGen;

namespace SatiRogue.RelEcs.Systems.MapGenStratgies; 

public class InitMapGen : GDSystem {
   public override void Run() {
      World.AddElement(new MapGenData());
   }
}