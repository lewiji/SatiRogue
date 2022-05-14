using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Commands.Actions;
using SatiRogue.Grid;
using SatiRogue.Math;
using SatiRogue.Turn;

namespace SatiRogue.Entities; 

public enum EnemyTypes { Maw, Ratfolk }

public struct EnemyVariation {
   public EnemyTypes BaseType { get; set; }
   public string? Variation { get; set; } = null;
}

public partial class EnemyData : EntityData {
   private Vector3i _destination;
   public EnemyData(Vector3i? gridPosition = null, EnemyTypes? enemyType = null) : base(gridPosition, true) {
      EnemyType = enemyType.GetValueOrDefault();
      Name = "Enemy";
      PickRandomDestination();
   }

   public void PickRandomDestination() {
      var randomCell = (uint) GD.Randi() % MapGenerator._mapData.Cells.Count();
      _destination = MapGenerator._mapData.Cells.ElementAt((int)randomCell).Position;
   }
   
   public EnemyTypes EnemyType { get; private set; }

   [OnReady] private void ConnectEnemyTurnSignal() {
      Systems.TurnHandler.Connect(nameof(TurnHandler.OnEnemyTurnStarted), this, nameof(HandleTurn));
   }

   private void HandleTurn() {
      var path = MapGenerator._mapData.FindPath(GridPosition, _destination);
      if (path.Length > 1) {
         Systems.TurnHandler.AddEnemyCommand(new Move(this, EntityUtils.VectorToMovementDirection(path[1] - GridPosition.ToVector3())));
      }
      else {
         PickRandomDestination();
      }
   }
}