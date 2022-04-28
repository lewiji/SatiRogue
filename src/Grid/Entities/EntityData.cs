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
         _lastPosition = _gridPosition;
         _gridPosition = value;
         EmitSignal(nameof(PositionChanged));
      }
   }

   public Vector3i _lastPosition;

   public Vector3i _inputDirection;

   protected bool BlocksCell = false;
   
   public bool Move(MovementDirection dir)
   {
      _inputDirection = EntityUtils.MovementDirectionToVector(dir);
      var targetPosition = GridPosition + _inputDirection;
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