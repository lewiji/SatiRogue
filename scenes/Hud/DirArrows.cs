using Godot;
using System;
using Godot.Collections;
using GoDotNet;
using GodotOnReady.Attributes;
using SatiRogue;
using SatiRogue.Components;
using SatiRogue.Entities;
using SatiRogue.Turn;
using Array = Godot.Collections.Array;

public class DirArrows : GridContainer, IDependent  {
   /*([OnReadyGet] private ToolButton? Up;
   [OnReadyGet] private ToolButton? Left;
   [OnReadyGet] private ToolButton? Right;
   [OnReadyGet] private ToolButton? Down;*/
   private MovementDirection _movementDirection = MovementDirection.None;

   [Dependency] private TurnHandler _turnHandler => this.DependOn<TurnHandler>();

   public override void _Ready() {
      this.Depend();
   }

   public void Loaded() {
      _turnHandler.Connect(nameof(TurnHandler.OnPlayerTurnStarted), this, nameof(OnTurn));
      
      /*Up?.Connect("button_down", this, nameof(OnPressed), new Array{MovementDirection.Up});
      Left?.Connect("button_down", this, nameof(OnPressed), new Array{MovementDirection.Left});
      Right?.Connect("button_down", this, nameof(OnPressed), new Array{MovementDirection.Right});
      Down?.Connect("button_down", this, nameof(OnPressed), new Array{MovementDirection.Down});
      
      Up?.Connect("button_up", this, nameof(OnReleased), new Array{MovementDirection.Up});
      Left?.Connect("button_up", this, nameof(OnReleased), new Array{MovementDirection.Left});
      Right?.Connect("button_up", this, nameof(OnReleased), new Array{MovementDirection.Right});
      Down?.Connect("button_up", this, nameof(OnReleased), new Array{MovementDirection.Down});*/
   }
   
   private void OnPressed(MovementDirection movementDirection) {
      GD.Print("yyy");
      _movementDirection = movementDirection;
      if (_turnHandler.Turn == Turn.PlayerTurn) {
         OnTurn();
      }
   }

   private void OnReleased(MovementDirection movementDirection) {
      _movementDirection = MovementDirection.None;
   }

   private void OnTurn() {
      if (_movementDirection != MovementDirection.None) {
         GD.Print("It's turntime");
         EntityRegistry.Player?.GetComponent<InputHandlerComponent>()?.ForceInput(_movementDirection);
      }
   }
}
