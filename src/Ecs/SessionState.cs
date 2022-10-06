using System.Threading.Tasks;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Loading.Nodes;
using SatiRogue.Ecs.Session;
namespace SatiRogue.Ecs; 

public class SessionState : GameState {
   public override void Init(GameStateController gameStateController) {
      CreateSystems(gameStateController);
      InitMapGen(gameStateController);
   }
   private async void InitMapGen(GameStateController gameStateController) {

      var mapGenState = ChangeToMapGenState(gameStateController);
      await ToSignal(mapGenState, nameof(MapGenState.FinishedGenerating));
      var shaderCompiler = gameStateController.World.GetElement<ShaderCompiler>();
      shaderCompiler.Visible = false;
      
      var fade = gameStateController.World.GetElement<Fade>();
      await fade.FadeFromBlack();

      Logger.Info("Freeing shader compiler & loading state");
      shaderCompiler.QueueFree();
      var loadingState = gameStateController.World.GetElement<LoadingState>();
      loadingState?.QueueFree();
      await ToSignal(loadingState, "tree_exited");
      await ToSignal(fade.GetTree(), "idle_frame");
      Logger.Info("Freed.");
   }

   void CreateSystems(GameStateController gameStateController) {
      gameStateController?.World.AddElement(this);
      
      InitSystems.Add(new InitPersistentPlayerData());
   }

   private MapGenState ChangeToMapGenState(GameStateController gameStateController) {
      var mapGenState = new MapGenState();
      gameStateController.PushState(mapGenState);
      return mapGenState;
   }

   public void ChangeToDungeonState(GameStateController gameStateController) {
      var playState = new DungeonState();
      gameStateController.PushState(playState);
   }
}