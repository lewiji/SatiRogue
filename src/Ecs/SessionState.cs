using System.Threading.Tasks;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Loading.Nodes;
using SatiRogue.Ecs.Session;
using SatiRogue.Tools;
namespace SatiRogue.Ecs;

public class SessionState : GameState {
   public override void Init(GameStateController gameStateController) {
      CreateSystems(gameStateController);
      InitMapGen(gameStateController);
   }

   void CreateSystems(GameStateController gameStateController) {
      gameStateController?.World.AddOrReplaceElement(this);

      InitSystems.Add(new InitPersistentPlayerData());
   }

   async void InitMapGen(GameStateController gameStateController) {
      var mapGenState = ChangeToMapGenState(gameStateController);
      await ToSignal(mapGenState, nameof(MapGenState.FinishedGenerating));
      await StartShaderCompiler(gameStateController);
   }

   async Task StartShaderCompiler(GameStateController gameStateController) {
      var shaderCompiler = gameStateController.World.GetElement<ShaderCompiler>();
      shaderCompiler.Visible = false;

      var fade = gameStateController.World.GetElement<Fade>();
      await fade.FadeFromBlack();

      Logger.Info("Freeing shader compiler & loading state");
      
      await ToSignal(fade.GetTree(), "idle_frame");
      await ToSignal(fade.GetTree(), "idle_frame");
      await ToSignal(fade.GetTree(), "idle_frame");
      await ToSignal(fade.GetTree(), "idle_frame");
      await ToSignal(fade.GetTree(), "idle_frame");
      var loadingState = gameStateController.World.GetElement<LoadingState>();
      loadingState?.QueueFree();
      Logger.Info("Freed.");
   }

   MapGenState ChangeToMapGenState(GameStateController gameStateController) {
      var mapGenState = new MapGenState(gameStateController);
      gameStateController.PushState(mapGenState);
      return mapGenState;
   }

   public void ChangeToDungeonState(GameStateController gameStateController) {
      var playState = new DungeonState(gameStateController);
      gameStateController.PushState(playState);
   }

   public SessionState(GameStateController gameStateController) : base(gameStateController) { }
}