using System;
using Active.Core;
using static Active.Status;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.MathUtils;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Components.Actor; 

public class BehaviourTree {
   private BaseBt? TreeInstance { get; }

   public BehaviourTree() {
      TreeInstance = new BaseBt();
   }

   public BehaviourTree(BaseBt treeInstance) {
      TreeInstance = treeInstance;
   }

   public void Step(World world, Enemy enemy, InputDirectionComponent inputDir, GridPositionComponent gridPos, Nodes.Actors.Player player,
      GridPositionComponent playerGridPos) =>
      TreeInstance!.Step(world, enemy, inputDir, gridPos, player, playerGridPos);
}

public class BaseBt : Gig {
   private int _lastSawPlayer = -1;
   private float _rangeToPlayer = -1;
   public status Step(World world, Enemy enemy, InputDirectionComponent inputDir, GridPositionComponent gridPos, Nodes.Actors.Player player,
      GridPositionComponent playerGridPos) {
      if (!PlayerInRange(enemy, gridPos, playerGridPos)) {
         if (_rangeToPlayer > enemy.SightRange * 2f) {
            return done();
         }

         return MoveRandomly(inputDir);
      }
      if (CheckLineOfSight(world, gridPos, playerGridPos)) {
         return MoveTowardsGridPos(gridPos, playerGridPos, inputDir) || Attack(inputDir);
      }

      if (_lastSawPlayer is > -1 and < 5) {
         return MoveTowardsGridPos(gridPos, playerGridPos, inputDir);
      }

      _lastSawPlayer = -1;
      return MoveRandomly(inputDir);
   }

   private status MoveTowardsGridPos(GridPositionComponent pos1, GridPositionComponent pos2, InputDirectionComponent inputDir) {
      if (_rangeToPlayer is not (-1 or > 1)) return fail();
      inputDir.Direction = (pos2.Position - pos1.Position).Normalized().ToVector2();
      return done();
   }
   
   private status Attack(InputDirectionComponent inputDir) {
      if (Math.Abs(_rangeToPlayer - 1) > 0.4f) return fail();
      Logger.Info("ATTACKING!!!");
      inputDir.Direction = Vector2.Zero;
      // TODO attack
      return done();
   }

   public override status Step() {
      throw new NotImplementedException("Call Step(World, Enemy...) instead");
   }

   private bool PlayerInRange(Enemy enemy, GridPositionComponent gridPos, GridPositionComponent playerGridPos) =>
      DistanceBetween(gridPos, playerGridPos) <= enemy.SightRange;
   
   private status MoveRandomly(InputDirectionComponent inputDir) {
      inputDir.Direction = new Vector2(Mathf.Round((float) GD.RandRange(-1, 1)), Mathf.Round((float) GD.RandRange(-1, 1)));
      return done();
   }
   
   private float DistanceBetween(GridPositionComponent pos1, GridPositionComponent pos2) {
      _rangeToPlayer = (pos1.Position - pos2.Position).Length();
      return _rangeToPlayer;
   }
   
   private bool CheckLineOfSight(World world, GridPositionComponent pos1, GridPositionComponent pos2) {
      var mapData = world.GetElement<MapGenData>();
      var los = BresenhamsLine.Line(pos1.Position, pos2.Position, pos => !mapData.IsWall(pos));
      if (los) {
         _lastSawPlayer = 0;
      }
      else if (_lastSawPlayer != -1) {
         _lastSawPlayer += 1;
      }
      return los;
   }
}