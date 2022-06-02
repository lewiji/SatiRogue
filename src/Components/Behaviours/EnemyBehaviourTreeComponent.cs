using System;
using System.Collections.Generic;
using Active;
using Active.Core;
using Godot;
using SatiRogue.Commands.Actions;
using SatiRogue.Debug;
using SatiRogue.Entities;
using SatiRogue.Turn;
using static Active.Status;

namespace SatiRogue.Components.Behaviours;

public class EnemyBehaviourTreeComponent : BehaviourTreeComponent {

   public override void _EnterTree() {
      base._EnterTree();
      Name = "EnemyBehaviourTreeComponent";
      if (EcOwner is EnemyEntity enemyEntity)
         BehaviourTree = new EnemyBehaviourTree(enemyEntity);
      else
         Logger.Error($"EnemyBehaviourTree: Parent entity {EcOwner?.Name} was not EnemyEntity");
   }

   private class EnemyBehaviourTree : Gig {
      private readonly EnemyEntity _enemyEntity;
      private int _squaredSightRange;
      private int _rangeToPlayer = -1;

      public EnemyBehaviourTree(EnemyEntity entity) {
         _enemyEntity = entity;
         _squaredSightRange = _enemyEntity.SightRange * _enemyEntity.SightRange;
      }

      public override status Step() => CheckDistanceToPlayer() && CheckLineOfSight() && (MoveToPlayer() || Attack()) || MoveRandomly();

      private status CheckDistanceToPlayer() {
         _rangeToPlayer = Mathf.FloorToInt((_enemyEntity.GridPosition - EntityRegistry.Player!.GridPosition).Abs().Length());
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
            return done();
         }
         return fail();
      }

      private status Attack() {
         if (_rangeToPlayer == 1) {
            Logger.Info("ATTACKING!!!");
            _enemyEntity.GetComponent<MovementComponent>()?.SetDestination(null);
            Systems.TurnHandler.AddEnemyCommand(new ActionAttack(_enemyEntity, EntityRegistry.Player!));
            return done();
         }
         return fail();
      }

      private status MoveRandomly()
      {
         var enemyMovement = _enemyEntity.GetComponent<MovementComponent>();
         if (enemyMovement != null && enemyMovement.HasDestination())
            return done();
         Logger.Info("Moving randomly");
         Systems.TurnHandler.AddEnemyCommand(new ActionPickRandomDestination(_enemyEntity.GetComponent<MovementComponent>()!));
         return done();
      }
   }
}