using Godot;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Play.Systems;
using RelEcs;
using SatiRogue.Ecs.Play.Systems.Init;
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
         .Add(new SetInitialPositionSystem())
         .Add(new CharacterHealthBarSystem());

      OnTurnSystems
         .Add(new PlayerMovementSystem())
         .Add(new EnemyBehaviourSystem())
         .Add(new EnemyMovementSystem())
         .Add(new CharacterAnimationSystem())
         .Add(new ResetInputDirectionSystem());

      PhysicsSystems
         .Add(new InterpolateWalkAnimationSystem())
         .Add(new InputSystem())
         .Add(new TurnHandlerSystem())
         .Add(new HealthSystem());
   }
}