using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Menu.Systems;
namespace SatiRogue.Ecs;

public class MenuState : GameState {
   GameStateController _gsc = null!;

   public override void Init(GameStateController gameStates) {
      _gsc = gameStates;
      gameStates.World.AddElement(this);

      Menu.Systems.Intro introScene;
      InitSystems.Add(new InitMenu()).Add(new InitOptions()).Add(introScene = new Menu.Systems.Intro());

      introScene.Connect(nameof(Menu.Systems.Intro.IntroFinished), this, nameof(OnIntroFinished));
   }

   async void OnIntroFinished() {
      await _gsc.World.GetElement<Fade>().FadeFromBlack();
   }
}