using System;
using System.Collections.Generic;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Debug;
using SatiRogue.Grid.MapGen;
using SatiRogue.scenes.Hud;

namespace SatiRogue.Entities;

public class StairsEntityParameters : GridEntityParameters {
   public Vector2 Direction;
}

public partial class StairsEntity : GridEntity {
   private StairsEntityParameters? _parameters;

   protected override List<Turn.Turn> TurnTypesToExecuteOn { get; set; } = new () {Turn.Turn.EnemyTurn};

   protected override IGameObjectParameters? Parameters {
      get => _parameters;
      set => _parameters = value as StairsEntityParameters;
   }

   public override void _EnterTree() {
      if (_parameters == null)
         throw new Exception(
            "StairsEntity was added to the tree without StairsEntityParameters; call InitialiseWithParameters before registering the entity.");

      _parameters.BlocksCell = false;
      base.Parameters = _parameters;
      base._EnterTree();
      Name = _parameters.Name ?? $"Stairs{_parameters.Direction}";
      Logger.Info($"Stairs at: {_parameters.GridPosition}");
   }

   public override void HandleTurn() {
      base.HandleTurn();
      if (EntityRegistry.Player != null && EntityRegistry.Player.GridPosition.IsEqualApprox(GridPosition)) {
         ConfirmStairsDialog.ConfirmStairs();
      }
   }
}