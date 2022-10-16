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
   readonly float _minTurnTime = 0.08f;
   public int TurnNumber { get; private set; }
   Turn? _turn;
   DebugUi? _debugUi;

   [Signal] public delegate void ExecutePlayerTurn();
   [Signal] public delegate void ExecuteNpcTurn();
   [Signal] public delegate void ExecuteTurnEnd();

   async void SetCurrentTurn(TurnType turnType) {
      if (_turn == null || (_turn.CurrentTurn == TurnType.Idle && turnType != TurnType.PlayerTurn) || _turn.CurrentTurn == turnType) {
         return;
      }
      _turn.CurrentTurn = turnType;
      _debugUi?.SetTurn(_turn.CurrentTurn);
      ProcessTurnChanges();
   }

   void ProcessOnTurnSystems(TurnType currentTurnType) {
      this.Send(new TurnChangedTrigger(currentTurnType));
   }

   public void Run() {
      _turn ??= World.GetElement<Turn>();
      _debugUi ??= World.GetElement<DebugUi>();
      HandlePlayerInputTrigger();
   }

   async void HandlePlayerInputTrigger() { // Wait for player input to move to EnemyTurn

      if (_turn is not {CurrentTurn: TurnType.Idle}) {
         return;
      }

      if (!InputSystem.PlayerInputted) return;
      SetCurrentTurn(TurnType.PlayerTurn);
      
      InputSystem.PlayerInputted = false;
   }

   async void ProcessTurnChanges() { // Progress turn phases on changed
      switch (_turn?.CurrentTurn) {
            case TurnType.EnemyTurn:
               SetCurrentTurn(TurnType.Processing);
               EmitSignal(nameof(ExecuteNpcTurn));
               await ToSignal(this.GetElement<SceneTree>().CreateTimer(_minTurnTime), "timeout");
               EmitSignal(nameof(ExecuteTurnEnd));
               SetCurrentTurn(TurnType.Idle);
               break;
            case TurnType.PlayerTurn:
               SetCurrentTurn(TurnType.Processing);
               EmitSignal(nameof(ExecutePlayerTurn));
               await ToSignal(this.GetElement<SceneTree>().CreateTimer(_minTurnTime), "timeout");
               SetCurrentTurn(TurnType.EnemyTurn);
               break;
            case TurnType.Idle:
               TurnNumber += 1;
               InputSystem.HandlingInput = true;
               break;
            case TurnType.Processing:
               // Handled by player input trigger/idle timeouts
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
   }
}