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

   [Export]
   public Logger.LogLevel LogLevel {
	  get => _logLevel;
	  set {
		 _logLevel = value;
		 Logger.Level = _logLevel;
	  }
   }

   [OnReady]
   private void AddGameStateController() {
	   var gsc = new GameStateController();
	   AddChild(gsc);
	   gsc.PushState(new PlayState());
   }
}
