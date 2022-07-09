using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Commands.Actions;
using SatiRogue.Entities;
using SatiRogue.Grid;
using SatiRogue.Grid.MapGen;
using SatiRogue.MathUtils;
using SatiRogue.Turn;
using Action = SatiRogue.Commands.Action;

namespace SatiRogue.Components;

public enum MovementDirection {
   Left,
   Up,
   Right,
   Down,
   UpLeft,
   UpRight,
   DownLeft,
   DownRight,
   None
}

public partial class MovementComponent : Component {
   /// <summary>
   ///    Emitted when the entity's GridPosition changes
   /// </summary>
   [Signal]
   public delegate void PositionChanged();

   public static bool _recordingPathfindingCalls;
   public static int numPathingCallsThisTurn;
   private Vector3i? _destination;

   private Vector3i? _gridPosition;

   private Vector3i? _initialPosition;
   private GridEntity? _parent;
   private Queue<Vector3>? _path;
   public Cell? CurrentCell { get; protected set; }


   public Vector3i GridPosition {
      get => _gridPosition.GetValueOrDefault();
      set {
         LastPosition = _gridPosition;
         _gridPosition = value;
         if (_initialPosition == null) {
            _initialPosition = value;
         }
         EmitSignal(nameof(PositionChanged));
      }
   }

   public Vector3i? LastPosition { get; protected set; }
   public Vector3i InputDirection { get; protected set; }


   public override void _Ready() {
      _parent = base.EcOwner as GridEntity;
      _parent?.Connect(nameof(Entity.Died), this, nameof(OnDead));
      _parent?.TurnHandler.Connect(nameof(TurnHandler.OnPlayerTurnStarted), this, nameof(RecordPathfindingCalls));
      _parent?.TurnHandler.Connect(nameof(TurnHandler.OnEnemyTurnStarted), this, nameof(StopRecordingPathfindingCalls));
      SetInitialPosition();
   }

   private void RecordPathfindingCalls() {
      numPathingCallsThisTurn = 0;
      _recordingPathfindingCalls = true;
   }

   private async void StopRecordingPathfindingCalls() {
      //await ToSignal(GetTree(), "idle_frame");
   }

   private async void SetInitialPosition() {
      _parent?.RuntimeMapNode.Connect(nameof(RuntimeMapNode.MapChanged), this, nameof(OnMapChanged));
      await ToSignal(GetTree(), "idle_frame");
      OnMapChanged();
   }

   private async void OnMapChanged() {
      if (EcOwner != null && _initialPosition != null)
      {
         _parent?.RuntimeMapNode.MapData?.GetCellAt(_initialPosition.Value).Occupants.Add(EcOwner.GetInstanceId());
         CurrentCell = _parent?.RuntimeMapNode.MapData?.GetCellAt(_initialPosition.Value);
      }
      if (_parent is EnemyEntity)
      {
         new ActionPickRandomDestination(this._parent!).Execute();
      }
   }

   public void SetDestination(Vector3i? destination) {
      _path = null;
      _destination = destination;
   }

   public bool HasDestination()
   {
      return _destination.HasValue && _path is {Count: > 0};
   }

   public MovementDirection GetNextMovementDirectionOnPath() {
      if (_destination == null) return MovementDirection.None;
      
      if (_path == null) {
         if (_recordingPathfindingCalls) numPathingCallsThisTurn += 1;
         if (_parent?.RuntimeMapNode.MapData != null) {
            _path = new Queue<Vector3>(_parent.RuntimeMapNode.MapData.FindPath(GridPosition, _destination));
            if (_path.Count > 0) _path.Dequeue();
         }
      }
      
      if (_path is {Count: >= 1}) {
         return VectorToMovementDirection(_path.Dequeue() - GridPosition.ToVector3());
      }

      return MovementDirection.None;
   }
   
   public override void HandleTurn() {
      Action action;
      if (_path == null && _destination != null) {
         if (_recordingPathfindingCalls) numPathingCallsThisTurn += 1;
         if (_parent?.RuntimeMapNode.MapData != null) {
            _path = new Queue<Vector3>(_parent!.RuntimeMapNode.MapData.FindPath(GridPosition, _destination));
            if (_path.Count > 0) _path.Dequeue();
         }
      }
   }

   public bool Move(MovementDirection dir) {
      if (EcOwner == null) return false;
      if (!Enabled || !EcOwner.Enabled) return false;

      InputDirection = MovementDirectionToVector(dir);

      var targetPosition = GridPosition + InputDirection;
      var targetCell = _parent?.RuntimeMapNode.MapData?.GetCellAt(targetPosition);

      if (targetCell?.Blocked ?? true) return false;

      _parent?.RuntimeMapNode.MapData?.GetCellAt(GridPosition).RemoveOccupant(EcOwner.GetInstanceId());
      targetCell.Occupants.Add(EcOwner!.GetInstanceId());

      CurrentCell = targetCell;
      GridPosition = targetPosition;

      return true;
   }

   public GameObject[]? TestMoveForOccupants(MovementDirection dir) {
      if (EcOwner == null) return null;

      InputDirection = MovementDirectionToVector(dir);

      var targetPosition = GridPosition + InputDirection;
      var targetCell = MapGenerator.MapData?.GetCellAt(targetPosition);
      return targetCell == null ? 
         Array.Empty<GameObject>() : 
         Array.ConvertAll<ulong, GameObject>(targetCell.Occupants.ToArray(), id => (GameObject) GD.InstanceFromId(id)).Where(inst => 
         IsInstanceValid(inst)).ToArray();
   }


   private void OnDead() {
      RuntimeMapNode.Instance?.MapData?.GetCellAt(GridPosition).RemoveOccupant(EcOwner.GetInstanceId());
      CurrentCell?.Occupants.Remove(EcOwner!.GetInstanceId());
   }

   public static MovementDirection GetRandomMovementDirection() {
      var dir = Mathf.RoundToInt((float)GD.RandRange(0, 7));
      return (MovementDirection)dir;
   }

   public static Vector3i MovementDirectionToVector(MovementDirection dir) {
      switch (dir) {
         case MovementDirection.Left:
            return Vector3i.Left;
         case MovementDirection.Up:
            return Vector3i.Forward;
         case MovementDirection.Right:
            return Vector3i.Right;
         case MovementDirection.Down:
            return Vector3i.Back;
         case MovementDirection.UpLeft:
            return Vector3i.Forward + Vector3i.Left;
         case MovementDirection.UpRight:
            return Vector3i.Forward + Vector3i.Right;
         case MovementDirection.DownLeft:
            return Vector3i.Back + Vector3i.Left;
         case MovementDirection.DownRight:
            return Vector3i.Back + Vector3i.Right;
         case MovementDirection.None:
            return Vector3i.Zero;
         default:
            throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
      }
   }

   public static MovementDirection VectorToMovementDirection(Vector3 vec3) {
      if (vec3.IsEqualApprox(Vector3.Left))
         return MovementDirection.Left;
      if (vec3.IsEqualApprox(Vector3.Right))
         return MovementDirection.Right;
      if (vec3.IsEqualApprox(Vector3.Forward))
         return MovementDirection.Up;
      if (vec3.IsEqualApprox(Vector3.Back))
         return MovementDirection.Down;
      if (vec3.IsEqualApprox(Vector3.Forward + Vector3.Right))
         return MovementDirection.UpRight;
      if (vec3.IsEqualApprox(Vector3.Forward + Vector3.Left))
         return MovementDirection.UpLeft;
      if (vec3.IsEqualApprox(Vector3.Back + Vector3.Right))
         return MovementDirection.DownRight;
      if (vec3.IsEqualApprox(Vector3.Back + Vector3.Left)) return MovementDirection.DownLeft;

      return MovementDirection.None;
   }
}