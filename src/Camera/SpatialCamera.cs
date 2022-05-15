using Godot;

namespace SatiRogue.Camera;

public class SpatialCamera : Godot.Camera {
   public override void _Input(InputEvent @event) {
      if (@event is not InputEventMouseButton {Pressed: true} inputEventMouseButton) return;
      switch (inputEventMouseButton.ButtonIndex) {
         case (int) ButtonList.WheelUp:
            Fov -= 1f;
            break;
         case (int) ButtonList.WheelDown:
            Fov += 1f;
            break;
      }
   }
}