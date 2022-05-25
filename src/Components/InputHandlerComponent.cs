using System.Collections.Generic;
using Godot;
using SatiRogue.Components;
using SatiRogue.Debug;
using SatiRogue.Entities;
using SatiRogue.Turn;

namespace SatiRogue.Player;

public class InputHandlerComponent : Component {
   private readonly Timer _moveTimer = new();
   private readonly TurnHandler _turnHandler = Systems.TurnHandler;
   public bool CanMove;

   protected override List<Turn.Turn> TurnTypesToExecuteOn { get; set; } = new() {Turn.Turn.PlayerTurn, Turn.Turn.EnemyTurn};

   public override void _Ready() {
      AddChild(_moveTimer);
      _moveTimer.OneShot = true;
   }

   public override void HandleTurn() {
      switch (Systems.TurnHandler.Turn) {
         case Turn.Turn.PlayerTurn:
            HandlePlayerTurnStarted();
            break;
         case Turn.Turn.EnemyTurn:
            HandleEnemyTurnStarted();
            break;
      }
   }

   private async void HandlePlayerTurnStarted() {
      if (!_moveTimer.IsStopped()) await ToSignal(_moveTimer, "timeout");
      Logger.Info("Player turn started. Awaiting input.");
      CanMove = true;
   }

   private void HandleEnemyTurnStarted() {
      Logger.Info("Received player input. Player turn ended. Enemy turn started.");
      CanMove = false;
   }

   public override void _Process(float delta) {
      if (!CanMove || EntityRegistry.Player == null) return;

      var movementDirection = MovementDirection.None;

      if (Input.IsActionPressed("move_left"))
         movementDirection = MovementDirection.Left;
      else if (Input.IsActionPressed("move_right"))
         movementDirection = MovementDirection.Right;
      else if (Input.IsActionPressed("move_down"))
         movementDirection = MovementDirection.Down;
      else if (Input.IsActionPressed("move_up")) movementDirection = MovementDirection.Up;

      if (movementDirection == MovementDirection.None) return;

      EntityRegistry.Player.GetComponent<PlayerMovementComponent>()?.SetDestination(movementDirection);

      _moveTimer?.Start(0.12f);
   }
}