using Godot;
using System;
using Godot.Collections;
using GodotOnReady.Attributes;
using SatiRogue;
using SatiRogue.Components;
using SatiRogue.Entities;
using SatiRogue.Turn;
using Array = Godot.Collections.Array;

public partial class DirArrows : GridContainer {
   [OnReadyGet] private ToolButton Up;
   [OnReadyGet] private ToolButton Left;
   [OnReadyGet] private ToolButton Right;
   [OnReadyGet] private ToolButton Down;
   private MovementDirection _movementDirection = MovementDirection.None;

   [OnReady]
   private void ConnectTurns() {
      Systems.TurnHandler.Connect(nameof(TurnHandler.OnPlayerTurnStarted), this, nameof(OnTurn));
   }

   [OnReady]
   private void ConnectSignals() {
      Up.Connect("button_down", this, nameof(OnPressed), new Array{MovementDirection.Up});
      Left.Connect("button_down", this, nameof(OnPressed), new Array{MovementDirection.Left});
      Right.Connect("button_down", this, nameof(OnPressed), new Array{MovementDirection.Right});
      Down.Connect("button_down", this, nameof(OnPressed), new Array{MovementDirection.Down});
      
      Up.Connect("button_up", this, nameof(OnReleased), new Array{MovementDirection.Up});
      Left.Connect("button_up", this, nameof(OnReleased), new Array{MovementDirection.Left});
      Right.Connect("button_up", this, nameof(OnReleased), new Array{MovementDirection.Right});
      Down.Connect("button_up", this, nameof(OnReleased), new Array{MovementDirection.Down});
   }

   private void OnPressed(MovementDirection movementDirection) {
      GD.Print("yyy");
      _movementDirection = movementDirection;
      if (Systems.TurnHandler.Turn == Turn.PlayerTurn) {
         OnTurn();
      }
   }

   private void OnReleased(MovementDirection movementDirection) {
      _movementDirection = MovementDirection.None;
   }

   private void OnTurn() {
      if (_movementDirection != MovementDirection.None) {
         GD.Print("It's turntime");
         EntityRegistry.Player?.GetComponent<InputHandlerComponent>().ForceInput(_movementDirection);
      }
   }
}
