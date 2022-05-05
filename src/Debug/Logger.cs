using System.Collections.Generic;
using System.Text.Json;
using Godot;

namespace SatiRogue.Debug; 

public enum LogLevel {None, Error, Warn, Info, Debug, All}

public class Logger : Node {
    public static LogLevel Level { get; set; } = OS.HasFeature("debug") ? LogLevel.Debug : LogLevel.Error;
    private static readonly Queue<string> QueuedLogs = new Queue<string>();
    public static readonly int MaxLogsPerFrame = 10;
    
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions {
        IncludeFields = true
    };

    public static void Print(object what, LogLevel logLevel = LogLevel.Debug) {
        if (logLevel == LogLevel.All || Level >= logLevel) {
            QueuedLogs.Enqueue($"  => {logLevel} : {JsonSerializer.Serialize(what, JsonSerializerOptions)}");
        }
    }

    public override void _Process(float delta) {
        if (QueuedLogs.Count <= 0) return;
        var numToLog = Mathf.Min(MaxLogsPerFrame, QueuedLogs.Count);
        for (var i = 0; i < numToLog; i++) {
            GD.Print(QueuedLogs.Dequeue());
        }
    }
}