using Godot;
using GoDotNet;
using GodotOnReady.Attributes;
using SatiRogue.Debug;
using SatiRogue.Grid.MapGen;
using SatiRogue.RelEcs;
using SatiRogue.RelEcs.States;
using SatiRogue.Tools;

namespace SatiRogue;

public partial class Main : Node {
   private Logger.LogLevel _logLevel;
   private GameStateController _gsc;

   [Export]
   public Logger.LogLevel LogLevel {
	  get => _logLevel;
	  set {
		 _logLevel = value;
		 Logger.Level = _logLevel;
	  }
   }


   [OnReady]
   private void CreateGameStateController() {
	   _gsc = new GameStateController();
	   AddChild(_gsc);
	   _gsc.World.AddElement(this);
   }

   [OnReady]
   private void AddMapGenState() {
	   var mapGenState = new MapGenState();
	   _gsc.PushState(mapGenState);
   }

   public void OnMapGenInitFinished() {
	   var playState = new PlayState();
	   _gsc.PushState(playState);
   }
}
