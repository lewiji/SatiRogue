using Godot;
using RelEcs;
using World = RelEcs.World;

namespace SatiRogue.Ecs.MapGenerator.Systems;

public class PassToPlayState : Reference, ISystem {
   public World World { get; set; } = null!;

   public async void Run() {
      //await ToSignal(this.GetElement<MapGenState>().GetTree().CreateTimer(0.618f), "timeout");
      World.GetElement<Main>().ChangeToPlayState();
      World.GetElement<MapGenState>().EmitSignal(nameof(MapGenState.FinishedGenerating));
   }
}