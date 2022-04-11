using Godot;
using RoguelikeMono.Math;

namespace RoguelikeMono.Grid.Entities; 

public class EntityData : Node {
   [Signal] public delegate void PositionChanged();
    
   protected Vector3i _gridPosition;
   public Vector3i GridPosition
   {
      get => _gridPosition;
      set
      {
         _gridPosition = value;
         EmitSignal(nameof(PositionChanged));
      }
   }

   protected bool BlocksCell = false;
   
   public bool Move(MovementDirection dir) {
      var targetPosition = GridPosition + EntityUtils.MovementDirectionToVector(dir);
      var currentCell = GridGenerator._mapData.GetCellAt(GridPosition);
      var targetCell = GridGenerator._mapData.GetCellAt(targetPosition);
      if (!targetCell.Blocked) {
         currentCell.Occupants.Remove(GetInstanceId());
         targetCell.Occupants.Add(GetInstanceId());
         GridPosition = targetPosition;
      }
      return true;
   }
}