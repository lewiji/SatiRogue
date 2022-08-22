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
   private void CreateMonitorTimer() {
      var timer = new Timer();
      timer.WaitTime = 1f;
      AddChild(timer);
      timer.Connect("timeout", this, nameof(CheckMonitors));
      timer.Start();
   }

   private void CheckMonitors() {
      _lastObjects = _totalObjects;
      _totalObjects = Performance.GetMonitor(Performance.Monitor.ObjectCount);

      var delta = _totalObjects - _lastObjects;

      GD.Print(delta);
   }

   [OnReady]
   private void CreateManualGcTimer() {
      var timer = new Timer();
      timer.WaitTime = 3f;
      AddChild(timer);
      timer.Connect("timeout", this, nameof(ClearGc));
      timer.Start();
   }

   private void ClearGc() {
      GC.Collect(GC.MaxGeneration);
      GC.WaitForPendingFinalizers();
      GC.Collect();

      PrintStrayNodes();
   }

   [OnReady]
   private void AddMapGenState() {
      var mapGenState = new MapGenState();
      _gsc.PushState(mapGenState);
   }

   public void OnMapGenInitFinished() {
      var playState = new PlayState();
      _gsc.PushState(playState);
   }
}