using RelEcs;
using World = RelEcs.World;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Play.Nodes;
using SatiRogue.Ecs.Play.Systems;
using SatiRogue.Ecs.Play.Systems.Exit;
using SatiRogue.Ecs.Play.Systems.Init;

namespace SatiRogue.Ecs;

public class DungeonState : GameState {
   public readonly SystemGroup OnTurnSystems = new();
   GameStateController? _gsc;

   public override void Init(GameStateController gameStates) {
      CreateSystems(gameStates);
      var timer = GetTree().CreateTimer(1f);
      timer.Connect("timeout", this, nameof(PrintOrphans));
   }

   void CreateSystems(GameStateController gameStates) {
      _gsc = gameStates;
      _gsc.World.AddElement(this);

      var entitiesNode = new Entities();
      _gsc.World.AddElement(entitiesNode);
      AddChild(entitiesNode);

      var mapGeomNode = new MapGeometry();
      _gsc.World.AddElement(mapGeomNode);
      AddChild(mapGeomNode);

      InitSystems.Add(new SpatialMapSystem())
         .Add(new SetupAudioSystem())
         .Add(new TurnHandlerInitSystem())
         .Add(new PlaceStairs())
         .Add(new SpawnPlayerSystem())
         .Add(new SpawnEnemySystem())
         .Add(new SpawnItemsSystem())
         .Add(new SetInitialPositionSystem())
         .Add(new InitFogSystem())
         .Add(new FogSystem())
         .Add(new CharacterHealthBarSystem())
         .Add(new SpawnHudSystem())
         .Add(new InventorySystem());

      OnTurnSystems.Add(new PlayerMovementSystem())
         .Add(new PlayerShootSystem())
         .Add(new EnemyBehaviourSystem())
         .Add(new CharacterMovementSystem())
         .Add(new FogSystem())
         .Add(new ResetInputDirectionSystem())
         .Add(new HealthSystem())
         .Add(new PersistInventorySystem());

      var turnHandlerSystem = new TurnHandlerSystem();
      var inputSystem = new InputSystem();
      _gsc.World.AddElement(turnHandlerSystem);
      _gsc.World.AddElement(inputSystem);

      PhysicsSystems.Add(new InterpolateWalkAnimationSystem())
         .Add(inputSystem)
         .Add(turnHandlerSystem)
         .Add(new ProjectileSystem())
         .Add(new CharacterAnimationSystem())
         .Add(new PlayerIndicatorSystem())
         .Add(new AudioSystem())
         .Add(new CharacterDeathSystem())
         .Add(new LevelChangeSystem());

      ExitSystems.Add(new CleanupDungeonSystem());
   }

   void PrintOrphans() {
      PrintStrayNodes();
   }
}