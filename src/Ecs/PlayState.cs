using RelEcs;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Play.Systems;
using SatiRogue.Ecs.Play.Systems.Init;
namespace SatiRogue.Ecs;

public class PlayState : GameState {
   public readonly SystemGroup OnTurnSystems = new();
   private GameStateController? _gsc;

   public override void Init(GameStateController gameStates) {
      _gsc = gameStates;
      _gsc.World.AddElement(this);

      InitSystems.Add(new SpatialMapSystem())
         .Add(new TurnHandlerInitSystem())
         .Add(new SpawnPlayerSystem())
         .Add(new SpawnEnemySystem())
         .Add(new SetInitialPositionSystem())
         .Add(new CharacterHealthBarSystem());

      OnTurnSystems.Add(new PlayerMovementSystem())
         .Add(new EnemyBehaviourSystem())
         .Add(new CharacterMovementSystem())
         .Add(new ResetInputDirectionSystem())
         .Add(new HealthSystem());

      PhysicsSystems.Add(new InterpolateWalkAnimationSystem())
         .Add(new InputSystem())
         .Add(new TurnHandlerSystem())
         .Add(new CharacterAnimationSystem())
         .Add(new CharacterDeathSystem());
   }
}