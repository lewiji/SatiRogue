using Godot;

namespace RoguelikeMono.Camera;

public class TilemapCamera : Camera2D
{
    public override void _Ready()
    {
        
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventMouseButton {Pressed: true} inputEventMouseButton) return;
        switch (inputEventMouseButton.ButtonIndex)
        {
            case (int) ButtonList.WheelUp:
                GD.Print(@event);
                Zoom -= new Vector2(0.1f, 0.1f);
                break;
            case (int) ButtonList.WheelDown:
                GD.Print(@event);
                Zoom += new Vector2(0.1f, 0.1f);
                break;
        }
    }
}