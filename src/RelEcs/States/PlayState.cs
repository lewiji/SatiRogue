using System;
using SatiRogue.RelEcs.Systems;

namespace SatiRogue.RelEcs.States; 

public class PlayState : GameState {
   private GameStateController? _gsc;
   private TurnHandlerSystem? _turnHandler;
   public override void Init(GameStateController gameStates) {
      _gsc = gameStates;
      
      InitSystems
         .Add(new SpatialMapSystem())
         .Add(new SpawnPlayerSystem());
      
      OnTurnSystems
         .Add(new MovementSystem());

      _turnHandler = new TurnHandlerSystem();
      ProcessSystems
         .Add(_turnHandler)
         .Add(new InputSystem());

      PhysicsSystems
         .Add(new InterpolateWalkAnimationSystem());
   }

   public override void _Process(float delta) {
      if (!InitSystems.HasRun) return;
      switch (_turnHandler?.CurrentTurn) {
         case TurnType.Processing:
            break;
         case TurnType.PlayerTurn:
            OnTurnSystems.Run(_gsc.World);
            _turnHandler.CurrentTurn = TurnType.Processing;
            break;
         case TurnType.EnemyTurn:
            OnTurnSystems.Run(_gsc.World);
            _turnHandler.CurrentTurn = TurnType.Processing;
            break;
         default:
            throw new ArgumentOutOfRangeException();
      }
   }
}