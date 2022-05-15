using Godot;
using GodotOnReady.Attributes;

namespace SatiRogue.Entities;

public partial class Entity3D : Spatial {
   private readonly EntityData? _entityData;
   public Entity3D() { }

   public Entity3D(EntityData entityData) {
      _entityData = entityData;
   }

   [OnReady]
   private void ConnectPositionChanged() {
      _entityData?.Connect(nameof(EntityData.PositionChanged), this, nameof(HandlePositionChanged));
   }

   private void HandlePositionChanged() {
      if (_entityData == null) return;
      Translation = _entityData.GridPosition.ToVector3();
      Visible = _entityData.Visible;
   }
}