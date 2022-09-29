using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Menu.Nodes;
using SatiRogue.Ecs.Play.Nodes.Hud;
using SatiRogue.Ecs.Play.Triggers;
using RelEcs;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Systems.Init;

public class SpawnHudSystem : Reference, ISystem {
   public World World { get; set; } = null!;
   static readonly PackedScene HudScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/HUD.tscn");
   static readonly PackedScene HealthUiScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/Health.tscn");
   static readonly PackedScene LootUiScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/Loot.tscn");
   static readonly PackedScene InvUiScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/Inventory.tscn");
   static readonly PackedScene StairsConfirmationScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/StairsConfirmation.tscn");
   static readonly PackedScene FloorCounterScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/FloorCounter.tscn");
   static readonly PackedScene TouchControlsScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/TouchControls/TouchControls.tscn");

   public void Run() {
      var playState = World.GetElement<PlayState>();
      var hud = HudScene.Instance();
      var uiParent = hud.GetNode("%HudItems");
      playState.AddChild(hud);

      var healthUi = HealthUiScene.Instance<HealthUi>();
      uiParent.AddChild(healthUi);
      World.AddElement(healthUi);

      var floorCounterUi = FloorCounterScene.Instance<FloorCounter>();
      uiParent.AddChild(floorCounterUi);
      World.AddElement(floorCounterUi);

      var touchControls = TouchControlsScene.Instance();
      uiParent.AddChild(touchControls);

      var lootUi = LootUiScene.Instance<Loot>();
      uiParent.AddChild(lootUi);
      World.AddElement(lootUi);

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