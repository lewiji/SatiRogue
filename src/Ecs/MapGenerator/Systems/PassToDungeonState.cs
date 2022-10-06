using Godot;
using RelEcs;
using SatiRogue.Ecs.Core;
using World = RelEcs.World;

namespace SatiRogue.Ecs.MapGenerator.Systems;

public class PassToDungeonState : Reference, ISystem {
   public World World { get; set; } = null!;

   public async void Run() {
      World.GetElement<SessionState>().ChangeToDungeonState(World.GetElement<GameStateController>());
      World.GetElement<MapGenState>().EmitSignal(nameof(MapGenState.FinishedGenerating));
   }
}