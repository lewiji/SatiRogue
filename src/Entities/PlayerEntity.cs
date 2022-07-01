using System;
using System.Collections.Generic;
using Godot;
using SatiRogue.Components;
using SatiRogue.Components.Render;
using SatiRogue.Components.Stats;
using SatiRogue.Debug;
using SatiRogue.Grid;
using SatiRogue.Grid.MapGen;
using SatiRogue.MathUtils;
using SatiRogue.Player;

namespace SatiRogue.Entities;

public class PlayerEntity : GridEntity {
   [Signal] public delegate void PlayerPositionChanged();

   [Signal] public delegate void SignalAnimation(string name);

   protected override List<Turn.Turn> TurnTypesToExecuteOn { get; set; } = new() { Turn.Turn.PlayerTurn };

   public override void _EnterTree() {
      base._EnterTree();
      Uuid = Guid.Empty.ToString();
      Name = "Player";
      BlocksCell = true;
      AddComponent(new InputHandlerComponent());
      AddComponent(new StatHealthComponent(10));
      AddComponent(new PlayerRendererComponent());
      AddComponent(new GridIndicatorSpatialComponent());
   }

   protected override void RegisterMovementComponent(Vector3i? gridPosition)
   {
      MovementComponent = new PlayerMovementComponent(gridPosition);
      AddComponent(MovementComponent).Connect(
         nameof(MovementComponent.PositionChanged), this, nameof(OnPositionChanged));
   }

   public override void _Ready() {
      Logger.Info("Player ready");

      RuntimeMapNode.Instance?.Connect(nameof(RuntimeMapNode.MapChanged), this, nameof(OnMapDataChanged));
      Visible = true;
      CallDeferred(nameof(OnPositionChanged));
   }

   protected override void OnPositionChanged() {
      Logger.Debug("Player position changed");
      //CalculateVisibility();
      EmitSignal(nameof(PositionChanged));
      EmitSignal(nameof(PlayerPositionChanged));
   }
   
   protected override async void OnDead() {
      Enabled = false;
      TurnTypesToExecuteOn.Clear();
      DisableComponents();
      await ToSignal(GetTree().CreateTimer(2f), "timeout");
      EntityRegistry.UnregisterEntity(this);
      EntityRegistry.Clear();
      ClearComponents();
      QueueFree();
      GetNode<Systems>("/root/Systems").Restart();
   }

   private void OnMapDataChanged() {
      Logger.Info("Player map data changed");
   }

   public override void HandleTurn()
   {
      base.HandleTurn();
      Logger.Info("Player handling turn");
      CallDeferred(nameof(CalculateVisibility));
   }

   private void CalculateVisibility() {
      if (RuntimeMapNode.Instance?.MapData != null) 
         ShadowCast.ComputeVisibility(RuntimeMapNode.Instance.MapData, GridPosition, 11.0f);
   }
}