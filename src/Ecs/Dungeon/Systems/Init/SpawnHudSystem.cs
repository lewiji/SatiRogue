using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using SatiRogue.Ecs.Dungeon.Triggers;
using SatiRogue.Ecs.Menu.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using SatiRogue.Tools;
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
   static readonly PackedScene MessageLogScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/MessageLog.tscn");
   static readonly PackedScene DebugUiScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/DebugUi.tscn");

   public void Run() {
      var playerStore = World.GetElement<PersistentPlayerData>();

      var playState = World.GetElement<DungeonState>();
      var hud = HudScene.Instance();
      var uiParent = hud.GetNode("%HudItems");
      playState.AddChild(hud);

      var healthUi = HealthUiScene.Instance<HealthUi>();
      uiParent.AddChild(healthUi);
      healthUi.Percent = playerStore.Health / (float) playerStore.Stats.Record.Health;
      World.AddOrReplaceElement(healthUi);

      var floorCounterUi = FloorCounterScene.Instance<FloorCounter>();
      uiParent.AddChild(floorCounterUi);
      World.AddOrReplaceElement(floorCounterUi);

      if (OS.HasTouchscreenUiHint()) {
         var touchControls = TouchControlsScene.Instance();
         uiParent.AddChild(touchControls);
      }

      var lootUi = LootUiScene.Instance<Loot>();
      uiParent.AddChild(lootUi);
      lootUi.NumLoots = playerStore.Gold;
      World.AddOrReplaceElement(lootUi);

      var messageLog = MessageLogScene.Instance<MessageLog>();
      uiParent.AddChild(messageLog);
      World.AddOrReplaceElement(messageLog);

      var debugUi = DebugUiScene.Instance<DebugUi>();
      uiParent.AddChild(debugUi);
      World.AddOrReplaceElement(debugUi);

      var invUi = InvUiScene.Instance<Inventory>();
      uiParent.AddChild(invUi);
      World.AddOrReplaceElement(invUi);

      var stairsConfirm = StairsConfirmationScene.Instance<StairsConfirmation>();
      uiParent.AddChild(stairsConfirm);
      World.AddOrReplaceElement(stairsConfirm);
      stairsConfirm.Connect(nameof(StairsConfirmation.StairsConfirmed), this, nameof(OnStairsDown));

      var fade = hud.GetNode<DeathScreen>("FadeCanvasLayer/Fade");
      World.AddOrReplaceElement(fade);
      fade.Connect(nameof(DeathScreen.Continue), this, nameof(OnContinueFromDeath));
      fade.Connect(nameof(DeathScreen.Exit), this, nameof(OnExitFromDeath));

      hud.GetNode<Button>("%OptionsButton").Connect("pressed", this, nameof(OnOptionsPressed));
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