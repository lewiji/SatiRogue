using System.Collections.Generic;
using Godot;
using GoDotNet;
using SatiRogue.Components;
using SatiRogue.Debug;
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
   private bool _visible;
   public MovementComponent? MovementComponent { get; protected set; }

   [Signal]
   public delegate void PositionChanged();

   [Signal]
   public delegate void VisibilityChanged();

   protected override IGameObjectParameters? Parameters
   {
      get => _parameters;
      set
      {
         _parameters = value as GridEntityParameters;
      }
   }

   public bool Visible
   {
      get => _visible;
      set
      {
         _visible = value;
         EmitSignal(nameof(VisibilityChanged));
      }
   }

   public bool BlocksCell { get; protected set; }

   public Vector3i GridPosition
   {
      get => MovementComponent?.GridPosition ?? Vector3i.Zero;
      set
      {
         MovementComponent!.GridPosition = value;
      }
   }

   public override void _EnterTree() {
      base._EnterTree();
      Name = Parameters?.Name ?? "GridEntity";
      BlocksCell = _parameters?.BlocksCell.GetValueOrDefault() ?? true;
      Visible = _parameters?.Visible.GetValueOrDefault() ?? false;
   }

   public override void Loaded() {
      base.Loaded();
      RegisterMovementComponent(_parameters?.GridPosition);
   }

   protected virtual void RegisterMovementComponent(Vector3i? gridPosition) {
      MovementComponent = new MovementComponent();
      Logger.Info($"Registering GridEntity at {gridPosition.GetValueOrDefault().ToVector3()}");
      MovementComponent.Connect(nameof(MovementComponent.PositionChanged), this, nameof(OnPositionChanged));
      this.Autoload<Scheduler>().NextFrame(() => {
         AddComponent(MovementComponent);
         if (gridPosition != null) MovementComponent.GridPosition = gridPosition.Value;
      });
      CallDeferred(nameof(CheckVisibility));
   }

   protected virtual void OnPositionChanged() {
      EmitSignal(nameof(PositionChanged));
      CheckVisibility();
   }

   public override void HandleTurn()
   {
      CallDeferred(nameof(CheckVisibility));
   }

   public void CheckVisibility()
   {
      Visible = GetIsVisible();
   }

   private bool GetIsVisible() {
      return MovementComponent?.CurrentCell?.Visibility == CellVisibility.CurrentlyVisible;
   }

   public float DistanceSquaredTo(GridEntity otherEntity) {
      return GridPosition.DistanceSquaredTo(otherEntity.GridPosition);
   }

   public bool HasLineOfSightTo(GridEntity otherEntity) {
      return BresenhamsLine.Line(GridPosition, otherEntity.GridPosition, pos => !MapGenerator.MapData.IsWall(pos));
   }
}