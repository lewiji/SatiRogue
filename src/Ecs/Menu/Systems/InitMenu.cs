using Godot;
using RelEcs;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Loading.Nodes;
namespace SatiRogue.Ecs.Menu.Systems;

public class InitMenu : GdSystem {
   private static readonly PackedScene MenuScene = GD.Load<PackedScene>("res://src/Ecs/Menu/Nodes/Menu.tscn");
   private Nodes.Menu? _menu;
   
   public override async void Run() {
      var menuState = GetElement<MenuState>();
      _menu = MenuScene.Instance<Nodes.Menu>();
      _menu.Connect(nameof(Nodes.Menu.NewGameRequested), this, nameof(OnNewGameRequested));
      menuState.AddChild(_menu);
      AddElement(_menu);
      await ToSignal(menuState.GetTree(), "idle_frame");
      await GetElement<Fade>().FadeFromBlack();
   }

   private async void OnNewGameRequested() {
      var fade = GetElement<Fade>();
      await fade.FadeToBlack();
      GetElement<MenuState>().Visible = false;
      GetElement<Main>().AddLoadingState();
      await ToSignal(fade.GetTree().CreateTimer(1.618f), "timeout");
      GetElement<ShaderCompiler>().QueueFree();
      GetElement<Main>().ChangeToMapGenState();
      await ToSignal(fade.GetTree(), "idle_frame");
      await fade.FadeFromBlack();
   }
}