using System.Collections.Generic;
using System.Text.Json;
using Godot;

namespace SatiRogue.Debug;

public enum LogLevel {
   None,
   Error,
   Warn,
   Info,
   Debug,
   All
}

public class Logger : Node {
   private const int MaxLogsPerFrame = 10;
   private static readonly Queue<string> QueuedLogs = new();

   private static readonly JsonSerializerOptions JsonSerializerOptions = new() {
      IncludeFields = true
   };

   public static LogLevel Level { get; set; } = OS.HasFeature("debug") ? LogLevel.Debug : LogLevel.Error;

   public static void Print(object what, LogLevel logLevel = LogLevel.Info) {
      if (logLevel == LogLevel.All || Level >= logLevel)
         QueuedLogs.Enqueue($"  => {logLevel} : {JsonSerializer.Serialize(what, JsonSerializerOptions)}");
   }

   public static void Debug(object what) {
      Print(what, LogLevel.Debug);
   }

   public static void Error(object what) {
      Print(what, LogLevel.Error);
   }

   public static void Warn(object what) {
      Print(what, LogLevel.Warn);
   }

   public static void Info(object what) {
      Print(what);
   }

   public override void _Process(float delta) {
      if (QueuedLogs.Count <= 0) return;
      var numToLog = Mathf.Min(MaxLogsPerFrame, QueuedLogs.Count);
      for (var i = 0; i < numToLog; i++) GD.Print(QueuedLogs.Dequeue());
   }
}