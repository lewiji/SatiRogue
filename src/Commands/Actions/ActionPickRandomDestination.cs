using System.Linq;
using Godot;
using SatiRogue.Components;
using SatiRogue.Debug;
using SatiRogue.Entities;
using SatiRogue.Grid;
using SatiRogue.Grid.MapGen;

namespace SatiRogue.Commands.Actions;

public class ActionPickRandomDestination : Action {
   public ActionPickRandomDestination(GridEntity owner) : base(owner) { }

   public override Error Execute() {
      if (Owner?.RuntimeMapNode.MapData == null) return Error.Failed;
      
      var randomCell = GD.Randi() % Owner!.RuntimeMapNode.MapData.Cells.Count();
      var destination = Owner?.RuntimeMapNode.MapData.Cells.ElementAt((int) randomCell).Position;

      if (Owner is GridEntity gridEntity && gridEntity.GetComponent<MovementComponent>() is { } movementComponent) {
         movementComponent.SetDestination(destination);
         movementComponent.Move(movementComponent.GetNextMovementDirectionOnPath());
      }
      else {
         Logger.Warn($"Tried to set destination for entity {Owner}, but no MovementComponent was found.");
         return Error.Failed;
      }

      return Error.Ok;
   }
}