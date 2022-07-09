using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Commands;
using SatiRogue.Components;
using SatiRogue.Debug;
using Action = SatiRogue.Commands.Action;

namespace SatiRogue.Turn;

public enum Turn {
   PlayerTurn,
   EnemyTurn,
   Processing
}

public partial class TurnHandler : Node {
   [Signal]
   public delegate void OnEnemyTurnStarted();

   [Signal]
   public delegate void OnPlayerTurnStarted();

   private readonly Queue<Command> _enemyCommands = new();
   private readonly Queue<Command> _playerCommands = new();

   private Turn _turn;
   private readonly Timer _turnTimer = new() {OneShot = true, WaitTime = 0.11f};
   private int _turnNumber = 0;

   public Turn Turn {
      private set {
         _turn = value;
         switch (_turn) {
            case Turn.PlayerTurn:
               _turnNumber += 1;
               Logger.Info("========");
               Logger.Info($"TurnHandler: Start of Turn {_turnNumber}.");
               EmitSignal(nameof(OnPlayerTurnStarted));
               break;
            case Turn.EnemyTurn:
               Logger.Info("Player turn processed. Enemy turn started.");
               EmitSignal(nameof(OnEnemyTurnStarted));
               CallDeferred(nameof(ProcessTurn));
               break;
            case Turn.Processing:
               Logger.Debug("TurnHandler: Processing commands...");
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }
      get => _turn;
   }

   [OnReady]
   private async void SetFirstTurn() {
      AddChild(_turnTimer);
      await ToSignal(GetTree().CreateTimer(0.25f), "timeout");
      Turn = Turn.PlayerTurn;
   }

   public void SetPlayerCommand(Command command) {
      if (Turn != Turn.PlayerTurn)
         throw new Exception("TurnHandler: Tried to SetPlayerCommand, but Turn is not PlayerTurn.");
      //_playerCommands.Enqueue(command);
      Logger.Info("TurnHandler: Player command set.");
      command.Execute();
      Turn = Turn.EnemyTurn;
   }

   public void AddEnemyCommand(Command enemyCommand) {
      if (Turn != Turn.EnemyTurn)
         throw new Exception("TurnHandler: Tried to AddEnemyCommand, but Turn is not EnemyTurn.");
      _enemyCommands.Enqueue(enemyCommand);
   }

   public void SetTurnSpeed(float timeSeconds)
   {
      _turnTimer.WaitTime = timeSeconds;
   }

   private void ProcessTurn() {
      if (Turn == Turn.Processing)
         throw new Exception("TurnHandler: ProcessTurn was called, but Turn is already Processing.");
      Turn = Turn.Processing;
      
      _turnTimer.Start();

      while (_playerCommands.Any() || _enemyCommands.Any()) {
         while (_playerCommands.Any()) _playerCommands.Dequeue().Execute();
         if (_enemyCommands.Count > 0) {
            var enemyCommand = _enemyCommands.Dequeue();
            if (enemyCommand is Action enemyAction && enemyAction.IsOwnerEnabled()) {
               enemyAction.Execute();
            }
         }
      }

      if (MovementComponent._recordingPathfindingCalls) Logger.Info($"{MovementComponent.numPathingCallsThisTurn} FindPath calls this turn");

      CallDeferred(nameof(StartNewTurn));
   }

   private async void StartNewTurn()
   {
      if (!_turnTimer.IsStopped())
         await ToSignal(_turnTimer, "timeout");
      Logger.Debug("TurnHandler: Processing finished, starting new turn.");
      Turn = Turn.PlayerTurn;
   }
}