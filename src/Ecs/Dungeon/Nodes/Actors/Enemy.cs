using Godot;
using GodotOnReady.Attributes;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Systems.Init;

namespace SatiRogue.Ecs.Dungeon.Nodes.Actors;

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
   public SpawnEnemySystem.EnemyRecord? EnemyRecord;

   public override void OnSpawn(EntityBuilder entityBuilder) {
      base.OnSpawn(entityBuilder);

      Material = EnemyRecord?.GraphicsSet.Material;
      Frames = EnemyRecord?.GraphicsSet.Frames;

      if (EnemyRecord?.Name != null)
         CharacterName = EnemyRecord.Name;

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
      CallDeferred(nameof(SetupSceneResources));
   }

   void SetupSceneResources() {
      _playerCamera = GetTree().Root.GetCamera();

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