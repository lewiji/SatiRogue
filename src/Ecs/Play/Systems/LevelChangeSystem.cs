using System.Threading.Tasks;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Play.Nodes.Hud;
using SatiRogue.Ecs.Play.Triggers;
using RelEcs;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Systems;

public class LevelChangeSystem : Reference, ISystem {
   public World World { get; set; } = null!;
   public static int FloorNumber { get; set; }
   bool _firstRun = true;

   public void Run() {
      foreach (var stairsDown in this.Receive<StairsDownTrigger>()) {
         Logger.Info("Stairs down system activating.");
         var gsc = World.GetElement<GameStateController>();

         if (gsc.CurrentState is not PlayState)
            continue;
         FloorNumber -= 1;
         ChangeLevel(gsc);
         break;
      }

      foreach (var restart in this.Receive<RestartGameTrigger>()) {
         Logger.Info("Restart game requested.");
         var gsc = World.GetElement<GameStateController>();

         if (gsc.CurrentState is not PlayState)
            continue;
         FloorNumber = 0;
         ChangeLevel(gsc);
         break;
      }

      foreach (var exit in this.Receive<BackToMainMenuTrigger>()) {
         Logger.Info("Exit to menu requested.");
         var gsc = World.GetElement<GameStateController>();

         if (gsc.CurrentState is not PlayState)
            continue;
         FloorNumber = 0;
         ExitToMainMenu(gsc);
         break;
      }

      if (!_firstRun)
         return;
      World.AddElement(this);
      World.GetElement<FloorCounter>().FloorNumber = FloorNumber;
      _firstRun = false;
   }

   async void ExitToMainMenu(GameStateController gsc) {
      World.GetElement<Menu.Nodes.Menu>().Visible = true;
      await DestroyCurrentStates(gsc);
      var fade = World.GetElement<Fade>();
      await fade.FadeFromBlack();
   }

   async void ChangeLevel(GameStateController gsc) {
      await DestroyCurrentStates(gsc);
      await CreateNewMapGenState();
   }

   async Task DestroyCurrentStates(GameStateController gsc) {
      var fade = World.GetElement<Fade>();
      await fade.FadeToBlack();
      gsc.PopState();
      await ToSignal(gsc.GetTree(), "idle_frame");
      gsc.PopState();
   }

   async Task CreateNewMapGenState() {
      GD.Print("Awaiting new mapgen");
      var mapGenState = World.GetElement<MapGenState>();
      await ToSignal(mapGenState, nameof(MapGenState.FinishedGenerating));
      var fade = World.GetElement<Fade>();
      await fade.FadeFromBlack();
      InputSystem.Paused = false;
   }
}