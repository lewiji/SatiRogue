using Godot;
using GodotOnReady.Attributes;
namespace SatiRogue.Ecs.Dungeon.Nodes.Hud;

public partial class FloorCounter : Control {
   [OnReadyGet("%FloorLabel")] Label _floorLabel = null!;

   int _floorNumber = 0;
   public int FloorNumber {
      get => _floorNumber;
      set {
         _floorNumber = value;
         _floorLabel.Text = $"{_floorNumber}F";
      }
   }

   [OnReady] public void SetFloorNumberText() {
      _floorLabel.Text = $"{_floorNumber}F";
   }
}