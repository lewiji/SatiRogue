using System.Text.Json;
using Godot;

namespace SatiRogue.Debug; 

public enum LogLevel {None, Error, Warn, Info, Debug, All}

public static class Logger {
    private static LogLevel _level = OS.HasFeature("debug") ? LogLevel.Info : LogLevel.Error;
    public static LogLevel Level {
        get => _level;
        set => _level = value;
    }
    
    private static JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions {
        IncludeFields = true
    };

    public static void Print(object what, LogLevel logLevel = LogLevel.Debug) {
        if (logLevel == LogLevel.All || _level >= logLevel) {
            GD.PrintS("  =>", logLevel, ":", JsonSerializer.Serialize(what, _jsonSerializerOptions));
        }
    }
}