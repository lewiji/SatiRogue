using System.Collections.Generic;
using Active.Core;
using SatiRogue.Debug;
using SatiRogue.Entities;
using static Active.Status;

namespace SatiRogue.Components.Behaviours;

public class EnemyBehaviourTreeComponent : BehaviourTreeComponent {
   protected override List<Turn.Turn> TurnTypesToExecuteOn { get; set; } = new() {Turn.Turn.EnemyTurn};

   public override void _EnterTree() {
      base._EnterTree();
      Name = "EnemyBehaviourTreeComponent";
      if (ParentEntity is EnemyEntity enemyEntity)
         BehaviourTree = new EnemyBehaviourTree(enemyEntity);
      else
         Logger.Error($"EnemyBehaviourTree: Parent entity {ParentEntity?.Name} was not EnemyEntity");
   }

   private class EnemyBehaviourTree : Gig {
      private readonly EnemyEntity _enemyEntity;
      private float _rangeToPlayer = -1;
      private readonly int _squaredSightRange;

      public EnemyBehaviourTree(EnemyEntity entity) {
         _enemyEntity = entity;
         _squaredSightRange = _enemyEntity.SightRange * _enemyEntity.SightRange;
      }

      public override status Step() {
         return CheckDistanceToPlayer() && CheckLineOfSight() && MoveToPlayer() && Attack();
      }

      private status CheckDistanceToPlayer() {
         _rangeToPlayer = _enemyEntity.DistanceSquaredTo(EntityRegistry.Player!);
         return _rangeToPlayer > _squaredSightRange
            ? fail(log && $"Player was not in squared sight range {_squaredSightRange} of {_enemyEntity.Name}. Range was: {_rangeToPlayer}")
            : done();
      }

      private status CheckLineOfSight() {
         if (!_enemyEntity.HasLineOfSightTo(EntityRegistry.Player!))
            return fail(log && $"Player was not in line of sight of {_enemyEntity.Name} at {_enemyEntity.GridPosition}");
         return done();
      }

      private status MoveToPlayer() {
         if (_rangeToPlayer is -1 or > 1) {
            _enemyEntity.GetComponent<MovementComponent>()?.SetDestination(EntityRegistry.Player!.GridPosition);
            return fail(log && $"Enemy {_enemyEntity.Name} is not adjacent to Player.");
         }

         return done();
      }

      private status Attack() {
         Logger.Info("ATTACKING!!!");
         return done();
      }
   }
}