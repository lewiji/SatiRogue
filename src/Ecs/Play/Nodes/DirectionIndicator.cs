using Godot;

public class DirectionIndicator : AnimatedSprite3D {
   private Vector2 _direction;
   public Vector2 Direction {
      get => _direction;
      set {
         _direction = value;
         Translation = new Vector3(_direction.x, 0.01f, _direction.y);
      }
   }
}