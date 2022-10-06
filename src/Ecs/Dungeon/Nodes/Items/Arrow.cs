using Godot;
using GodotOnReady.Attributes;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
namespace SatiRogue.Ecs.Dungeon.Nodes.Items;

public partial class Arrow : GameObject {
   [OnReadyGet("AnimatedSprite3D")] AnimatedSprite3D? _animatedSprite3D;
   Vector2 _direction;

   public Vector3 Destination;
   public int Range = 6;
   public Vector2 Direction {
      get => _direction;
      set {
         _direction = value;

         if (_animatedSprite3D == null) return;

         if (_direction == Vector2.Up) {
            _animatedSprite3D.RotationDegrees = new Vector3(-90, 45, 0);
         } else if (_direction == Vector2.Right) {
            _animatedSprite3D.RotationDegrees = new Vector3(-90, -45, 0);
         } else if (_direction == Vector2.Down) {
            _animatedSprite3D.RotationDegrees = new Vector3(-90, -135, 0);
         } else if (_direction == Vector2.Left) {
            _animatedSprite3D.RotationDegrees = new Vector3(-90, 135, 0);
         }
      }
   }

   public override void OnSpawn(EntityBuilder entityBuilder) {
      entityBuilder.Add<Firing>();
   }
}