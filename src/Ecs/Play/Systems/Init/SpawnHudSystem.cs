using Godot;
using RelEcs;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Play.Nodes.Hud;
namespace SatiRogue.Ecs.Play.Systems.Init;

public class SpawnHudSystem : GDSystem {
   private static readonly PackedScene HudScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/HUD.tscn");
   private static readonly PackedScene HealthUiScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/Health.tscn");
   private static readonly PackedScene LootUiScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/Loot.tscn");
   private static readonly PackedScene InvUiScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Hud/Inventory.tscn");

   public override void Run() {
      var gsc = GetElement<GameStateController>();
      var hud = HudScene.Instance();
      gsc.AddChild(hud);

      var healthUi = HealthUiScene.Instance<HealthUI>();
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

      var fade = hud.GetNode<Fade>("FadeCanvasLayer/Fade");
      AddElement(fade);
   }
}