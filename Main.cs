using System;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Debug;
using SatiRogue.Ecs;
using SatiRogue.Ecs.Core;

namespace SatiRogue;

public partial class Main : Node {
   private Logger.LogLevel _logLevel;
   private GameStateController _gsc = null!;
   private float _totalObjects = 0;
   private float _lastObjects = 0;

   [Export]
   public Logger.LogLevel LogLevel {
      get => _logLevel;
      set {
         _logLevel = value;
         Logger.Level = _logLevel;
      }
   }

   [OnReady]
   private void CreateGameStateController() {
      _gsc = new GameStateController();
      AddChild(_gsc);
      _gsc.World.AddElement(this);
   }
   
   
   [OnReady]
   private void AddWorldEnvironmentElement() {
      _gsc.World.AddElement(GetNode<WorldEnvironment>("WorldEnvironment"));
   }

   [OnReady]
   private void AddCoreState() {
      var coreState = new CoreState();
      _gsc.PushState(coreState);
   }
   
   public void AddLoadingState() {
      var loading = new LoadingState();
      _gsc.PushState(loading);
   }

   [OnReady]
   private async void AddMenuState() {
      await ToSignal(GetTree().CreateTimer(0.32f), "timeout");
      var menuState = new MenuState();
      _gsc.PushState(menuState);
   }

   public void ChangeToMapGenState() {
      var mapGenState = new MapGenState();
      _gsc.PushState(mapGenState);
   }

   public void ChangeToPlayState() {
      var playState = new PlayState();
      _gsc.PushState(playState);
   }

   [OnReady]
   private void CreateMonitorTimer() {
      if (LogLevel > Logger.LogLevel.Debug) return;

      var timer = new Timer {WaitTime = 1f, Autostart = true};
      timer.Connect("timeout", this, nameof(CheckMonitors));
      AddChild(timer);

      Logger.Warn("ObjectCount Monitor logging is switched on.");
   }

   private void CheckMonitors() {
      _lastObjects = _totalObjects;
      _totalObjects = Performance.GetMonitor(Performance.Monitor.ObjectCount);

      var delta = _totalObjects - _lastObjects;

      Logger.Debug($"{_totalObjects}, {delta}");
   }

   [OnReady]
   private void CreateManualGcTimer() {
      if (LogLevel > Logger.LogLevel.Debug) return;

      var timer = new Timer {WaitTime = 3f, Autostart = true};
      timer.Connect("timeout", this, nameof(ClearGc));
      AddChild(timer);

      Logger.Warn("Manual GC is switched on.");
   }

   private void ClearGc() {
      GC.Collect();
      GC.WaitForPendingFinalizers();
   }
}