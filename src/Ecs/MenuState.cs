using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Menu.Systems;
namespace SatiRogue.Ecs; 

public class MenuState : GameState {
   public override void Init(GameStateController gameStates) {
      gameStates.World.AddElement(this);

      InitSystems.Add(new InitMenu())
         .Add(new InitOptions());
   }
}