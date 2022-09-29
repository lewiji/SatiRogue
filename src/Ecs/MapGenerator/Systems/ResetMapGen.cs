using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using World = RelEcs.World;

namespace SatiRogue.Ecs.MapGenerator.Systems;

public class ResetMapGen : ISystem {
   public World World { get; set; } = null!;

   public void Run() {
      GD.Print("Resetting mapgen");
      var mapgen = World.GetElement<MapGenData>();
      mapgen.Reset();
      World.RemoveElement<PathfindingHelper>();
   }
}