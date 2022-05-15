using System;
using Godot;
using SatiRogue.Grid;
using SatiRogue.Math;

namespace SatiRogue.Entities;

public class EntityData : Node {
   /// <summary>
   /// Emitted when the entity's GridPosition changes
   /// </summary>
   [Signal] public delegate void PositionChanged();
   
   public Guid Uuid { get; protected set; } = Guid.NewGuid();
   // TODO Refactor below into component pattern
   public Vector3i InputDirection { get; protected set; }
   public Vector3i LastPosition { get; protected set; }
   public bool BlocksCell { get; set; }
   public bool Visible { get; set; }
   public Vector3i GridPosition {
      get => _gridPosition;
      set {
         LastPosition = _gridPosition;
         _gridPosition = value;
         Visible = GetIsVisible();
         EmitSignal(nameof(PositionChanged));
      }
   }
   private Vector3i _gridPosition;

   public EntityData(Vector3i? gridPosition = null, bool? blocksCell = null) {
      GridPosition = gridPosition.GetValueOrDefault();
      BlocksCell = blocksCell.GetValueOrDefault();
      Name = "Entity";
   }

   public bool Move(MovementDirection dir) {
      InputDirection = EntityUtils.MovementDirectionToVector(dir);
      var targetPosition = GridPosition + InputDirection;
      var currentCell = MapGenerator._mapData.GetCellAt(GridPosition);
      var targetCell = MapGenerator._mapData.GetCellAt(targetPosition);
      
      if (targetCell.Blocked) return false;
      
      currentCell.Occupants.Remove(GetInstanceId());
      targetCell.Occupants.Add(GetInstanceId());
      GridPosition = targetPosition;
      return true;

   }

   private bool GetIsVisible() {
      return MapGenerator._mapData.GetCellAt(_gridPosition).Visibility == CellVisibility.CurrentlyVisible;
   }
}