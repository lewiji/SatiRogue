using Godot;
using SatiRogue.Grid.Entities;

namespace SatiRogue;

public class PlayerInputHandler : Node {
    private Timer? _moveTimer;
    public bool CanMove;

    public override void _Ready() {
        _moveTimer = new Timer();
        AddChild(_moveTimer);
        _moveTimer.OneShot = true;
        _moveTimer.Connect("timeout", this, nameof(OnMoveTimerExpired));

        CanMove = true;
    }

    private void OnMoveTimerExpired() {
        CanMove = true;
    }

    public override void _Process(float delta) {
        if (CanMove && EntityRegistry.Player != null) {
            if (Input.IsActionPressed("move_left")) {
                EntityRegistry.Player.Move(MovementDirection.Left);
                CanMove = false;
            }

            if (Input.IsActionPressed("move_right")) {
                EntityRegistry.Player.Move(MovementDirection.Right);
                CanMove = false;
            }

            if (Input.IsActionPressed("move_down")) {
                EntityRegistry.Player.Move(MovementDirection.Down);
                CanMove = false;
            }

            if (Input.IsActionPressed("move_up")) {
                EntityRegistry.Player.Move(MovementDirection.Up);
                CanMove = false;
            }

            if (!CanMove) {
                _moveTimer?.Start(0.12f);
            }
        }
    }
}