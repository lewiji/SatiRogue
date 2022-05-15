using Godot;
using SatiRogue.Debug;
using SatiRogue.Tools;

namespace SatiRogue;

public class Main : Node {
   private LogLevel _logLevel;

   private Rng _rng = new();

   [Export]
   public LogLevel LogLevel {
      get => _logLevel;
      set {
         _logLevel = value;
         Logger.Level = _logLevel;
      }
   }
}