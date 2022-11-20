using SatiRogue.Ecs.MapGenerator.Components;
using RelEcs;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.MapGenerator.Systems;

public class InitMapGen : ISystem {
   

   public void Run(World world) {
      world.AddOrReplaceElement(new MapGenData());
   }
}