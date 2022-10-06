using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using SatiRogue.Ecs.Dungeon.Triggers;
using SatiRogue.Ecs.Menu.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems.Init;

public class SpawnHudSystem : Reference, ISystem {
   public World World { get; set; } = null!;
   static readonly PackedScene HudScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/HUD.tscn");
   static readonly PackedScene HealthUiScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/Health.tscn");
   static readonly PackedScene LootUiScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/Loot.tscn");
   static readonly PackedScene InvUiScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/Inventory.tscn");
   static readonly PackedScene StairsConfirmationScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/StairsConfirmation.tscn");
   static readonly PackedScene FloorCounterScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/FloorCounter.tscn");
   static readonly PackedScene TouchControlsScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/TouchControls/TouchControls.tscn");
   private static readonly PackedScene MessageLogScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/MessageLog.tscn");

   public void Run() {
      var playerStore = World.GetElement<PersistentPlayerData>();

      var playState = World.GetElement<DungeonState>();
      var hud = HudScene.Instance();
      var uiParent = hud.GetNode("%HudItems");
      playState.AddChild(hud);

      var healthUi = HealthUiScene.Instance<HealthUi>();
      uiParent.AddChild(healthUi);
      healthUi.Percent = playerStore.Health / (float) playerStore.Stats.Record.Health;
      World.AddElement(healthUi);

      var floorCounterUi = FloorCounterScene.Instance<FloorCounter>();
      uiParent.AddChild(floorCounterUi);
      World.AddElement(floorCounterUi);

      if (OS.HasTouchscreenUiHint()) {
         var touchControls = TouchControlsScene.Instance();
         uiParent.AddChild(touchControls);
      }

      var lootUi = LootUiScene.Instance<Loot>();
      uiParent.AddChild(lootUi);
      lootUi.NumLoots = playerStore.Gold;
      World.AddElement(lootUi);

      var messageLog = MessageLogScene.Instance<MessageLog>();
      uiParent.AddChild(messageLog);
      World.AddElement(messageLog);
      AddInitialSpawnMessageLog();

      var invUi = InvUiScene.Instance<Inventory>();
      uiParent.AddChild(invUi);
      World.AddElement(invUi);

      var stairsConfirm = StairsConfirmationScene.Instance<StairsConfirmation>();
      uiParent.AddChild(stairsConfirm);
      World.AddElement(stairsConfirm);
      stairsConfirm.Connect(nameof(StairsConfirmation.StairsConfirmed), this, nameof(OnStairsDown));

      var fade = hud.GetNode<DeathScreen>("FadeCanvasLayer/Fade");
      World.AddElement(fade);
      fade.Connect(nameof(DeathScreen.Continue), this, nameof(OnContinueFromDeath));
      fade.Connect(nameof(DeathScreen.Exit), this, nameof(OnExitFromDeath));

      hud.GetNode<Button>("%OptionsButton").Connect("pressed", this, nameof(OnOptionsPressed));
   }
   
   async void AddInitialSpawnMessageLog() {
      var msgLog = World.GetElement<MessageLog>();
      await msgLog.ToSignal(msgLog.GetTree().CreateTimer(0.618f), "timeout");
      msgLog.AddMessage("Worldling entered the dungeon realm.");
   }

   void OnOptionsPressed() {
      World.GetElement<Options>().Show();
   }

   void OnContinueFromDeath() {
      this.Send(new RestartGameTrigger());
   }

   void OnExitFromDeath() {
      this.Send(new BackToMainMenuTrigger());
   }

   void OnStairsDown() {
      Logger.Info("Hud stairs down callback triggering");
      this.Send(new StairsDownTrigger());
   }
}