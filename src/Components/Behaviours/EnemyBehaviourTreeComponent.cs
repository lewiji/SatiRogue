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
      private int _lastSawPlayer = -1;

      public EnemyBehaviourTree(EnemyEntity entity) {
         _enemyEntity = entity;
         _squaredSightRange = _enemyEntity.SightRange * _enemyEntity.SightRange;
      }

      public override status Step() {
         if (CheckDistanceToPlayer() > _squaredSightRange) return done();
         if (CheckLineOfSight()) {
            Logger.Info($"Enemy has line of sight {_enemyEntity.Name}");
            return MoveToPlayer() || Attack();
         }

         if (_rangeToPlayer <= _enemyEntity.SightRange && _lastSawPlayer is > -1 and < 5) {
            Logger.Info($"Enemy chases unseen player: {_enemyEntity.Name}");
            return MoveToPlayer();
         }
         
         Logger.Info($"Enemy moves randomly: {_enemyEntity.Name}");
         return MoveRandomly();
      }

      private int CheckDistanceToPlayer() {
         _rangeToPlayer = Mathf.FloorToInt((_enemyEntity.GridPosition - EntityRegistry.Player!.GridPosition).Abs().Length());
         return _rangeToPlayer;
      }

      private bool CheckLineOfSight() {
         var los = _enemyEntity.HasLineOfSightTo(EntityRegistry.Player!);
         if (los) {
            _lastSawPlayer = 0;
         }
         else {
            _lastSawPlayer += 1;
         }
         return los;
      }

      private status MoveToPlayer() {
         if (_rangeToPlayer is not (-1 or > 1)) return fail();
         if (_enemyEntity.GetComponent<MovementComponent>() is not { } movementComponent) return done();
         movementComponent.SetDestination(EntityRegistry.Player!.GridPosition);
         Systems.TurnHandler.AddEnemyCommand(new ActionMove(_enemyEntity, 
            movementComponent.GetNextMovementDirectionOnPath()));
         return done();
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
         _enemyEntity.GetComponent<MovementComponent>()?.SetDestination(null);
         Systems.TurnHandler.AddEnemyCommand(new ActionMove(_enemyEntity, MovementComponent.GetRandomMovementDirection()));
         
         return done();
      }
   }
}