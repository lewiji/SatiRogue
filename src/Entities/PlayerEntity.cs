using System;
using System.Collections.Generic;
using Godot;
using GoDotNet;
using SatiRogue.Camera;
using SatiRogue.Components;
using SatiRogue.Components.Render;
using SatiRogue.Components.Stats;
using SatiRogue.Components.Tools;
using SatiRogue.Debug;
using SatiRogue.Grid;
using SatiRogue.MathUtils;
namespace SatiRogue.Entities;

public class PlayerEntity : GridEntity {
   [Signal] public delegate void PlayerPositionChanged();

   protected override List<Turn.Turn> TurnTypesToExecuteOn { get; set; } = new() {Turn.Turn.PlayerTurn};

   public override void _EnterTree() {
      base._EnterTree();
      Uuid = Guid.Empty.ToString();
      Name = "Player";
      BlocksCell = true;

      AddComponent(new StatHealthComponent(), new StatsComponentParameters {
         statType = StatEffectTypes.Stat,
         statTypeIndex = (int) StatTypes.Health,
         maxValue = 10,
         minValue = 0,
         initialValue = 10
      });
   }

   private void OnTookDamage(int damage) {
      Logger.Info($"Damaged: {damage}");
      SpatialCamera.Shake(damage);
   }

   public override void Loaded() {
      base.Loaded();
      RuntimeMapNode.Connect(nameof(RuntimeMapNode.MapChanged), this, nameof(OnMapDataChanged));

      CallDeferred(nameof(CheckVisibility));
   }

   protected override void RegisterMovementComponent(Vector3i? gridPosition) {
      MovementComponent = new PlayerMovementComponent();
      MovementComponent.Connect(nameof(MovementComponent.PositionChanged), this, nameof(OnPositionChanged));
      //this.Autoload<Scheduler>().NextFrame(() => {
      AddComponent(MovementComponent);
      AddComponent(new InputHandlerComponent());
      AddComponent(new PlayerRendererComponent());
      AddComponent(new GridIndicatorSpatialComponent());
      AddComponent(new MousePickSpatialCellComponent());
      MovementComponent.GridPosition = gridPosition.GetValueOrDefault();
      // });
   }

   public override void _Ready() {
      Logger.Info("Player ready");
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
      this.Autoload<GameController>().Restart();
   }

   private void OnMapDataChanged() {
      Logger.Debug("Player map data changed");
   }

   public override void HandleTurn() {
      base.HandleTurn();
      Logger.Info("Player handling turn");
      CallDeferred(nameof(UpdateFov));
   }

   private async void UpdateFov() {
      Logger.Info("--- Calculating player FOV ---");
      await ToSignal(GetTree(), "idle_frame");

      if (RuntimeMapNode.MapData != null)
         //ShadowCast.ComputeVisibility(RuntimeMapNode.MapData, GridPosition, 11.0f);
         CheckVisibility();
   }
}