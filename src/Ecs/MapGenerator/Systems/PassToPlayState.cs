using RelEcs;

namespace SatiRogue.Ecs.MapGenerator.Systems;

public class PassToPlayState : GdSystem {
   public override void Run() {
      GetElement<Main>().ChangeToPlayState();
      GetElement<MapGenState>().EmitSignal(nameof(MapGenState.FinishedGenerating));
   }
}