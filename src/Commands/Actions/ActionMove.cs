using Godot;
using SatiRogue.Components;
using SatiRogue.Components.Render;
using SatiRogue.Entities;

namespace SatiRogue.Commands.Actions;

public class ActionMove : Action {
   private readonly MovementDirection _inputDir;

   public ActionMove(GridEntity owner, MovementDirection inputDir) : base(owner) {
      _inputDir = inputDir;
   }

   public override Error Execute() {
      if (Owner?.GetType() != typeof(GridEntity) && !Owner!.GetType().IsSubclassOf(typeof(GridEntity))) return Error.Failed;
      var err = ((GridEntity) Owner).GetComponent<MovementComponent>()!.Move(_inputDir) ? Error.Ok : Error.Failed;
      
      if (Owner.GetComponent<AnimatedSprite3DRendererComponent>() is { } spriteComponent)
         spriteComponent.PlayAnimation("walk");
      
      if (err != Error.Ok) NotifyEnemyIsBlocked();
      return err;
   }

   public void NotifyEnemyIsBlocked() {
      //if (Owner is EnemyEntity enemy) new ActionPickRandomDestination((GridEntity)Owner).Execute();
   }
}