using System.Linq;
using Godot;

namespace SatiRogue.Grid; 

public class RuntimeMapNode : Node {
   private MapData? _mapData;

   [Signal] public delegate void MapChanged();
   [Signal] public delegate void VisibilityChanged(Vector3[] changedCells);

   public MapData? MapData {
      get => _mapData;
      set {
         _mapData = value;
         if (_mapData != null) {
            EmitMapChangedSignal();
         }
      }
   }

   public override async void _Ready() {
      await ToSignal(GetTree(), "idle_frame");
      await ToSignal(GetTree(), "idle_frame");
      EmitMapChangedSignal();
   }


   public override void _Process(float delta) {
      if (MapData?.CellsVisibilityChanged.Count == 0) return;
      
      EmitSignal(nameof(VisibilityChanged), MapData?.CellsVisibilityChanged.ToArray());
      MapData?.CellsVisibilityChanged.Clear();
   }

   private void EmitMapChangedSignal() {
      EmitSignal(nameof(MapChanged));
   }
}