using Godot;
using RelEcs;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Nodes;
namespace SatiRogue.Ecs.Menu.Systems;

public class InitMenu : GdSystem {
   private static readonly PackedScene MenuScene = GD.Load<PackedScene>("res://src/Ecs/Menu/Nodes/Menu.tscn");

   public override void Run() {
      var menuState = GetElement<MenuState>();
      var menu = MenuScene.Instance<Nodes.Menu>();
      menu.Connect(nameof(Nodes.Menu.NewGameRequested), this, nameof(OnNewGameRequested));
      menuState.AddChild(menu);
      AddElement(menu);
   }

   private async void OnNewGameRequested() {
      var fade = GetElement<Fade>();
      await ToSignal(fade.GetTree(), "idle_frame");
      await fade.FadeToBlack();
      GetElement<Main>().ChangeToMapGenState();
      await ToSignal(fade.GetTree(), "idle_frame");
      await fade.FadeFromBlack();
   }
}