using System;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Debug;
using SatiRogue.Entities;
using SatiRogue.Turn;

namespace SatiRogue.Components;

public partial class InputHandlerComponent : Component {
   private bool _awaitingInput;
   private MovementDirection? _forcedInput;
   public static bool InputEnabled = false;

   public override void HandleTurn() {
      if (EcOwner == null) throw new Exception("InputHandlerComponent couldn't doesn't have owner");
      switch (((Entity) EcOwner).TurnHandler.Turn) {
         case Turn.Turn.PlayerTurn:
            HandlePlayerTurnStarted();
            break;
         case Turn.Turn.EnemyTurn:
            HandleEnemyTurnStarted();
            break;
         case Turn.Turn.Processing:
            break;
         default:
            throw new ArgumentOutOfRangeException();
      }
   }

   private async void HandlePlayerTurnStarted() {
      Logger.Debug("Player turn started. Awaiting input.");
      _awaitingInput = true;
   }

   private void HandleEnemyTurnStarted() {
      _awaitingInput = false;
   }

   public void ForceInput(MovementDirection dir) {
      _forcedInput = dir;
   }

   public override void _Process(float delta) {
      if (!InputEnabled || !_awaitingInput || EntityRegistry.Player == null) return;
      
      var movementDirection = _forcedInput ?? MovementDirection.None;

      if (Input.IsActionPressed("move_left"))
         movementDirection = MovementDirection.Left;
      else if (Input.IsActionPressed("move_right"))
         movementDirection = MovementDirection.Right;
      else if (Input.IsActionPressed("move_down"))
         movementDirection = MovementDirection.Down;
      else if (Input.IsActionPressed("move_up")) movementDirection = MovementDirection.Up;

      if (movementDirection == MovementDirection.None) return;
      _forcedInput = null;
      _awaitingInput = false;

      EntityRegistry.Player.GetComponent<PlayerMovementComponent>()?.SetDestination(movementDirection);
   }
}