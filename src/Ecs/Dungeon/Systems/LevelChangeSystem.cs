using System.Threading.Tasks;
using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using SatiRogue.Ecs.Dungeon.Triggers;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class LevelChangeSystem : Reference, ISystem {
   public World World { get; set; } = null!;
   bool _firstRun = true;

   public void Run() {
      var playerStore = World.GetElement<PersistentPlayerData>();

      foreach (var stairsDown in this.Receive<StairsDownTrigger>()) {
         Logger.Info("Stairs down system activating.");
         var gsc = World.GetElement<GameStateController>();

         if (gsc.CurrentState is not DungeonState)
            continue;
         playerStore.Floor -= 1;
         ChangeLevel(gsc);
         break;
      }

      foreach (var restart in this.Receive<RestartGameTrigger>()) {
         Logger.Info("Restart game requested.");
         var gsc = World.GetElement<GameStateController>();

         if (gsc.CurrentState is not DungeonState)
            continue;
         playerStore.Reset();
         ChangeLevel(gsc);
         break;
      }

      foreach (var exit in this.Receive<BackToMainMenuTrigger>()) {
         Logger.Info("Exit to menu requested.");
         var gsc = World.GetElement<GameStateController>();

         if (gsc.CurrentState is not DungeonState)
            continue;
         playerStore.Floor = 0;
         ExitToMainMenu(gsc);
         break;
      }

      if (!_firstRun)
         return;
      World.AddElement(this);
      World.GetElement<FloorCounter>().FloorNumber = playerStore.Floor;
      _firstRun = false;
      AddInitialSpawnMessageLog();
   }

   async void AddInitialSpawnMessageLog() {
      var playerStore = World.GetElement<PersistentPlayerData>();
      var msgLog = World.GetElement<MessageLog>();
      await msgLog.ToSignal(msgLog.GetTree().CreateTimer(0.618f), "timeout");
      msgLog.AddMessage($"Worldling {playerStore.PlayerName} entered the dungeon realm.");
   }

   async void AddNewFloorSpawnMessageLog() {
      var playerStore = World.GetElement<PersistentPlayerData>();
      var msgLog = World.GetElement<MessageLog>();
      await msgLog.ToSignal(msgLog.GetTree().CreateTimer(0.618f), "timeout");
      msgLog.AddMessage($"Worldling {playerStore.PlayerName} descended to floor {playerStore.Floor}.");
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
      AddNewFloorSpawnMessageLog();
   }
}