using RelEcs;

namespace SatiRogue.Ecs.MapGenerator.Systems;

public class PassToPlayState : GdSystem {
   public override async void Run() {
      await ToSignal(GetElement<MapGenState>().GetTree().CreateTimer(0.618f), "timeout");
      GetElement<Main>().ChangeToPlayState();
      GetElement<MapGenState>().EmitSignal(nameof(MapGenState.FinishedGenerating));
   }
}