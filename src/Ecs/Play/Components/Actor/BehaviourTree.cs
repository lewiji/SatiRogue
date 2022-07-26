using Active.Core;
using static Active.Status;
using Godot;
using SatiRogue.Debug;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Components.Actor; 

public class BehaviourTree {
   protected BaseBt? TreeInstance { get; private set; }

   public BehaviourTree() {
      TreeInstance = new BaseBt();
   }

   public BehaviourTree(BaseBt treeInstance) {
      TreeInstance = treeInstance;
   }

   public status? Step(World world, InputDirectionComponent inputDir, GridPositionComponent gridPos, GridPositionComponent playerGridPos) {
      return TreeInstance?.Step(world, inputDir, gridPos, playerGridPos);
   }
}

public class BaseBt : Gig {
   private int _rangeToPlayer = -1;
   
   private GridPositionComponent _gridPos;
   private GridPositionComponent _playerGridPos;
   private InputDirectionComponent _inputDir;
   private World _world;

   public status Step(World world, InputDirectionComponent inputDir, GridPositionComponent gridPos, GridPositionComponent playerGridPos) {
      _world = world;
      _gridPos = gridPos;
      _inputDir = inputDir;
      _playerGridPos = playerGridPos;
      return Step();
   }

   public override status Step() {
      if (CheckDistanceToPlayer() > 10f) return done();
      return MoveRandomly();
   }
   
   private status MoveRandomly() {
      _inputDir.Direction = new Vector2(Mathf.Round((float) GD.RandRange(-1, 1)), Mathf.Round((float) GD.RandRange(-1, 1)));
      Logger.Info("Moved randomly");
      //_turnHandler.AddEnemyCommand(new ActionMove(_enemyEntity, MovementComponent.GetRandomMovementDirection()));
      return done();
   }
   
   private int CheckDistanceToPlayer() {
      var player = _world.GetElement<Nodes.Actors.Player>();
      _rangeToPlayer = Mathf.FloorToInt((_gridPos.Position - _playerGridPos.Position)
      .Abs()
      .Length());
      return _rangeToPlayer;
   }
}