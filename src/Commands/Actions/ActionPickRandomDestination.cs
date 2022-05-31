using System.Linq;
using Godot;
using SatiRogue.Components;
using SatiRogue.Debug;
using SatiRogue.Grid;
using SatiRogue.Grid.MapGen;

namespace SatiRogue.Commands.Actions;

public class ActionPickRandomDestination : Action {
   public ActionPickRandomDestination(MovementComponent owner) : base(owner) { }

   public override Error Execute() {
      if (RuntimeMapNode.Instance?.MapData == null) return Error.Failed;
      
      var randomCell = GD.Randi() % RuntimeMapNode.Instance.MapData.Cells.Count();
      var destination = RuntimeMapNode.Instance.MapData.Cells.ElementAt((int) randomCell).Position;

      if (Owner is MovementComponent movementComponent) {
         movementComponent.SetDestination(destination);
      }
      else {
         Logger.Warn($"Tried to set destination for entity {Owner}, but no MovementComponent was found.");
         return Error.Failed;
      }

      return Error.Ok;
   }
}