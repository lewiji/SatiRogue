using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Loading.Nodes;
using SatiRogue.Ecs.Menu.Nodes;
using RelEcs;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Menu.Systems;

public class InitMenu : Reference, ISystem {
   public World World { get; set; } = null!;
   static readonly PackedScene MenuScene = GD.Load<PackedScene>("res://src/Ecs/Menu/Nodes/Menu.tscn");
   Nodes.Menu? _menu;

   public void Run() {
      var menuState = World.GetElement<MenuState>();
      _menu = MenuScene.Instance<Nodes.Menu>();
      _menu.Connect(nameof(Nodes.Menu.NewGameRequested), this, nameof(OnNewGameRequested));
      _menu.Connect(nameof(Nodes.Menu.OptionsRequested), this, nameof(OnOptionsRequested));
      menuState.AddChild(_menu);
      World.AddElement(_menu);
      World.AddElement(this);
   }

   public async void OnNewGameRequested() {
      var gsc = World.GetElement<GameStateController>();
      var fade = World.GetElement<Fade>();
      await fade.FadeToBlack();

      if (_menu != null)
         _menu.Visible = false;
      World.GetElement<Options>().Hide();

      if (!gsc.HasState<LoadingState>()) {
         var loadingState = World.GetElement<Main>().AddLoadingState();
         await ToSignal(loadingState, nameof(LoadingState.FinishedLoading));
         Logger.Info("Freeing shader compiler & loading state");
         var shaderCompiler = World.GetElement<ShaderCompiler>();
         shaderCompiler.QueueFree();
         loadingState.QueueFree();
         await ToSignal(loadingState, "tree_exited");
         await ToSignal(fade.GetTree(), "idle_frame");
         Logger.Info("Freed.");
      }

      Logger.Info("Changing to mapgen state.");
      var mapGenState = World.GetElement<Main>().ChangeToMapGenState();
      await ToSignal(mapGenState, nameof(MapGenState.FinishedGenerating));
      await fade.FadeFromBlack();
   }

   void OnOptionsRequested() {
      World.GetElement<Options>().Show();
   }
}