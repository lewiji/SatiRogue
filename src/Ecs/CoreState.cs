using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Systems;
using SatiRogue.Tools;

namespace SatiRogue.Ecs;

public class CoreState : GameState {
   public override void Init(GameStateController gameStates) {
      gameStates.World.AddOrReplaceElement(this);
      InitSystems.Add(new InitFade()).Add(new InitGdSerializer());
   }
}