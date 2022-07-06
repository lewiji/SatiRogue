using System;
using System.Collections.Generic;
using Godot;
using SatiRogue.Camera;
using SatiRogue.Components;
using SatiRogue.Components.Render;
using SatiRogue.Components.Stats;
using SatiRogue.Components.Tools;
using SatiRogue.Debug;
using SatiRogue.Grid;
using SatiRogue.Grid.MapGen;
using SatiRogue.MathUtils;
using SatiRogue.Player;

namespace SatiRogue.Entities;

public class PlayerEntity : GridEntity {
   [Signal] public delegate void PlayerPositionChanged();


   protected override List<Turn.Turn> TurnTypesToExecuteOn { get; set; } = new() { Turn.Turn.PlayerTurn };

   public override void _EnterTree() {
      base._EnterTree();
      Uuid = Guid.Empty.ToString();
      Name = "Player";
      BlocksCell = true;
   }

   private void OnTookDamage(int damage) {
      Logger.Info($"Damaged: {damage}");
      SpatialCamera.Shake(damage);
   }

   public override void Loaded() {
      base.Loaded();
      
      AddComponent(new InputHandlerComponent());
      AddComponent(new StatHealthComponent(10)).Connect(nameof(StatsComponent.TookDamage), this, nameof(OnTookDamage));
      AddComponent(new PlayerRendererComponent());
      AddComponent(new GridIndicatorSpatialComponent());
      AddComponent(new MousePickSpatialCellComponent());
   }

   protected override void RegisterMovementComponent(Vector3i? gridPosition)
   {
      MovementComponent = new PlayerMovementComponent(gridPosition);
      Connect(
         nameof(MovementComponent.PositionChanged), this, nameof(OnPositionChanged));
      AddComponent(MovementComponent);
      MovementComponent.GridPosition = gridPosition.GetValueOrDefault();
      CallDeferred(nameof(CheckVisibility));
   }

   public override void _Ready() {
      Logger.Info("Player ready");

      RuntimeMapNode.Instance?.Connect(nameof(RuntimeMapNode.MapChanged), this, nameof(OnMapDataChanged));
      Visible = true;
   }

   protected override void OnPositionChanged() {
      Logger.Debug("Player position changed");
      //CalculateVisibility();
      EmitSignal(nameof(PositionChanged));
      EmitSignal(nameof(PlayerPositionChanged));
   }
   
   protected override async void OnDead() {
      Enabled = false;
      await ToSignal(GetTree().CreateTimer(2f), "timeout");
      GetNode<GameController>(GameController.Path).Restart();
   }

   private void OnMapDataChanged() {
      Logger.Debug("Player map data changed");
   }

   public override void HandleTurn()
   {
      base.HandleTurn();
      Logger.Info("Player handling turn");
      CallDeferred(nameof(UpdateFov));
   }

   private async void UpdateFov() {
      Logger.Info("--- Calculating player FOV ---");
      await ToSignal(GetTree(), "idle_frame");
      if (RuntimeMapNode.Instance?.MapData != null) 
         ShadowCast.ComputeVisibility(RuntimeMapNode.Instance.MapData, GridPosition, 11.0f);
      CheckVisibility();
   }
}