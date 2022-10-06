using System;
using Active.Core;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using SatiRogue.Ecs.Dungeon.Triggers;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.MapGenerator.Systems;
using SatiRogue.MathUtils;
using SatiRogue.Tools;
using static Active.Status;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Components.Actor;

public class BehaviourTree {
   public BehaviourTree() {
      TreeInstance = new BaseBt();
   }

   public BehaviourTree(BaseBt treeInstance) {
      TreeInstance = treeInstance;
   }

   BaseBt? TreeInstance { get; }
   MessageLog? _messageLog;

   public void Step(World world,
      Enemy enemy,
      InputDirectionComponent inputDir,
      GridPositionComponent gridPos,
      HealthComponent playerHealthComponent,
      GridPositionComponent playerGridPos,
      Stats enemyStats,
      Stats playerStats) {
      _messageLog ??= world.GetElement<MessageLog>();
      TreeInstance!.Step(world, enemy, inputDir, gridPos, playerHealthComponent, playerGridPos, enemyStats, playerStats, _messageLog);
   }
}

public class BaseBt : Gig {
   int _lastSawPlayer = -1;
   int _rangeToPlayer = -1;

   public status Step(World world,
      Enemy enemy,
      InputDirectionComponent inputDir,
      GridPositionComponent gridPos,
      HealthComponent playerHealth,
      GridPositionComponent playerGridPos,
      Stats enemyStats,
      Stats playerStats,
      MessageLog messageLog) {
      if (!PlayerInRange(enemyStats, gridPos, playerGridPos)) {
         if (_rangeToPlayer > enemyStats.Record.SightRange * 2f) {
            return done();
         }

         return MoveRandomly(inputDir);
      }

      if (CheckLineOfSight(world, gridPos, playerGridPos)) {
         return MoveTowardsGridPos(world.GetElement<PathfindingHelper>(), gridPos, playerGridPos, inputDir)
                || Attack(playerHealth, playerGridPos, playerStats, gridPos, inputDir, enemy, enemyStats, world, messageLog)
                || MoveRandomly(inputDir);
      }

      if (_lastSawPlayer is > -1 and < 5) {
         return MoveTowardsGridPos(world.GetElement<PathfindingHelper>(), gridPos, playerGridPos, inputDir) || MoveRandomly(inputDir);
      }

      _lastSawPlayer = -1;

      return MoveRandomly(inputDir);
   }

   status MoveTowardsGridPos(PathfindingHelper pathfindingHelper,
      GridPositionComponent pos1,
      GridPositionComponent pos2,
      InputDirectionComponent inputDir) {
      if (_rangeToPlayer is not (-1 or > 1))
         return fail();
      var path = pathfindingHelper.FindPath(pos1.Position, pos2.Position);

      if (path.Length > 1) {
         inputDir.Direction = (path[1] - pos1.Position).Round().ToVector2();

         return done();
      }

      return fail();
   }

   status Attack(HealthComponent playerHealth,
      GridPositionComponent gridPos,
      Stats playerStats,
      GridPositionComponent playerGridPos,
      InputDirectionComponent inputDir,
      Enemy enemy,
      Stats enemyStats,
      World world,
      MessageLog messageLog) {
      if (_rangeToPlayer is (-1 or > 1))
         return fail();

      var damage = Mathf.Max(0, enemyStats.Record.Strength - playerStats.Record.Defence);
      playerHealth.Value -= damage;

      var player = world.GetElement<Player>();
      messageLog.AddMessage($"[shake]{enemy.CharacterName} hit {player.CharacterName} for {damage} damage![/shake]");

      inputDir.Direction = Vector2.Zero;

      world.Send(new CharacterAnimationTrigger(enemy, "attack"));
      world.Send(new CharacterAnimationTrigger(player, "hit"));

      if (enemy.AnimatedSprite3D != null) {
         enemy.AnimatedSprite3D.FlipH = playerGridPos.Position.x > gridPos.Position.x;
      }

      // TODO attack
      return done();
   }

   public override status Step() {
      throw new NotImplementedException("Call Step(World, Enemy...) instead");
   }

   bool PlayerInRange(Stats enemyStats, GridPositionComponent gridPos, GridPositionComponent playerGridPos) {
      return DistanceBetween(gridPos, playerGridPos) <= enemyStats.Record.SightRange;
   }

   status MoveRandomly(InputDirectionComponent inputDir) {
      inputDir.Direction = new Vector2(Mathf.Round((float) GD.RandRange(-1, 1)), Mathf.Round((float) GD.RandRange(-1, 1)));

      return done();
   }

   float DistanceBetween(GridPositionComponent pos1, GridPositionComponent pos2) {
      _rangeToPlayer = Mathf.FloorToInt((pos1.Position - pos2.Position).Length());

      return _rangeToPlayer;
   }

   bool CheckLineOfSight(World world, GridPositionComponent pos1, GridPositionComponent pos2) {
      var mapData = world.GetElement<MapGenData>();
      var los = BresenhamsLine.Line(pos1.Position, pos2.Position, pos => !mapData.IsWall(pos));

      if (los) {
         _lastSawPlayer = 0;
      } else if (_lastSawPlayer != -1) {
         _lastSawPlayer += 1;
      }

      return los;
   }
}