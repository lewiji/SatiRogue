using Godot;
using RelEcs;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Loading.Nodes;
using SatiRogue.Ecs.Menu.Nodes;
namespace SatiRogue.Ecs.Menu.Systems;

public class InitMenu : GdSystem {
   private static readonly PackedScene MenuScene = GD.Load<PackedScene>("res://src/Ecs/Menu/Nodes/Menu.tscn");
   private Nodes.Menu? _menu;
   
   public override async void Run() {
      var menuState = GetElement<MenuState>();
      _menu = MenuScene.Instance<Nodes.Menu>();
      _menu.Connect(nameof(Nodes.Menu.NewGameRequested), this, nameof(OnNewGameRequested));
      _menu.Connect(nameof(Nodes.Menu.OptionsRequested), this, nameof(OnOptionsRequested));
      menuState.AddChild(_menu);
      AddElement(_menu);
   }

   private async void OnNewGameRequested() {
      var fade = GetElement<Fade>();
      await fade.FadeToBlack();
      _menu.Visible = false;
      GetElement<Options>().Hide();
      var loadingState = GetElement<Main>().AddLoadingState();
      await ToSignal(loadingState, nameof(LoadingState.FinishedLoading));
      GetElement<ShaderCompiler>().QueueFree();
      var mapGenState = GetElement<Main>().ChangeToMapGenState();
      await ToSignal(mapGenState, nameof(MapGenState.FinishedGenerating));
      await fade.FadeFromBlack();
   }

   private void OnOptionsRequested() {
      GetElement<Options>().Show();
   }
}