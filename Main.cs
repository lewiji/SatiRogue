using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs;
using SatiRogue.Ecs.Core;
using SatiRogue.resources;

namespace SatiRogue;

public partial class Main : Node {
   Logger.LogLevel _logLevel;
   GameStateController _gsc = null!;
   float _totalObjects;
   float _lastObjects;

   [Export]
   public Logger.LogLevel LogLevel {
	  get => _logLevel;
	  set {
		 _logLevel = value;
		 Logger.Level = _logLevel;
	  }
   }

   public override void _Ready()
   {
	   CreateGameStateController();
	   LoadSatiConfig();
	   AddWorldEnvironmentElement();
	   AddCoreState();
	   AddMenuState();
   }

   void CreateGameStateController() {
	  _gsc = new GameStateController();
	  AddChild(_gsc);
	  _gsc.World.AddOrReplaceElement(this);
   }

   void LoadSatiConfig() {
	  var config = GD.Load<Resource>("res://sati_config.tres");

	  if (config is SatiConfig satiConfig) {
		 _gsc.World.AddOrReplaceElement(satiConfig);
	  }
   }

   void AddWorldEnvironmentElement() {
	  var worldEnvironment = GetNode<WorldEnvironment>("WorldEnvironment");
	  _gsc.World.AddOrReplaceElement(worldEnvironment);

	  var environment = GD.Load<Godot.Environment>(OS.GetName() == "Android"
		 ? "res://scenes/res/EnvironmentAndroid.tres"
		 : "res://scenes/res/EnvironmentBase.tres");
	  worldEnvironment.Environment = environment;
   }

   void AddCoreState() {
	  var coreState = new CoreState(_gsc);
	  _gsc.PushState(coreState);
   }

   public LoadingState AddLoadingState() {
	  var loading = new LoadingState(_gsc);
	  _gsc.PushState(loading);
	  return loading;
   }

   void AddMenuState() {
	  var menuState = new MenuState(_gsc);
	  _gsc.PushState(menuState);
   }

   public void AddSessionState() {
	  var sessionState = new SessionState(_gsc);
	  _gsc.PushState(sessionState);
   }
}
