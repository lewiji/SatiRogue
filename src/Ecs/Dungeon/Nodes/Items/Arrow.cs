using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
namespace SatiRogue.Ecs.Dungeon.Nodes.Items;

public partial class Arrow : GameObject {
   AnimatedSprite3D? _animatedSprite3D;
   Vector2 _direction;

   public Vector3 Destination;
   public int Range = 6;
   public Vector2 Direction {
      get => _direction;
      set {
         _direction = value;

         if (_animatedSprite3D == null) return;

         if (_direction == Vector2.Up) {
	         _animatedSprite3D.Rotation = new Vector3(Mathf.RadToDeg(-90), Mathf.RadToDeg(45), 0);
         } else if (_direction == Vector2.Right) {
            _animatedSprite3D.Rotation = new Vector3(Mathf.RadToDeg(-90), Mathf.RadToDeg(-45), 0);
         } else if (_direction == Vector2.Down) {
            _animatedSprite3D.Rotation = new Vector3(Mathf.RadToDeg(-90), Mathf.RadToDeg(-135), 0);
         } else if (_direction == Vector2.Left) {
            _animatedSprite3D.Rotation = new Vector3(Mathf.RadToDeg(-90), Mathf.RadToDeg(135), 0);
         }
      }
   }

   public override void _Ready()
   {
	   _animatedSprite3D = GetNode<AnimatedSprite3D>("AnimatedSprite3D");
   }

   public override void OnSpawn(EntityBuilder entityBuilder) {
      entityBuilder.Add<Firing>();
   }
}