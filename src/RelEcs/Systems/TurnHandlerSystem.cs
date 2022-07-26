using Godot;
using RelEcs;
using SatiRogue.RelEcs.Triggers;

namespace SatiRogue.RelEcs.Systems; 

public class TurnHandlerSystem : GDSystem {
   private bool _hasRun;
   private Entity _turnHandlerEntity;
   public TurnType CurrentTurn = TurnType.Processing;

   public override void Run() {
      if (!_hasRun) {
         _hasRun = true;
         _turnHandlerEntity = Spawn()
            .Add(new Turn())
            .Id();
      }

      foreach (var turnTrigger in Receive<TurnChangedTrigger>()) {
         if (turnTrigger.Turn == TurnType.PlayerTurn) {
            ResetTurn();
         }
      }

      foreach (var inputTrigger in Receive<PlayerHasMadeInputTrigger>()) {
         var turn = GetComponent<Turn>(_turnHandlerEntity);
         if (turn.CurrentTurn == TurnType.Processing) {
            turn.CurrentTurn = TurnType.PlayerTurn;
            CurrentTurn = turn.CurrentTurn;
            Send(new TurnChangedTrigger(turn.CurrentTurn));
         }
      }
      
   }

   private async void ResetTurn() {
      await ToSignal(GetElement<SceneTree>().CreateTimer(0.25f), "timeout");
      var turn = GetComponent<Turn>(_turnHandlerEntity);
      turn.CurrentTurn = TurnType.Processing;
      CurrentTurn = turn.CurrentTurn;
      Send(new TurnChangedTrigger(turn.CurrentTurn));
   }
}