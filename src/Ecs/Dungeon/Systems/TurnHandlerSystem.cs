using System;
using Godot;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Triggers;
using RelEcs;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Systems;

public class TurnHandlerSystem : Reference, ISystem {
   public World World { get; set; } = null!;
   readonly float _minTurnTime = 0.1f;
   public int TurnNumber { get; private set; }

   void SetCurrentTurn(TurnType turnType) {
      var turn = World.GetElement<Turn>();

      if (turn.CurrentTurn == TurnType.Idle && turnType != TurnType.PlayerTurn)
         return;

      turn.CurrentTurn = turnType;

      CallDeferred(nameof(ProcessOnTurnSystems), turn.CurrentTurn);
   }

   void ProcessOnTurnSystems(TurnType currentTurnType) {
      if (currentTurnType == TurnType.Processing) {
         // Process turn by running OnTurnSystems
         TickWorld();
         TurnNumber += 1;
      }
      this.Send(new TurnChangedTrigger(currentTurnType));
   }

   void TickWorld() {
      World.GetElement<DungeonState>().OnTurnSystems.Run(World);
   }

   public void Run() {
      ProcessTurnChanges();
      HandlePlayerInputTrigger();
   }

   void HandlePlayerInputTrigger() { // Wait for player input to move to EnemyTurn
      var turn = World.GetElement<Turn>();

      if (turn.CurrentTurn == TurnType.Idle) {
         return;
      }

      foreach (var _ in this.Receive<PlayerHasMadeInputTrigger>()) {
         if (turn.CurrentTurn == TurnType.PlayerTurn) {
            SetCurrentTurn(TurnType.EnemyTurn);
         }
      }

      InputSystem.HandlingInput = true;
   }

   void ProcessTurnChanges() { // Progress turn phases on changed
      var triggers = this.Receive<TurnChangedTrigger>();

      foreach (var turnTrigger in triggers) {
         switch (turnTrigger.Turn) {
            case TurnType.Processing:
               ResetTurn();
               break;
            case TurnType.EnemyTurn:
               SetCurrentTurn(TurnType.Processing);
               break;
            case TurnType.PlayerTurn:
            case TurnType.Idle:
               // Handled by player input trigger/idle timeouts
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }
   }

   async void ResetTurn() {
      SetCurrentTurn(TurnType.Idle);
      await ToSignal(this.GetElement<SceneTree>().CreateTimer(_minTurnTime), "timeout");
      SetCurrentTurn(TurnType.PlayerTurn);

      this.Send(new NewTurnTrigger());
   }
}