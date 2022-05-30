using System.Collections.Generic;
using Godot;
using SatiRogue.Components;
using SatiRogue.Grid;
using SatiRogue.Grid.MapGen;
using SatiRogue.MathUtils;

namespace SatiRogue.Entities;

public class GridEntityParameters : EntityParameters {
   public bool? BlocksCell { get; set; } = null;
   public bool? Visible { get; set; } = null;
   public Vector3i? GridPosition { get; set; } = null;
}

public abstract class GridEntity : Entity {
   private GridEntityParameters? _parameters;
   
   [Signal]
   public delegate void PositionChanged();

   protected override IGameObjectParameters? Parameters {
      get => _parameters;
      set => _parameters = value as GridEntityParameters;
   }

   public bool Visible { get; protected set; }
   public bool BlocksCell { get; protected set; }

   public Vector3i GridPosition {
      get => GetComponent<MovementComponent>()!.GridPosition;
      set => GetComponent<MovementComponent>()!.GridPosition = value;
   }

   public override void _EnterTree() {
      base._EnterTree();
      Name = Parameters?.Name ?? "GridEntity";
      BlocksCell = _parameters?.BlocksCell.GetValueOrDefault() ?? true;
      Visible = _parameters?.Visible.GetValueOrDefault() ?? false;
      RegisterMovementComponent(_parameters?.GridPosition.GetValueOrDefault());
   }

   protected virtual void RegisterMovementComponent(Vector3i? gridPosition) {
      var movementComponent = new MovementComponent(gridPosition);
      AddComponent(movementComponent);
      movementComponent.Connect(nameof(MovementComponent.PositionChanged), this, nameof(OnPositionChanged));
   }

   protected virtual void OnPositionChanged() {
      Visible = GetIsVisible();
      EmitSignal(nameof(PositionChanged));
   }

   private bool GetIsVisible() {
      return MapGenerator.MapData.GetCellAt(GridPosition).Visibility == CellVisibility.CurrentlyVisible;
   }

   public float DistanceSquaredTo(GridEntity otherEntity) {
      return GridPosition.DistanceSquaredTo(otherEntity.GridPosition);
   }

   public bool HasLineOfSightTo(GridEntity otherEntity) {
      return BresenhamsLine.Line(GridPosition, otherEntity.GridPosition, pos => !MapGenerator.MapData.IsWall(pos));
   }
}