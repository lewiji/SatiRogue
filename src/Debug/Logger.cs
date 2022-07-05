using System;
using System.Collections.Generic;
using Godot;
using GoDotLog;

namespace SatiRogue.Debug;

public class Logger : Node {
   public enum LogLevel {
      None,
      Error,
      Warn,
      Info,
      Debug,
      All
   }

   private const int MaxLogsPerFrame = 10;
   private static readonly Queue<KeyValuePair<LogLevel, object>> QueuedLogs = new();

   private readonly ILog _log = new GDLog(nameof(Logger));

   public static LogLevel Level { get; set; } =  LogLevel.Error;

   private static void Print(object what, LogLevel logLevel = LogLevel.Info) {
      if (logLevel == LogLevel.All || Level >= logLevel)
         QueuedLogs.Enqueue(new KeyValuePair<LogLevel, object>(logLevel, what));
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
      for (var i = 0; i < numToLog; i++) {
         var logKeyPair = QueuedLogs.Dequeue();
         var logString = logKeyPair.Value.ToString();
         switch (logKeyPair.Key) {
            case LogLevel.Error:
               _log.Error(logString);
               break;
            case LogLevel.Warn:
               _log.Warn(logString);
               break;
            case LogLevel.Info:
            case LogLevel.Debug:
            case LogLevel.All:
            case LogLevel.None:
               _log.Print(logString);
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }
   }
}