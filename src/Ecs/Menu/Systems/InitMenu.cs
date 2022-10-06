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

      LoadingState? loadingState = null;

      if (!gsc.HasState<LoadingState>()) {
         loadingState = World.GetElement<Main>().AddLoadingState();
         await ToSignal(loadingState, nameof(LoadingState.FinishedLoading));
      }

      Logger.Info("Creating new session state.");
      World.GetElement<Main>().AddSessionState();
   }

   void OnOptionsRequested() {
      World.GetElement<Options>().Show();
   }
}