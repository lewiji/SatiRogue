using Godot;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Play.Systems;
using RelEcs;
using SatiRogue.Grid;
using SatiRogue.MathUtils;

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
         .Add(new HealthSystem())
         .Add(new MovementSystem());

      PhysicsSystems
         .Add(new InterpolateWalkAnimationSystem())
         .Add(new InputSystem())
         .Add(new TurnHandlerSystem());
   }
}