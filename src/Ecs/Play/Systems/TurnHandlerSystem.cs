using System;
using Godot;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Triggers;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems;

public class TurnHandlerSystem : GdSystem {
   readonly float _minTurnTime = 0.1f;
   public int TurnNumber { get; private set; }

   void SetCurrentTurn(TurnType turnType) {
      var turn = GetElement<Turn>();
      turn.CurrentTurn = turnType;
      Send(new TurnChangedTrigger(turn.CurrentTurn));

      if (turn.CurrentTurn != TurnType.Processing) return;
      // Process turn by running OnTurnSystems
      TickWorld();
      TurnNumber += 1;
   }

   void TickWorld() {
      GetElement<PlayState>().OnTurnSystems.Run(World);
   }

   public override void Run() {
      ProcessTurnChanges();
      HandlePlayerInputTrigger();
   }

   void HandlePlayerInputTrigger() { // Wait for player input to move to EnemyTurn
      foreach (var _ in Receive<PlayerHasMadeInputTrigger>()) {
         var turn = GetElement<Turn>();

         if (turn.CurrentTurn == TurnType.PlayerTurn) {
            SetCurrentTurn(TurnType.EnemyTurn);
            InputSystem.HandlingInput = true;
         } else {
            InputSystem.HandlingInput = true;
         }
      }
   }

   void ProcessTurnChanges() { // Progress turn phases on changed
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
            default: throw new ArgumentOutOfRangeException();
         }
      }
   }

   async void ResetTurn() {
      await ToSignal(GetElement<SceneTree>().CreateTimer(_minTurnTime), "timeout");
      SetCurrentTurn(TurnType.PlayerTurn);

      await ToSignal(GetElement<SceneTree>().CreateTimer(0.2f), "timeout");
      Send(new NewTurnTrigger());
   }
}