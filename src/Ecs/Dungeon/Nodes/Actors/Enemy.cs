using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Nodes;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Nodes.Actors;

public partial class Enemy : Character {
   public SpriteFrames? Frames;
   public Material? Material;
   public Stats Stats = new(Stats.DefaultEnemyClass);

   [OnReadyGet("HoverStats")]
   HoverStats _hoverStats = null!;
   bool _hovering = false;
   Camera? _playerCamera;

   [OnReadyGet("Area")]
   Area _area = null!;

   public override void OnSpawn(EntityBuilder entityBuilder) {
      base.OnSpawn(entityBuilder);

      entityBuilder.Add(Stats)
         .Add(new HealthComponent(Stats.Record.Health))
         .Add(new GridPositionComponent())
         .Add(new InputDirectionComponent())
         .Add(new BehaviourTree())
         .Add(new Walkable())
         .Add<Alive>();
   }

   [OnReady]
   public void OnEnemyReady() {
      _playerCamera = GetTree().Root.GetCamera();
      GD.Print("Enemy ready");

      _hoverStats.Visible = _hovering;
      _hoverStats.SetStatsRecord(Stats.Record);
      _area.Connect("mouse_entered", this, nameof(OnMouseEntered));
      _area.Connect("mouse_exited", this, nameof(OnMouseExited));

      if (AnimatedSprite3D == null)
         return;
      AnimatedSprite3D.Frames = Frames;
      AnimatedSprite3D.MaterialOverride = Material;
   }

   void OnMouseEntered() {
      _hovering = true;
      _hoverStats.Visible = true;
   }

   void OnMouseExited() {
      _hovering = false;
      _hoverStats.Visible = false;
   }

   public override void _PhysicsProcess(float delta) {
      if (_hovering && _playerCamera != null) {
         _hoverStats.RectGlobalPosition = _playerCamera.UnprojectPosition(GlobalTranslation)
                                          - new Vector2(_hoverStats.RectSize.x / 2f, _hoverStats.RectSize.y * 1.32f);
      }
   }
}