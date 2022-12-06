using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using World = RelEcs.World;

namespace SatiRogue.Ecs.MapGenerator.Systems;

public partial class ResetMapGen : ISystem {
   

   public void Run(World world) {
      GD.Print("Resetting mapgen");
      var mapgen = world.GetElement<MapGenData>();
      mapgen.Reset();
      world.RemoveElement<PathfindingHelper>();
   }
}