using System;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Debug;
using SatiRogue.Ecs;
using SatiRogue.Ecs.Core;
using SatiRogue.resources;

namespace SatiRogue;

public partial class Main : Node {
   Logger.LogLevel _logLevel;
   GameStateController _gsc = null!;
   float _totalObjects;
   float _lastObjects;

   [Export]
   public Logger.LogLevel LogLevel {
      get => _logLevel;
      set {
         _logLevel = value;
         Logger.Level = _logLevel;
      }
   }

   [OnReady]
   void CreateGameStateController() {
      _gsc = new GameStateController();
      AddChild(_gsc);
      _gsc.World.AddElement(this);
   }

   [OnReady]
   void LoadSatiConfig() {
      var config = GD.Load<Resource>("res://sati_config.tres");

      if (config is SatiConfig satiConfig) {
         _gsc.World.AddElement(satiConfig);
      }
   }

   [OnReady]
   void AddWorldEnvironmentElement() {
      var worldEnvironment = GetNode<WorldEnvironment>("WorldEnvironment");
      _gsc.World.AddElement(worldEnvironment);

      var environment = GD.Load<Godot.Environment>(OS.GetName() == "Android"
         ? "res://scenes/res/EnvironmentAndroid.tres"
         : "res://scenes/res/EnvironmentBase.tres");
      worldEnvironment.Environment = environment;
   }

   [OnReady]
   void AddCoreState() {
      var coreState = new CoreState();
      _gsc.PushState(coreState);
   }

   public LoadingState AddLoadingState() {
      var loading = new LoadingState();
      _gsc.PushState(loading);
      return loading;
   }

   [OnReady]
   void AddMenuState() {
      var menuState = new MenuState();
      _gsc.PushState(menuState);
   }

   public void AddSessionState() {
      var sessionState = new SessionState();
      _gsc.PushState(sessionState);
   }

   void CreateMonitorTimer() {
      if (LogLevel > Logger.LogLevel.Debug)
         return;

      var timer = new Timer {WaitTime = 1f, Autostart = true};
      timer.Connect("timeout", this, nameof(CheckMonitors));
      AddChild(timer);

      Logger.Warn("ObjectCount Monitor logging is switched on.");
   }

   void CheckMonitors() {
      _lastObjects = _totalObjects;
      _totalObjects = Performance.GetMonitor(Performance.Monitor.ObjectCount);

      var delta = _totalObjects - _lastObjects;

      Logger.Debug($"{_totalObjects}, {delta}");
   }

   void CreateManualGcTimer() {
      if (LogLevel > Logger.LogLevel.Debug)
         return;

      var timer = new Timer {WaitTime = 3f, Autostart = true};
      timer.Connect("timeout", this, nameof(ClearGc));
      AddChild(timer);

      Logger.Warn("Manual GC is switched on.");
   }

   void ClearGc() {
      GC.Collect();
      GC.WaitForPendingFinalizers();
   }
}