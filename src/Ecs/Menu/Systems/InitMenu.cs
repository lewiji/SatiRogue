using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Loading.Nodes;
using SatiRogue.Ecs.Menu.Nodes;
using RelEcs;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Menu.Systems;

public class InitMenu : Reference, ISystem {
   
   static readonly PackedScene MenuScene = GD.Load<PackedScene>("res://src/Ecs/Menu/Nodes/Menu.tscn");
   Nodes.Menu? _menu;
   World? _world;
   public void Run(World world)
   {
      _world ??= world;
      var menuState = world.GetElement<MenuState>();
      _menu = MenuScene.Instance<Nodes.Menu>();
      _menu.Connect(nameof(Nodes.Menu.NewGameRequested), this, nameof(OnNewGameRequested));
      _menu.Connect(nameof(Nodes.Menu.OptionsRequested), this, nameof(OnOptionsRequested));
      menuState.AddChild(_menu);
      world.AddOrReplaceElement(_menu);
      world.AddOrReplaceElement(this);
   }

   public async void OnNewGameRequested() {
      var gsc = _world!.GetElement<GameStateController>();
      var fade = _world!.GetElement<Fade>();
      await fade.FadeToBlack();

      if (_menu != null)
         _menu.Visible = false;
      _world!.GetElement<Options>().Hide();

      LoadingState? loadingState = null;

      if (!gsc.HasState<LoadingState>()) {
         loadingState = _world!.GetElement<Main>().AddLoadingState();
         await ToSignal(loadingState, nameof(LoadingState.FinishedLoading));
      }

      Logger.Info("Creating new session state.");
      _world!.GetElement<Main>().AddSessionState();
   }

   void OnOptionsRequested() {
      _world!.GetElement<Options>().Show();
   }
}