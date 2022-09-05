using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Play.Systems;
using SatiRogue.Ecs.Play.Systems.Init;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs;

public class PlayState : GameState {
   public readonly SystemGroup OnTurnSystems = new();
   GameStateController? _gsc;

   public override void Init(GameStateController gameStates) {
      CreateSystems(gameStates);
   }

   void CreateSystems(GameStateController gameStates) {
      _gsc = gameStates;
      _gsc.World.AddElement(this);

      InitSystems.Add(new SpatialMapSystem())
         .Add(new SetupAudioSystem())
         .Add(new TurnHandlerInitSystem())
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
         .Add(new HealthSystem());

      var turnHandlerSystem = new TurnHandlerSystem();
      var inputSystem = new InputSystem();

      PhysicsSystems.Add(new InterpolateWalkAnimationSystem())
         .Add(inputSystem)
         .Add(turnHandlerSystem)
         .Add(new ProjectileSystem())
         .Add(new CharacterAnimationSystem())
         .Add(new PlayerIndicatorSystem())
         .Add(new AudioSystem())
         .Add(new CharacterDeathSystem());

      _gsc.World.AddElement(turnHandlerSystem);
      _gsc.World.AddElement(inputSystem);
   }
}