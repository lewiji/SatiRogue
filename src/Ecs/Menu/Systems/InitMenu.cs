using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Loading.Nodes;
using SatiRogue.Ecs.Menu.Nodes;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Menu.Systems;

public class InitMenu : GdSystem {
   static readonly PackedScene MenuScene = GD.Load<PackedScene>("res://src/Ecs/Menu/Nodes/Menu.tscn");
   Nodes.Menu? _menu;

   public override void Run() {
      var menuState = GetElement<MenuState>();
      _menu = MenuScene.Instance<Nodes.Menu>();
      _menu.Connect(nameof(Nodes.Menu.NewGameRequested), this, nameof(OnNewGameRequested));
      _menu.Connect(nameof(Nodes.Menu.OptionsRequested), this, nameof(OnOptionsRequested));
      menuState.AddChild(_menu);
      AddElement(_menu);
      AddElement(this);
   }

   public async void OnNewGameRequested() {
      var gsc = GetElement<GameStateController>();
      var fade = GetElement<Fade>();
      await fade.FadeToBlack();

      if (_menu != null) _menu.Visible = false;
      GetElement<Options>().Hide();

      if (!gsc.HasState<LoadingState>()) {
         var loadingState = GetElement<Main>().AddLoadingState();
         await ToSignal(loadingState, nameof(LoadingState.FinishedLoading));
         Logger.Info("Freeing shader compiler & loading state");
         var shaderCompiler = GetElement<ShaderCompiler>();
         shaderCompiler.QueueFree();
         loadingState.QueueFree();
         await ToSignal(loadingState, "tree_exited");
         await ToSignal(fade.GetTree(), "idle_frame");
         Logger.Info("Freed.");
      }

      Logger.Info("Changing to mapgen state.");
      var mapGenState = GetElement<Main>().ChangeToMapGenState();
      await ToSignal(mapGenState, nameof(MapGenState.FinishedGenerating));
      await fade.FadeFromBlack();
   }

   void OnOptionsRequested() {
      GetElement<Options>().Show();
   }
}