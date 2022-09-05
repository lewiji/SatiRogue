using System;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Debug;
using SatiRogue.Ecs;
using SatiRogue.Ecs.Core;
namespace SatiRogue;

public partial class Main : Node {
   Logger.LogLevel _logLevel;
   GameStateController _gsc = null!;
   float _totalObjects;
   float _lastObjects;

   [Export] public Logger.LogLevel LogLevel {
      get => _logLevel;
      set {
         _logLevel = value;
         Logger.Level = _logLevel;
      }
   }

   [OnReady] void CreateGameStateController() {
      _gsc = new GameStateController();
      AddChild(_gsc);
      _gsc.World.AddElement(this);
   }

   [OnReady] void AddWorldEnvironmentElement() {
      _gsc.World.AddElement(GetNode<WorldEnvironment>("WorldEnvironment"));
   }

   [OnReady] void AddCoreState() {
      var coreState = new CoreState();
      _gsc.PushState(coreState);
   }

   public LoadingState AddLoadingState() {
      var loading = new LoadingState();
      _gsc.PushState(loading);
      return loading;
   }

   [OnReady] void AddMenuState() {
      var menuState = new MenuState();
      _gsc.PushState(menuState);
   }

   public MapGenState ChangeToMapGenState() {
      var mapGenState = new MapGenState();
      _gsc.PushState(mapGenState);
      return mapGenState;
   }

   public void ChangeToPlayState() {
      var playState = new PlayState();
      _gsc.PushState(playState);
   }

   [OnReady] void CreateMonitorTimer() {
      if (LogLevel > Logger.LogLevel.Debug) return;

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

   [OnReady] void CreateManualGcTimer() {
      if (LogLevel > Logger.LogLevel.Debug) return;

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