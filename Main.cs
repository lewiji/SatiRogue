using Godot;
using SatiRogue.Debug;
using SatiRogue.Tools;

namespace SatiRogue; 

public class Main : Node {
    [Export]
    public LogLevel LogLevel {
        get => _logLevel;
        set {
            _logLevel = value;
            Logger.Level = _logLevel;
        }
    }

    private Rng _rng = new();
    private LogLevel _logLevel;
}