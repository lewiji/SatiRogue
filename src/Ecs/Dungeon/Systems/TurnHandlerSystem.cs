using System;
using System.Threading.Tasks;
using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Nodes;
using SatiRogue.Ecs.Dungeon.Triggers;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class TurnHandlerSystem : Reference, ISystem {
   public World World { get; set; } = null!;
   readonly float _minTurnTime = 0.2f;
   public int TurnNumber { get; private set; }
   Turn? _turn;
   SatiSystemGroup _onTurnSystems;
   DebugUi? _debugUi;

   public TurnHandlerSystem(SatiSystemGroup onTurnSystems) {
      _onTurnSystems = onTurnSystems;
   }

   async void SetCurrentTurn(TurnType turnType) {
      if (_turn == null || (_turn.CurrentTurn == TurnType.Idle && turnType != TurnType.PlayerTurn)) {
         return;
      }
      _turn.CurrentTurn = turnType;

      _debugUi?.SetTurn(_turn.CurrentTurn);

      ProcessTurnChanges();
   }

   void ProcessOnTurnSystems(TurnType currentTurnType) {
      this.Send(new TurnChangedTrigger(currentTurnType));
   }

   void TickWorld() {
     _onTurnSystems.Run(World);
   }

   public void Run() {
      _turn ??= World.GetElement<Turn>();
      _debugUi ??= World.GetElement<DebugUi>();
      HandlePlayerInputTrigger();
   }

   async void HandlePlayerInputTrigger() { // Wait for player input to move to EnemyTurn

      if (_turn == null || _turn.CurrentTurn == TurnType.Idle) {
         return;
      }

      if (!InputSystem.PlayerInputted) return;

      if (_turn.CurrentTurn == TurnType.PlayerTurn) {
         SetCurrentTurn(TurnType.EnemyTurn);
      }
      InputSystem.PlayerInputted = false;
   }

   async void ProcessTurnChanges() { // Progress turn phases on changed
      switch (_turn?.CurrentTurn) {
            case TurnType.Processing:
               TickWorld();
               TurnNumber += 1;
               await ResetTurn();
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

   async Task ResetTurn() {
      SetCurrentTurn(TurnType.Idle);
      await ToSignal(this.GetElement<SceneTree>().CreateTimer(_minTurnTime), "timeout");
      SetCurrentTurn(TurnType.PlayerTurn);
   }
}