using SatiRogue.Ecs.MapGenerator.Components;
using RelEcs;
using World = RelEcs.World;
namespace SatiRogue.Ecs.MapGenerator.Systems;

public class InitMapGen : ISystem {
   public World World { get; set; } = null!;

   public void Run() {
      World.AddElement(new MapGenData());
   }
}