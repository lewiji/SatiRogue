using System;
using Godot;
using SatiRogue.Components;
using SatiRogue.Debug;
using SatiRogue.Grid;
using SatiRogue.MathUtils;
using SatiRogue.Player;

namespace SatiRogue.Entities;

public class PlayerEntity : GridEntity {
   [Signal]
   public delegate void PlayerPositionChanged();

   public override void _EnterTree() {
      base._EnterTree();
      Uuid = Guid.Empty.ToString();
      Name = "Player";
      BlocksCell = true;
      AddComponent(new InputHandlerComponent());
   }

   protected override void RegisterMovementComponent(Vector3i? gridPosition) {
      AddComponent(new PlayerMovementComponent(gridPosition)).Connect(
         nameof(MovementComponent.PositionChanged), this, nameof(OnPositionChanged));
   }

   public override void _Ready() {
      Logger.Info("Player ready");

      GetNode<MapGenerator>(MapGenerator.Path).Connect(nameof(MapGenerator.MapChanged), this, nameof(OnMapDataChanged));
      CallDeferred(nameof(OnPositionChanged));
   }

   protected override void OnPositionChanged() {
      Logger.Info("Player position changed");
      CalculateVisibility();
      EmitSignal(nameof(PlayerPositionChanged));
   }

   private void OnMapDataChanged() {
      Logger.Info("Player map data changed");
      CallDeferred(nameof(CalculateVisibility));
   }

   private void CalculateVisibility() {
      ShadowCast.ComputeVisibility(MapGenerator._mapData, GridPosition, 11.0f);
   }
}