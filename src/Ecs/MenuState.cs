using RelEcs;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Menu.Systems;
namespace SatiRogue.Ecs; 

public class MenuState : GameState {
   private GameStateController _gsc;
   public override void Init(GameStateController gameStates) {
      _gsc = gameStates;
      gameStates.World.AddElement(this);

      Intro intro;
      InitSystems
         .Add(new InitMenu())
         .Add(new InitOptions())
         .Add(intro = new Intro());

      intro.Connect(nameof(Intro.IntroFinished), this, nameof(OnIntroFinished));
   }

   void OnIntroFinished() {
       _gsc.World.GetElement<Fade>().FadeFromBlack();
   }
}