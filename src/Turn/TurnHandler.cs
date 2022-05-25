using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Commands;
using SatiRogue.Components;
using SatiRogue.Debug;

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

   public Turn Turn {
      private set {
         _turn = value;
         switch (_turn) {
            case Turn.PlayerTurn:
               EmitSignal(nameof(OnPlayerTurnStarted));
               break;
            case Turn.EnemyTurn:
               EmitSignal(nameof(OnEnemyTurnStarted));
               CallDeferred(nameof(ProcessTurn));
               break;
            case Turn.Processing:
               Logger.Info("TurnHandler: Processing commands...");
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }
      get => _turn;
   }

   [OnReady]
   private async void SetFirstTurn() {
      await ToSignal(GetTree(), "idle_frame");
      Turn = Turn.PlayerTurn;
   }

   public void SetPlayerCommand(Command command) {
      if (Turn != Turn.PlayerTurn)
         throw new Exception("TurnHandler: Tried to SetPlayerCommand, but Turn is not PlayerTurn.");
      _playerCommands.Enqueue(command);
      Turn = Turn.EnemyTurn;
   }

   public void AddEnemyCommand(Command enemyCommand) {
      if (Turn != Turn.EnemyTurn)
         throw new Exception("TurnHandler: Tried to AddEnemyCommand, but Turn is not EnemyTurn.");
      _enemyCommands.Enqueue(enemyCommand);
   }

   private void ProcessTurn() {
      if (Turn == Turn.Processing)
         throw new Exception("TurnHandler: ProcessTurn was called, but Turn is already Processing.");
      Turn = Turn.Processing;

      while (_playerCommands.Any()) _playerCommands.Dequeue().Execute();

      while (_enemyCommands.Any()) _enemyCommands.Dequeue().Execute();

      if (MovementComponent._recordingPathfindingCalls) GD.Print($"{MovementComponent.numPathingCallsThisTurn} FindPath calls this turn");

      CallDeferred(nameof(StartNewTurn));
   }

   private void StartNewTurn() {
      Logger.Info("TurnHandler: Processing finished, starting new turn.");
      Turn = Turn.PlayerTurn;
   }
}