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

public partial class TurnHandlerSystem : RefCounted, ISystem {
   
   readonly float _minTurnTime = 0.08f;
   public int TurnNumber { get; private set; }
   Turn? _turn;
   DebugUi? _debugUi;
   World? _world;
   [Signal] public delegate void ExecutePlayerTurnEventHandler();
   [Signal] public delegate void ExecuteNpcTurnEventHandler();
   [Signal] public delegate void ExecuteTurnEndEventHandler();

   void SetCurrentTurn(TurnType turnType) {
      if (_turn == null || (_turn.CurrentTurn == TurnType.Idle && turnType != TurnType.PlayerTurn) || _turn.CurrentTurn == turnType) {
         return;
      }
      _turn.CurrentTurn = turnType;
      CallDeferred(nameof(ProcessTurnChanges));
   }

   public void Run(World world)
   {
      _world ??= world;
      _turn ??= world.GetElement<Turn>();
      _debugUi ??= world.GetElement<DebugUi>();
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

   void ProcessTurnChanges() { // Progress turn phases on changed
      switch (_turn?.CurrentTurn) {
            case TurnType.EnemyTurn:
               SetCurrentTurn(TurnType.Processing);
               EmitSignal(nameof(ExecuteNpcTurn));
               _world!.GetElement<SceneTree>().CreateTimer(_minTurnTime)
                  .Connect("timeout",new Callable(this,nameof(OnEnemyTurnFinished)));
               break;
            case TurnType.PlayerTurn:
               SetCurrentTurn(TurnType.Processing);
               EmitSignal(nameof(ExecutePlayerTurn));
               _world!.GetElement<SceneTree>().CreateTimer(_minTurnTime)
                  .Connect("timeout",new Callable(this,nameof(OnPlayerTurnFinished)));
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

   void OnEnemyTurnFinished()
   {
      EmitSignal(nameof(ExecuteTurnEnd));
      SetCurrentTurn(TurnType.Idle);
   }

   void OnPlayerTurnFinished()
   {
      SetCurrentTurn(TurnType.EnemyTurn);
   }
}