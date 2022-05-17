using System.Linq;
using Godot;
using SatiRogue.Components;
using SatiRogue.Debug;
using SatiRogue.Entities;
using SatiRogue.Grid;

namespace SatiRogue.Commands.Actions; 

public class ActionPickRandomDestination : Action {
   public ActionPickRandomDestination(MovementComponent owner) : base(owner) { }
   public override Error Execute() {
      var randomCell = GD.Randi() % MapGenerator._mapData.Cells.Count();
      var destination = MapGenerator._mapData.Cells.ElementAt((int) randomCell).Position;
      
      if (Owner is MovementComponent movementComponent) {
         movementComponent.SetDestination(destination);
      } else {
         Logger.Warn($"Tried to set destination for entity {Owner}, but no MovementComponent was found.");
         return Error.Failed;
      }

      return Error.Ok;
   }
}