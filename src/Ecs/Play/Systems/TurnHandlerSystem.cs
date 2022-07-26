using System;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.MapGenerator.Triggers;
using SatiRogue.Ecs.Play.Components;
using RelEcs;

namespace SatiRogue.Ecs.Play.Systems; 

public class TurnHandlerSystem : GDSystem {
   private bool _hasRun;
   private Entity _turnHandlerEntity = null!;
   private float _minTurnTime = 0.1f;
   
   private void SetCurrentTurn(TurnType turnType) {
      var turn = GetComponent<Components.Turn>(_turnHandlerEntity);
      turn.CurrentTurn = turnType;
      Send(new TurnChangedTrigger(turn.CurrentTurn));

      if (turn.CurrentTurn != TurnType.Processing) return;
      // Process turn by running OnTurnSystems
      GetElement<PlayState>().OnTurnSystems.Run(World);
   }

   public override void Ready() {
      
      _turnHandlerEntity = Spawn()
         .Add(new Components.Turn())
         .Id();
      CallDeferred(nameof(SetCurrentTurn), (int)TurnType.PlayerTurn);
   }

   public override void Run() {
      ProcessTurnChanges();
      HandlePlayerInputTrigger();
   }

   private void HandlePlayerInputTrigger() { // Wait for player input to move to EnemyTurn
      foreach (var _ in Receive<PlayerHasMadeInputTrigger>()) {
         var turn = GetComponent<Components.Turn>(_turnHandlerEntity);
         if (turn.CurrentTurn == TurnType.PlayerTurn) {
            SetCurrentTurn(TurnType.EnemyTurn);
         }
      }
   }

   private void ProcessTurnChanges() { // Progress turn phases on changed
      foreach (var turnTrigger in Receive<TurnChangedTrigger>()) {
         switch (turnTrigger.Turn) {
            case TurnType.Processing:
               ResetTurn();
               break;
            case TurnType.EnemyTurn:
               //TODO: Enemy processing
               SetCurrentTurn(TurnType.Processing);
               break;
            case TurnType.PlayerTurn:
               // Handled by player input trigger
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }
   }

   private async void ResetTurn() {
      await ToSignal(GetElement<SceneTree>().CreateTimer(_minTurnTime), "timeout");
      SetCurrentTurn(TurnType.PlayerTurn);
   }
}