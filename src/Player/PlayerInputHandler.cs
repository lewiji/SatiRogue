using Godot;
using SatiRogue.Commands.Actions;
using SatiRogue.Entities;
using SatiRogue.Turn;

namespace SatiRogue.Player;

public class PlayerInputHandler : Node {
    private Timer _moveTimer = new();
    public bool CanMove;
    private TurnHandler _turnHandler = Systems.TurnHandler;

    public override void _Ready() {
        AddChild(_moveTimer);
        _moveTimer.OneShot = true;

        _turnHandler.Connect(nameof(TurnHandler.OnPlayerTurnStarted), this, nameof(HandlePlayerTurnStarted));
        _turnHandler.Connect(nameof(TurnHandler.OnEnemyTurnStarted), this, nameof(HandleEnemyTurnStarted));
    }

    private async void HandlePlayerTurnStarted() {
        if (!_moveTimer.IsStopped()) {
            await ToSignal(_moveTimer, "timeout");
        }
        CanMove = true;
    }
    
    private void HandleEnemyTurnStarted() {
        CanMove = false;
    }

    public override void _Process(float delta) {
        if (!CanMove || EntityRegistry.Player == null) return;
        
        if (Input.IsActionPressed("move_left")) {
            _turnHandler.SetPlayerCommand(new Move(EntityRegistry.Player, MovementDirection.Left));
        } else if (Input.IsActionPressed("move_right")) {
            _turnHandler.SetPlayerCommand(new Move(EntityRegistry.Player, MovementDirection.Right));
        } else if (Input.IsActionPressed("move_down")) {
            _turnHandler.SetPlayerCommand(new Move(EntityRegistry.Player, MovementDirection.Down));
        } else if (Input.IsActionPressed("move_up")) {
            _turnHandler.SetPlayerCommand(new Move(EntityRegistry.Player, MovementDirection.Up));
        }

        _moveTimer?.Start(0.12f);
    }
}