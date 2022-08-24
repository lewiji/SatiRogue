using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Loading.Systems;

namespace SatiRogue.Ecs;

public class LoadingState : GameState {
   public override void Init(GameStateController gameStates) {
      gameStates.World.AddElement(this);
      InitSystems.Add(new PreloadResources())
         .Add(new CompileShaders());
   }
}