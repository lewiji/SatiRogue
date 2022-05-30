using Godot;
using SatiRogue.Components;
using SatiRogue.Entities;

namespace SatiRogue.Commands.Actions;

public class ActionMove : Action {
   private readonly MovementDirection _inputDir;

   public ActionMove(MovementComponent movementComponent, MovementDirection inputDir) : base(movementComponent) {
      _inputDir = inputDir;
   }

   public override Error Execute() {
      if (Owner?.GetType() != typeof(MovementComponent) && !Owner!.GetType().IsSubclassOf(typeof(MovementComponent))) return Error.Failed;
      var err = ((MovementComponent) Owner).Move(_inputDir) ? Error.Ok : Error.Failed;
      if (err != Error.Ok) NotifyEnemyIsBlocked();
      return err;
   }

   public void NotifyEnemyIsBlocked() {
      if (Owner?.EcOwner is EnemyEntity enemy) new ActionPickRandomDestination((MovementComponent) Owner).Execute();
   }
}