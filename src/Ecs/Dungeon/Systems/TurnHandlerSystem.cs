using System;
using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Triggers;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class TurnHandlerSystem : Reference, ISystem {
   public World World { get; set; } = null!;
   readonly float _minTurnTime = 0.1f;
   public int TurnNumber { get; private set; }
   Turn? _turn;
   SatiSystemGroup _onTurnSystems;

   public TurnHandlerSystem(SatiSystemGroup onTurnSystems) {
      _onTurnSystems = onTurnSystems;
   }

   void SetCurrentTurn(TurnType turnType) {

      if (_turn == null || (_turn.CurrentTurn == TurnType.Idle && turnType != TurnType.PlayerTurn))
         return;

      _turn.CurrentTurn = turnType;
      
      if (turnType == TurnType.Processing) {
         // Process turn by running OnTurnSystems
         TickWorld();
         TurnNumber += 1;
      }

      CallDeferred(nameof(ProcessOnTurnSystems), _turn.CurrentTurn);
   }

   void ProcessOnTurnSystems(TurnType currentTurnType) {
      this.Send(new TurnChangedTrigger(currentTurnType));
   }

   void TickWorld() {
     _onTurnSystems.Run(World);
   }

   public void Run() {
      _turn ??= World.GetElement<Turn>();
      ProcessTurnChanges();
      HandlePlayerInputTrigger();
   }

   void HandlePlayerInputTrigger() { // Wait for player input to move to EnemyTurn

      if (_turn == null || _turn.CurrentTurn == TurnType.Idle) {
         return;
      }

      foreach (var _ in this.Receive<PlayerHasMadeInputTrigger>()) {
         if (_turn.CurrentTurn == TurnType.PlayerTurn) {
            SetCurrentTurn(TurnType.EnemyTurn);
         }
      }
   }

   void ProcessTurnChanges() { // Progress turn phases on changed
      var triggers = this.Receive<TurnChangedTrigger>();

      foreach (var turnTrigger in triggers) {
         switch (turnTrigger.Turn) {
            case TurnType.Processing:
               ResetTurn();
               return;
            case TurnType.EnemyTurn:
               SetCurrentTurn(TurnType.Processing);
               return;
            case TurnType.PlayerTurn:
               InputSystem.HandlingInput = true;
               return;
            case TurnType.Idle:
               // Handled by player input trigger/idle timeouts
               return;
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