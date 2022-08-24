using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Systems;
namespace SatiRogue.Ecs;

public class CoreState : GameState {
   public override void Init(GameStateController gameStates) {
      gameStates.World.AddElement(this);
      InitSystems
         .Add(new InitFade())
         .Add(new InitGdSerializer());
   }
}