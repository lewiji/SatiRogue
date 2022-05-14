using Godot;
using SatiRogue.Entities;

namespace SatiRogue.Commands.Actions; 

public class Move : Action {
   private MovementDirection InputDir;
   
   public Move(EntityData owner, MovementDirection inputDir) : base(owner) {
      InputDir = inputDir;
   }
   
   public override Error Execute() {
      var err = Owner.Move(InputDir) ? Error.Ok : Error.Failed;
      if (err != Error.Ok) NotifyEnemyIsBlocked();
      return err;
   }

   public void NotifyEnemyIsBlocked() {
      if (Owner is EnemyData enemy) {
         GD.Print("Enemy is blocked!");
         enemy.PickRandomDestination();
      }
   }
}