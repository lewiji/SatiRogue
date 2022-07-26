using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Grid;

namespace SatiRogue.Entities.VisualRepresentation;

public partial class Enemy2D : Node2D {
   private readonly EnemyEntity? _enemyEntity;
   public Enemy2D() { }

   public Enemy2D(EnemyEntity enemyEntity) {
      _enemyEntity = enemyEntity;
   }

   [OnReady]
   private void ConnectPositionChanged() {
      _enemyEntity?.Connect(nameof(EnemyEntity.EnemyPositionChanged), this, nameof(HandlePositionChanged));
   }

   private void HandlePositionChanged() {
      if (_enemyEntity == null) return;
      var tileWorldPosition = _enemyEntity.GridPosition * TileMapGridRepresentation.TileSize;
      if (tileWorldPosition != null) Position = new Vector2(tileWorldPosition.Value.x, tileWorldPosition.Value.z);
      Visible = _enemyEntity.Visible;
   }
}