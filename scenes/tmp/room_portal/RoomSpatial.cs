using Godot;
using SatiRogue.Debug;

namespace SatiRogue.scenes.tmp.room_portal; 

public class RoomSpatial : Spatial
{
    public override void _EnterTree() {
        //GdExtensions.Print("Test");
    }

    public override void _Ready()
    {
        Logger.Print("Test default");
        _testAllDebugLevels();
        Logger.Print("Setting LogLevel to Warn", LogLevel.All);
        Logger.Level = LogLevel.Warn;
        _testAllDebugLevels();
    }

    private void _testAllDebugLevels() {
        Logger.Print("Test all", LogLevel.All);
        Logger.Print("Test debug", LogLevel.Debug);
        Logger.Print("Test info", LogLevel.Info);
        Logger.Print("Test warn", LogLevel.Warn);
        Logger.Print("Test error", LogLevel.Error);
    }
}