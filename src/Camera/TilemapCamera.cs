using Godot;

namespace SatiRogue.Camera;

public class TilemapCamera : Camera2D {
    public override void _Ready() { }

    public override void _Input(InputEvent @event) {
        if (@event is not InputEventMouseButton {Pressed: true} inputEventMouseButton) return;
        switch (inputEventMouseButton.ButtonIndex) {
            case (int) ButtonList.WheelUp:
                Zoom -= new Vector2(0.1f, 0.1f);
                Zoom = Zoom.Abs();
                break;
            case (int) ButtonList.WheelDown:
                Zoom += new Vector2(0.1f, 0.1f);
                break;
        }
    }
}