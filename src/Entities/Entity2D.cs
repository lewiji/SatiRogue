using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Debug;
using SatiRogue.Grid;

namespace SatiRogue.Entities; 

public partial class Entity2D : Node2D {
   public Entity2D () { }
   public Entity2D(EntityData entityData) {
      _entityData = entityData;
   }
   
   private EntityData? _entityData;
   
   [OnReady] private void ConnectPositionChanged() {
      _entityData?.Connect(nameof(EntityData.PositionChanged), this, nameof(HandlePositionChanged));
   }

   private void HandlePositionChanged() {
      if (_entityData == null) return;
      var tileWorldPosition = _entityData.GridPosition * TileMapGridRepresentation.TileSize;
      if (tileWorldPosition != null) Position = new Vector2(tileWorldPosition.Value.x, tileWorldPosition.Value.z);
   }
}