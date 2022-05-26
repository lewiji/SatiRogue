using System;
using System.Collections.Generic;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Commands.Actions;
using SatiRogue.Entities;
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

   public MovementComponent(Vector3i? initialPosition = null) {
      _initialPosition = initialPosition;
   }

   public Vector3i GridPosition {
      get => _gridPosition.GetValueOrDefault();
      set {
         LastPosition = _gridPosition;
         _gridPosition = value;
         EmitSignal(nameof(PositionChanged));
      }
   }

   public Vector3i? LastPosition { get; protected set; }
   protected Vector3i InputDirection { get; set; }

   public override GameObject? Parent {
      get => _parent;
      set => _parent = value as GridEntity;
   }

   protected override List<Turn.Turn> TurnTypesToExecuteOn { get; set; } = new() {Turn.Turn.EnemyTurn};

   [OnReady]
   private void ConnectEnemyTurnSignal() {
      Systems.TurnHandler.Connect(nameof(TurnHandler.OnPlayerTurnStarted), this, nameof(RecordPathfindingCalls));
      Systems.TurnHandler.Connect(nameof(TurnHandler.OnEnemyTurnStarted), this, nameof(StopRecordingPathfindingCalls));
   }

   private void RecordPathfindingCalls() {
      numPathingCallsThisTurn = 0;
      _recordingPathfindingCalls = true;
   }

   private async void StopRecordingPathfindingCalls() {
      //await ToSignal(GetTree(), "idle_frame");
   }

   public override void _EnterTree() {
      GridPosition = _initialPosition.GetValueOrDefault();
      
   }

   [OnReady]
   private void SetInitialPosition() {
      GetNode<MapGenerator>(MapGenerator.Path).Connect(nameof(MapGenerator.MapChanged), this, nameof(OnMapChanged));
   }

   private void OnMapChanged() {
      if (Parent != null && _initialPosition != null)
            MapGenerator.MapData?.GetCellAt(_initialPosition.Value).Occupants.Add(Parent.GetInstanceId());
      new ActionPickRandomDestination(this).Execute();
   }

   public void SetDestination(Vector3i? destination) {
      _path = null;
      _destination = destination;
   }

   public override void HandleTurn() {
      Action action;
      if (_path == null && _destination != null) {
         if (_recordingPathfindingCalls) numPathingCallsThisTurn += 1;
         _path = new Queue<Vector3>(MapGenerator.MapData.FindPath(GridPosition, _destination));
         if (_path.Count > 0) _path.Dequeue();
      }

      if (_path is {Count: >= 1})
         action = new ActionMove(this, VectorToMovementDirection(_path.Dequeue() - GridPosition.ToVector3()));
      else
         action = new ActionPickRandomDestination(this);

      Systems.TurnHandler.AddEnemyCommand(action);
   }

   public bool Move(MovementDirection dir) {
      if (Parent == null) return false;

      InputDirection = MovementDirectionToVector(dir);

      var targetPosition = GridPosition + InputDirection;
      var currentCell = MapGenerator.MapData.GetCellAt(GridPosition);
      var targetCell = MapGenerator.MapData.GetCellAt(targetPosition);

      if (targetCell.Blocked) return false;

      currentCell.Occupants.Remove(Parent!.GetInstanceId());
      targetCell.Occupants.Add(Parent!.GetInstanceId());
      GridPosition = targetPosition;

      return true;
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