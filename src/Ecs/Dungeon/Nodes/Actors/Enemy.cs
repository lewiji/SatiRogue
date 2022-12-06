using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Systems.Init;
using SatiRogue.Resources;

namespace SatiRogue.Ecs.Dungeon.Nodes.Actors;

public partial class Enemy : Character {
   public SpriteFrames? Frames;
   public Stats Stats = new(EnemyData.DefaultEnemyLevel);

   HoverStats _hoverStats = null!;
   bool _hovering = false;
   Camera3D? _playerCamera;


   //[OnReadyGet("Area3D")]
   //Area3D _area = null!;
   public EnemyResource? EnemyResource;

   public override void OnSpawn(EntityBuilder entityBuilder) {
      base.OnSpawn(entityBuilder);

      //Material = EnemyRecord?.GraphicsSet.Material;
      Frames = EnemyResource?.SpriteFrames;

      if (EnemyResource?.Name != null)
         CharacterName = EnemyResource.Name;

      entityBuilder.Add(Stats)
         .Add(new HealthComponent(Stats.Record.Health))
         .Add(new GridPositionComponent())
         .Add(new CharacterAnimationComponent())
         .Add(new InputDirectionComponent())
         .Add(new BehaviourTree())
         .Add(new Walkable())
         .Add<Alive>();
   }

   public override void _Ready()
   {
			_hoverStats = GetNode<HoverStats>("HoverStats");
			CallDeferred(nameof(SetupSceneResources));
   }

   void SetupSceneResources() {
      _playerCamera = GetTree().Root.GetCamera3d();

      _hoverStats.Visible = _hovering;
      _hoverStats.SetStatsRecord(Stats.Record);
      //_area.Connect("mouse_entered",new Callable(this,nameof(OnMouseEntered)));
      //_area.Connect("mouse_exited",new Callable(this,nameof(OnMouseExited)));

      if (AnimatedSprite3D == null)
         return;
      AnimatedSprite3D.Frames = Frames;
   }

   void OnMouseEntered() {
      _hovering = true;
      _hoverStats.Visible = true;
   }

   void OnMouseExited() {
      _hovering = false;
      _hoverStats.Visible = false;
   }

   public override void _PhysicsProcess(double delta) {
      if (_hovering && _playerCamera != null) {
         _hoverStats.GlobalPosition = _playerCamera.UnprojectPosition(GlobalPosition)
                                          - new Vector2(_hoverStats.Size.x / 2f, _hoverStats.Size.y * 1.32f);
      }
   }
}