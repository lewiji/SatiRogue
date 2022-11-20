using System.Threading.Tasks;
using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using SatiRogue.Ecs.Dungeon.Triggers;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class LevelChangeSystem : Reference, ISystem {
   
   bool _firstRun = true;
   World? _world;
   
   public void Run(World world)
   {
      _world ??= world;
      var playerStore = world.GetElement<PersistentPlayerData>();

      foreach (var stairsDown in world.Receive<StairsDownTrigger>(this)) {
         Logger.Info("Stairs down system activating.");
         var gsc = world.GetElement<GameStateController>();

         if (gsc.CurrentState is not DungeonState)
            continue;
         playerStore.Floor -= 1;
         ChangeLevel(gsc);
         break;
      }

      foreach (var restart in world.Receive<RestartGameTrigger>(this)) {
         Logger.Info("Restart game requested.");
         var gsc = world.GetElement<GameStateController>();

         if (gsc.CurrentState is not DungeonState)
            continue;
         playerStore.Reset();
         ChangeLevel(gsc);
         break;
      }

      foreach (var exit in world.Receive<BackToMainMenuTrigger>(this)) {
         Logger.Info("Exit to menu requested.");
         var gsc = world.GetElement<GameStateController>();

         if (gsc.CurrentState is not DungeonState)
            continue;
         playerStore.Floor = 0;
         ExitToMainMenu(gsc);
         break;
      }

      if (!_firstRun)
         return;
      world.AddOrReplaceElement(this);
      world.GetElement<FloorCounter>().FloorNumber = playerStore.Floor;
      _firstRun = false;
      AddInitialSpawnMessageLog();
   }

   async void AddInitialSpawnMessageLog() {
      var playerStore = _world!.GetElement<PersistentPlayerData>();
      var msgLog = _world!.GetElement<MessageLog>();
      await msgLog.ToSignal(msgLog.GetTree().CreateTimer(0.618f), "timeout");
      msgLog.AddMessage($"Worldling {playerStore.PlayerName} entered the dungeon realm.");
   }

   async void AddNewFloorSpawnMessageLog() {
      var playerStore = _world!.GetElement<PersistentPlayerData>();
      var msgLog = _world!.GetElement<MessageLog>();
      await msgLog.ToSignal(msgLog.GetTree().CreateTimer(0.618f), "timeout");
      msgLog.AddMessage($"Worldling {playerStore.PlayerName} descended to floor {playerStore.Floor}.");
   }

   async void ExitToMainMenu(GameStateController gsc) {
      _world!.GetElement<Menu.Nodes.Menu>().Visible = true;
      await DestroyCurrentStates(gsc);
      var fade = _world!.GetElement<Fade>();
      await fade.FadeFromBlack();
   }

   async void ChangeLevel(GameStateController gsc) {
      await DestroyCurrentStates(gsc);
      await CreateNewMapGenState();
   }

   async Task DestroyCurrentStates(GameStateController gsc) {
      var fade = _world!.GetElement<Fade>();
      await fade.FadeToBlack();
      gsc.PopState();
      await ToSignal(gsc.GetTree(), "idle_frame");
      gsc.PopState();
   }

   async Task CreateNewMapGenState() {
      GD.Print("Awaiting new mapgen");
      var mapGenState = _world!.GetElement<MapGenState>();
      await ToSignal(mapGenState, nameof(MapGenState.FinishedGenerating));
      await mapGenState.ToSignal(mapGenState.GetTree().CreateTimer(0.618f), "timeout");
      var fade = _world!.GetElement<Fade>();
      await fade.FadeFromBlack();
      InputSystem.Paused = false;
      AddNewFloorSpawnMessageLog();
   }
}