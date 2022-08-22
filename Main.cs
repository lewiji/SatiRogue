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