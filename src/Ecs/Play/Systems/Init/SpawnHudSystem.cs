using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Menu.Nodes;
using SatiRogue.Ecs.Play.Nodes.Hud;
using SatiRogue.Ecs.Play.Triggers;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems.Init;

public class SpawnHudSystem : GdSystem {
   static readonly PackedScene HudScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/HUD.tscn");
   static readonly PackedScene HealthUiScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/Health.tscn");
   static readonly PackedScene LootUiScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/Loot.tscn");
   static readonly PackedScene InvUiScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/Inventory.tscn");
   static readonly PackedScene StairsConfirmationScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/StairsConfirmation.tscn");
   static readonly PackedScene FloorCounterScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/FloorCounter.tscn");
   static readonly PackedScene TouchControlsScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/TouchControls/TouchControls.tscn");

   public override void Run() {
      var playState = GetElement<PlayState>();
      var hud = HudScene.Instance();
      var uiParent = hud.GetNode("%HudItems");
      playState.AddChild(hud);

      var healthUi = HealthUiScene.Instance<HealthUi>();
      uiParent.AddChild(healthUi);
      AddElement(healthUi);

      var floorCounterUi = FloorCounterScene.Instance<FloorCounter>();
      uiParent.AddChild(floorCounterUi);
      AddElement(floorCounterUi);

      var touchControls = TouchControlsScene.Instance();
      uiParent.AddChild(touchControls);

      var lootUi = LootUiScene.Instance<Loot>();
      uiParent.AddChild(lootUi);
      AddElement(lootUi);

      var invUi = InvUiScene.Instance<Inventory>();
      uiParent.AddChild(invUi);
      AddElement(invUi);

      var stairsConfirm = StairsConfirmationScene.Instance<StairsConfirmation>();
      uiParent.AddChild(stairsConfirm);
      AddElement(stairsConfirm);
      stairsConfirm.Connect(nameof(StairsConfirmation.StairsConfirmed), this, nameof(OnStairsDown));

      var fade = hud.GetNode<DeathScreen>("FadeCanvasLayer/Fade");
      AddElement(fade);
      fade.Connect(nameof(DeathScreen.Continue), this, nameof(OnContinueFromDeath));
      fade.Connect(nameof(DeathScreen.Exit), this, nameof(OnExitFromDeath));

      hud.GetNode<Button>("%OptionsButton").Connect("pressed", this, nameof(OnOptionsPressed));
   }

   void OnOptionsPressed() {
      GetElement<Options>().Show();
   }

   void OnContinueFromDeath() {
      Send(new RestartGameTrigger());
   }

   void OnExitFromDeath() {
      Send(new BackToMainMenuTrigger());
   }

   void OnStairsDown() {
      Logger.Info("Hud stairs down callback triggering");
      Send(new StairsDownTrigger());
   }
}