using Godot;
using RelEcs;
using SatiRogue.Ecs.Core;
using World = RelEcs.World;

namespace SatiRogue.Ecs.MapGenerator.Systems;

public partial class PassToDungeonState : RefCounted, ISystem {
   

   public void Run(World world) {
      world.GetElement<SessionState>().ChangeToDungeonState(world.GetElement<GameStateController>());
      world.GetElement<MapGenState>().EmitSignal(nameof(MapGenState.FinishedGenerating));
   }
}