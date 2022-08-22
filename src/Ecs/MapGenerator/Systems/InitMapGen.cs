using SatiRogue.Ecs.MapGenerator.Components;
using RelEcs;

namespace SatiRogue.Ecs.MapGenerator.Systems;

public class InitMapGen : GdSystem {
   public override void Run() {
      World.AddElement(new MapGenData());
   }
}