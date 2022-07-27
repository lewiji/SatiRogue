using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Play.Systems;
using RelEcs;

namespace SatiRogue.Ecs; 

public class PlayState : GameState {
   private GameStateController? _gsc;
   public readonly SystemGroup OnTurnSystems = new();
   public override void Init(GameStateController gameStates) {
      _gsc = gameStates;
      _gsc.World.AddElement(this as PlayState);
      
      InitSystems
         .Add(new SpatialMapSystem())
         .Add(new TurnHandlerInitSystem())
         .Add(new SpawnPlayerSystem())
         .Add(new SpawnEnemySystem())
         .Add(new SetInitialPositionSystem());
      
      OnTurnSystems
         .Add(new EnemyBehaviourSystem())
         .Add(new MovementSystem());

      PhysicsSystems
         .Add(new InterpolateWalkAnimationSystem())
         .Add(new TurnHandlerSystem())
         .Add(new InputSystem());
   }
}