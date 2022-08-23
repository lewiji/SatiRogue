using Godot;
using RelEcs;
using SatiRogue.Ecs.Core;
namespace SatiRogue.Ecs.Menu.Systems;

public class InitMenu : GdSystem {
   private static readonly PackedScene MenuScene = GD.Load<PackedScene>("res://src/Ecs/Menu/Nodes/Menu.tscn");

   public override void Run() {
      var gsc = GetElement<GameStateController>();
      var menu = MenuScene.Instance<Nodes.Menu>();
      gsc.AddChild(menu);
      AddElement(menu);
   }
}