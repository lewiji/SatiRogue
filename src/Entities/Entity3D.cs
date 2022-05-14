using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Debug;

namespace SatiRogue.Entities; 

public partial class Entity3D : Spatial {
   public Entity3D () { }

   public Entity3D(EntityData entityData) {
      _entityData = entityData;
   }

   private readonly EntityData? _entityData;
   
   [OnReady] private void ConnectPositionChanged() {
      _entityData?.Connect(nameof(EntityData.PositionChanged), this, nameof(HandlePositionChanged));
   }

   private void HandlePositionChanged() {
      if (_entityData == null) return;
      Translation = _entityData.GridPosition.ToVector3();
   }
}