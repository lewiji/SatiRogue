using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Menu.Systems;
using SatiRogue.Tools;
namespace SatiRogue.Ecs;

public partial class MenuState : GameState {
   GameStateController _gsc = null!;

   public override void Init(GameStateController gameStates) {
      _gsc = gameStates;
      gameStates.World.AddOrReplaceElement(this);

      Menu.Systems.Intro introSystem;
      InitSystems.Add(new InitMenu()).Add(new InitOptions()).Add(introSystem = new Menu.Systems.Intro());

      introSystem.IntroFinished += OnIntroFinished;
   }

   async void OnIntroFinished() {
      //await _gsc.World.GetElement<Fade>().FadeFromBlack();
   }

   public MenuState(GameStateController gameStateController) : base(gameStateController) { }
}