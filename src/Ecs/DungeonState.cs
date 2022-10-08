using RelEcs;
using World = RelEcs.World;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Dungeon.Nodes;
using SatiRogue.Ecs.Dungeon.Systems;
using SatiRogue.Ecs.Dungeon.Systems.Exit;
using SatiRogue.Ecs.Dungeon.Systems.Init;
using SatiRogue.resources;
using SatiRogue.Tools;

namespace SatiRogue.Ecs;

public class DungeonState : GameState {
   public SatiSystemGroup? OnTurnSystems;
   GameStateController? _gsc;

   public override void Init(GameStateController gameStates) {
      CreateSystems(gameStates);
      var timer = GetTree().CreateTimer(1f);
      timer.Connect("timeout", this, nameof(PrintOrphans));
   }

   void CreateSystems(GameStateController gameStates) {
      _gsc = gameStates;
      _gsc.World.AddOrReplaceElement(this);

      OnTurnSystems = new SatiSystemGroup(_gsc);

      var entitiesNode = new Entities();
      _gsc.World.AddOrReplaceElement(entitiesNode);
      AddChild(entitiesNode);

      var mapGeomNode = new MapGeometry();
      _gsc.World.AddOrReplaceElement(mapGeomNode);
      AddChild(mapGeomNode);

      InitSystems.Add(new SpatialMapSystem())
         .Add(new SpawnHudSystem())
         .Add(new DebugToolsSystem())
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
         .Add(new InventorySystem());

      var playerMovementSystem = new PlayerMovementSystem();
      _gsc.World.AddOrReplaceElement(playerMovementSystem);

      OnTurnSystems.Add(playerMovementSystem)
         .Add(new PlayerShootSystem())
         .Add(new EnemyBehaviourSystem())
         .Add(new CharacterMovementSystem())
         .Add(new FogSystem())
         .Add(new ResetInputDirectionSystem())
         .Add(new HealthSystem())
         .Add(new PersistInventorySystem());

      var turnHandlerSystem = new TurnHandlerSystem(OnTurnSystems);
      var inputSystem = new InputSystem();
      _gsc.World.AddOrReplaceElement(turnHandlerSystem);
      _gsc.World.AddOrReplaceElement(inputSystem);

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

   public DungeonState(GameStateController gameStateController) : base(gameStateController) { }
}