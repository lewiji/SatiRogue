using Godot;
namespace SatiRogue.Ecs.Dungeon.Nodes.Hud;

public partial class FloorCounter : Control {
   Label _floorLabel = null!;

   int _floorNumber = 0;
   public int FloorNumber {
      get => _floorNumber;
      set {
         _floorNumber = value;
         _floorLabel.Text = $"{_floorNumber}F";
      }
   }

   public override void _Ready()
   {
	   _floorLabel = GetNode<Label>("%FloorLabel");
	   SetFloorNumberText();
   }

   public void SetFloorNumberText() {
      _floorLabel.Text = $"{_floorNumber}F";
   }
}