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
   public SatiSystemGroup? OnPlayerTurnSystems;
   public SatiSystemGroup? OnNpcTurnSystems;
   public SatiSystemGroup? OnTurnEndSystems;
   GameStateController? _gsc;

   public override void Init(GameStateController gameStates) {
      CreateSystems(gameStates);
      var timer = GetTree().CreateTimer(1f);
      timer.Connect("timeout", this, nameof(PrintOrphans));
   }

   void CreateSystems(GameStateController gameStates) {
      _gsc = gameStates;
      _gsc.World.AddOrReplaceElement(this);

      OnPlayerTurnSystems = new SatiSystemGroup(_gsc);
      OnNpcTurnSystems = new SatiSystemGroup(_gsc);
      OnTurnEndSystems = new SatiSystemGroup(_gsc);

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
         .Add(new InventorySystem())
         .Add(new InitLightingSystem());

      var playerMovementSystem = new PlayerMovementSystem();
      _gsc.World.AddOrReplaceElement(playerMovementSystem);

      OnPlayerTurnSystems
         .Add(playerMovementSystem)
         .Add(new PlayerShootSystem())
         .Add(new FogSystem());
      
      OnNpcTurnSystems
         .Add(new EnemyBehaviourSystem())
         .Add(new CharacterMovementSystem());
      
      OnTurnEndSystems
         .Add(new OpenContainersSystem())
         .Add(new AttackSystem())
         .Add(new ResetInputDirectionSystem())
         .Add(new HealthSystem())
         .Add(new PersistInventorySystem());

      var turnHandlerSystem = new TurnHandlerSystem();
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

      turnHandlerSystem.Connect(nameof(TurnHandlerSystem.ExecutePlayerTurn), OnPlayerTurnSystems, nameof(SatiSystemGroup.Run));
      turnHandlerSystem.Connect(nameof(TurnHandlerSystem.ExecuteNpcTurn), OnNpcTurnSystems, nameof(SatiSystemGroup.Run));
      turnHandlerSystem.Connect(nameof(TurnHandlerSystem.ExecuteTurnEnd), OnTurnEndSystems, nameof(SatiSystemGroup.Run));

      ExitSystems.Add(new CleanupDungeonSystem());
   }

   void ExecutePlayerTurnSystems() {
      OnPlayerTurnSystems.Run();
   }

   void PrintOrphans() {
      PrintStrayNodes();
   }

   public DungeonState(GameStateController gameStateController) : base(gameStateController) { }
}