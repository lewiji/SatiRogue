using Godot;
using SatiRogue.Debug;
using SatiRogue.Tools;

namespace SatiRogue;

public class Main : Node {
   private Logger.LogLevel _logLevel;

   [Export]
   public Logger.LogLevel LogLevel {
	  get => _logLevel;
	  set {
		 _logLevel = value;
		 Logger.Level = _logLevel;
	  }
   }
}
