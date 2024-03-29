using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using SatiRogue.Ecs.Dungeon.Triggers;
using SatiRogue.Ecs.Menu.Nodes;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems.Init;

public class SpawnHudSystem : Reference, ISystem {
   
   static readonly PackedScene HudScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/HUD.tscn");
   static readonly PackedScene HealthUiScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/Health.tscn");
   static readonly PackedScene LootUiScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/Loot.tscn");
   static readonly PackedScene InvUiScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/Inventory.tscn");
   static readonly PackedScene StairsConfirmationScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/StairsConfirmation.tscn");
   static readonly PackedScene FloorCounterScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/FloorCounter.tscn");
   static readonly PackedScene TouchControlsScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/TouchControls/TouchControls.tscn");
   static readonly PackedScene MessageLogScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Hud/MessageLog.tscn");

   World? _world;
   
   public void Run(World world)
   {
      _world ??= world;
      var playerStore = world.GetElement<PersistentPlayerData>();

      var playState = world.GetElement<DungeonState>();
      var hud = HudScene.Instance<Hud>();
      var uiParent = hud.GetNode("%HudItems");
      playState.AddChild(hud);
      world.AddOrReplaceElement(hud);

      var healthUi = HealthUiScene.Instance<HealthUi>();
      uiParent.AddChild(healthUi);
      healthUi.Percent = playerStore.Health / (float) playerStore.Stats.Record.Health;
      world.AddOrReplaceElement(healthUi);

      var floorCounterUi = FloorCounterScene.Instance<FloorCounter>();
      uiParent.AddChild(floorCounterUi);
      world.AddOrReplaceElement(floorCounterUi);

      if (OS.HasTouchscreenUiHint()) {
         var touchControls = TouchControlsScene.Instance();
         uiParent.AddChild(touchControls);
      }

      var lootUi = LootUiScene.Instance<Loot>();
      uiParent.AddChild(lootUi);
      lootUi.NumLoots = playerStore.Gold;
      world.AddOrReplaceElement(lootUi);

      var messageLog = MessageLogScene.Instance<MessageLog>();
      uiParent.AddChild(messageLog);
      world.AddOrReplaceElement(messageLog);

      var invUi = InvUiScene.Instance<Inventory>();
      uiParent.AddChild(invUi);
      world.AddOrReplaceElement(invUi);

      var stairsConfirm = StairsConfirmationScene.Instance<StairsConfirmation>();
      uiParent.AddChild(stairsConfirm);
      world.AddOrReplaceElement(stairsConfirm);
      stairsConfirm.Connect(nameof(StairsConfirmation.StairsConfirmed), this, nameof(OnStairsDown));

      var fade = hud.GetNode<DeathScreen>("FadeCanvasLayer/Fade");
      world.AddOrReplaceElement(fade);
      fade.Connect(nameof(DeathScreen.Continue), this, nameof(OnContinueFromDeath));
      fade.Connect(nameof(DeathScreen.Exit), this, nameof(OnExitFromDeath));

      hud.GetNode<Button>("%OptionsButton").Connect("pressed", this, nameof(OnOptionsPressed));
   }

   void OnOptionsPressed() {
      _world!.GetElement<Options>().Show();
   }

   void OnContinueFromDeath() {
      _world!.Send(new RestartGameTrigger());
   }

   void OnExitFromDeath() {
      _world!.Send(new BackToMainMenuTrigger());
   }

   void OnStairsDown() {
      Logger.Info("Hud stairs down callback triggering");
      _world!.Send(new StairsDownTrigger());
   }
}