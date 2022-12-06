using Godot;
namespace SatiRogue.Ecs.Dungeon.Nodes;

public partial class DirectionIndicator : AnimatedSprite3D {
   Vector2 _direction;
   public Vector2 Direction {
      get => _direction;
      set {
         _direction = value;
         Position = new Vector3(_direction.x, 0.01f, _direction.y);
      }
   }
}