using Godot;
namespace SatiRogue.Ecs.Dungeon.Components.Actor;

public partial class InputDirectionComponent {
   Vector2 _direction;
   public Vector2 Direction {
      get => _direction;
      set {
         if (_direction != Vector2.Zero) {
            LastDirection = _direction;
         }
         _direction = value;
      }
   }
   public Vector2 LastDirection { get; set; }
}