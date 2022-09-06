using System.Threading.Tasks;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Play.Triggers;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems;

public class LevelChangeSystem : GdSystem {
   public override void Run() {
      foreach (var stairsDown in Receive<StairsDownTrigger>()) {
         Logger.Info("Stairs down system activating.");
         var gsc = GetElement<GameStateController>();

         if (gsc.CurrentState is not PlayState) continue;
         ChangeLevel(gsc);
         break;
      }

      foreach (var restart in Receive<RestartGameTrigger>()) {
         Logger.Info("Restart game requested.");
         var gsc = GetElement<GameStateController>();
         if (gsc.CurrentState is not PlayState) continue;
         ChangeLevel(gsc);
         break;
      }

      foreach (var exit in Receive<BackToMainMenuTrigger>()) {
         Logger.Info("Exit to menu requested.");
         var gsc = GetElement<GameStateController>();
         if (gsc.CurrentState is not PlayState) continue;
         ExitToMainMenu(gsc);
         break;
      }
   }

   async void ExitToMainMenu(GameStateController gsc) {
      GetElement<Menu.Nodes.Menu>().Visible = true;
      await DestroyCurrentStates(gsc);
      var fade = GetElement<Fade>();
      await fade.FadeFromBlack();
   }

   async void ChangeLevel(GameStateController gsc) {
      await DestroyCurrentStates(gsc);
      await CreateNewMapGenState();
   }

   async Task DestroyCurrentStates(GameStateController gsc) {
      var fade = GetElement<Fade>();
      await fade.FadeToBlack();
      gsc.PopState();
      await ToSignal(gsc.GetTree(), "idle_frame");
      gsc.PopState();
      await ToSignal(gsc.GetTree(), "idle_frame");
   }

   async Task CreateNewMapGenState() {
      var mapGenState = GetElement<Main>().ChangeToMapGenState();
      await ToSignal(mapGenState, nameof(MapGenState.FinishedGenerating));
      var fade = GetElement<Fade>();
      await fade.FadeFromBlack();
   }
}