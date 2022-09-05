using Godot;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Menu.Nodes;
using SatiRogue.Ecs.Play.Nodes.Hud;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems.Init;

public class SpawnHudSystem : GdSystem {
   static readonly PackedScene HudScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/HUD.tscn");
   static readonly PackedScene HealthUiScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/Health.tscn");
   static readonly PackedScene LootUiScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/Loot.tscn");
   static readonly PackedScene InvUiScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/Inventory.tscn");

   public override void Run() {
      var gsc = GetElement<GameStateController>();
      var hud = HudScene.Instance();
      gsc.AddChild(hud);

      var healthUi = HealthUiScene.Instance<HealthUi>();
      hud.AddChild(healthUi);
      Spawn(healthUi);
      AddElement(healthUi);

      var lootUi = LootUiScene.Instance<Loot>();
      hud.AddChild(lootUi);
      Spawn(lootUi);
      AddElement(lootUi);

      var invUi = InvUiScene.Instance<Inventory>();
      hud.AddChild(invUi);
      Spawn(invUi);
      AddElement(invUi);

      var fade = hud.GetNode<DeathScreen>("FadeCanvasLayer/Fade");
      AddElement(fade);

      hud.GetNode<Button>("OptionsButton").Connect("pressed", this, nameof(OnOptionsPressed));
   }

   void OnOptionsPressed() {
      GetElement<Options>().Show();
   }
}